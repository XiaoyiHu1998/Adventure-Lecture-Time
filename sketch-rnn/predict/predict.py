from __future__ import absolute_import
from __future__ import division
from __future__ import print_function
import argparse
import json
import ast
import functools
import sys
import numpy as np
import flask as fl
import tensorflow as tf

app = fl.Flask(__name__)

def parse_line(ndjson_line):
  """Parse an ndjson line and return tf.Example."""
  sample = json.loads(ndjson_line)
  inkarray = sample["drawing"]
  stroke_lengths = [len(stroke[0]) for stroke in inkarray]
  total_points = sum(stroke_lengths)
  np_ink = np.zeros((total_points, 3), dtype=np.float32)
  current_t = 0
  if not inkarray:
    print("Empty inkarray")
    return None
  for stroke in inkarray:
    if len(stroke[0]) != len(stroke[1]):
      print("Inconsistent number of x and y coordinates.")
      return None
    for i in [0, 1]:
      np_ink[current_t:(current_t + len(stroke[0])), i] = stroke[i]
    current_t += len(stroke[0])
    np_ink[current_t - 1, 2] = 1  # stroke_end
  # Preprocessing.
  # 1. Size normalization.
  lower = np.min(np_ink[:, 0:2], axis=0)
  upper = np.max(np_ink[:, 0:2], axis=0)
  scale = upper - lower
  scale[scale == 0] = 1
  np_ink[:, 0:2] = (np_ink[:, 0:2] - lower) / scale
  # 2. Compute deltas.
  np_ink[1:, 0:2] -= np_ink[0:-1, 0:2]
  np_ink = np_ink[1:, :]
  features = {}
  features["ink"] = tf.train.Feature(float_list=tf.train.FloatList(
      value=np_ink.flatten()))
  features["shape"] = tf.train.Feature(int64_list=tf.train.Int64List(
      value=np_ink.shape))
  f = tf.train.Features(feature=features)
  example = tf.train.Example(features=f)
  return example

def get_classes():
  classes = []
  with tf.gfile.GFile(FLAGS.classes_file, "r") as f:
    classes = [x.strip() for x in f]
  return classes


def get_input_fn(tfrecord_pattern):
  """Creates an input_fn that stores all the data in memory.

  Args:
   mode: one of tf.contrib.learn.ModeKeys.{TRAIN, INFER, EVAL}
   tfrecord_pattern: path to a TF record file created using create_dataset.py.
   batch_size: the batch size to output.

  Returns:
    A valid input_fn for the model estimator.
  """

  def _parse_tfexample_fn(example_proto):
    """Parse a single record which is expected to be a tensorflow.Example."""
    feature_to_type = {
        "ink": tf.VarLenFeature(dtype=tf.float32),
        "shape": tf.FixedLenFeature([2], dtype=tf.int64)
    }

    parsed_features = tf.parse_single_example(example_proto, feature_to_type)
    parsed_features["ink"] = tf.sparse_tensor_to_dense(parsed_features["ink"])
    return parsed_features

  def _input_fn():
    """Estimator `input_fn`.

    Returns:
      A tuple of:
      - Dictionary of string feature name to `Tensor`.
      - `Tensor` of target labels.
    """
    dataset = tf.data.Dataset.from_tensors(tfrecord_pattern.SerializeToString())
    dataset = dataset.map(_parse_tfexample_fn)
    dataset = dataset.batch(1)
    features = dataset.make_one_shot_iterator().get_next()
    return features, None

  return _input_fn


def model_fn(features, labels, mode, params):
  """Model function for RNN classifier.

  This function sets up a neural network which applies convolutional layers (as
  configured with params.num_conv and params.conv_len) to the input.
  The output of the convolutional layers is given to LSTM layers (as configured
  with params.num_layers and params.num_nodes).
  The final state of the all LSTM layers are concatenated and fed to a fully
  connected layer to obtain the final classification scores.

  Args:
    features: dictionary with keys: inks, lengths.
    labels: one hot encoded classes
    mode: one of tf.estimator.ModeKeys.{TRAIN, INFER, EVAL}
    params: a parameter dictionary with the following keys: num_layers,
      num_nodes, batch_size, num_conv, conv_len, num_classes, learning_rate.

  Returns:
    ModelFnOps for Estimator API.
  """

  def _get_input_tensors(features, labels):
    """Converts the input dict into inks, lengths, and labels tensors."""
    # features[ink] is a sparse tensor that is [8, batch_maxlen, 3]
    # inks will be a dense tensor of [8, maxlen, 3]
    # shapes is [batchsize, 2]
    shapes = features["shape"]
    # lengths will be [batch_size]
    lengths = tf.squeeze(
        tf.slice(shapes, begin=[0, 0], size=[params.batch_size, 1]), axis=1)
    inks = tf.reshape(features["ink"], [params.batch_size, -1, 3])
    if labels is not None:
      labels = tf.squeeze(labels)
    return inks, lengths, labels

  def _add_conv_layers(inks, lengths):
    """Adds convolution layers."""
    convolved = inks
    for i in range(len(params.num_conv)):
      convolved_input = convolved
      if params.batch_norm:
        convolved_input = tf.layers.batch_normalization(
            convolved_input,
            training=(mode == tf.estimator.ModeKeys.TRAIN))
      # Add dropout layer if enabled and not first convolution layer.
      if i > 0 and params.dropout:
        convolved_input = tf.layers.dropout(
            convolved_input,
            rate=params.dropout,
            training=(mode == tf.estimator.ModeKeys.TRAIN))
      convolved = tf.layers.conv1d(
          convolved_input,
          filters=params.num_conv[i],
          kernel_size=params.conv_len[i],
          activation=None,
          strides=1,
          padding="same",
          name="conv1d_%d" % i)
    return convolved, lengths

  def _add_regular_rnn_layers(convolved, lengths):
    """Adds RNN layers."""
    if params.cell_type == "lstm":
      cell = tf.nn.rnn_cell.BasicLSTMCell
    elif params.cell_type == "block_lstm":
      cell = tf.contrib.rnn.LSTMBlockCell
    cells_fw = [cell(params.num_nodes) for _ in range(params.num_layers)]
    cells_bw = [cell(params.num_nodes) for _ in range(params.num_layers)]
    if params.dropout > 0.0:
      cells_fw = [tf.contrib.rnn.DropoutWrapper(cell) for cell in cells_fw]
      cells_bw = [tf.contrib.rnn.DropoutWrapper(cell) for cell in cells_bw]
    outputs, _, _ = tf.contrib.rnn.stack_bidirectional_dynamic_rnn(
        cells_fw=cells_fw,
        cells_bw=cells_bw,
        inputs=convolved,
        sequence_length=lengths,
        dtype=tf.float32,
        scope="rnn_classification")
    return outputs

  def _add_cudnn_rnn_layers(convolved):
    """Adds CUDNN LSTM layers."""
    # Convolutions output [B, L, Ch], while CudnnLSTM is time-major.
    convolved = tf.transpose(convolved, [1, 0, 2])
    lstm = tf.contrib.cudnn_rnn.CudnnLSTM(
        num_layers=params.num_layers,
        num_units=params.num_nodes,
        dropout=params.dropout if mode == tf.estimator.ModeKeys.TRAIN else 0.0,
        direction="bidirectional")
    outputs, _ = lstm(convolved)
    # Convert back from time-major outputs to batch-major outputs.
    outputs = tf.transpose(outputs, [1, 0, 2])
    return outputs

  def _add_rnn_layers(convolved, lengths):
    """Adds recurrent neural network layers depending on the cell type."""
    if params.cell_type != "cudnn_lstm":
      outputs = _add_regular_rnn_layers(convolved, lengths)
    else:
      outputs = _add_cudnn_rnn_layers(convolved)
    # outputs is [batch_size, L, N] where L is the maximal sequence length and N
    # the number of nodes in the last layer.
    mask = tf.tile(
        tf.expand_dims(tf.sequence_mask(lengths, tf.shape(outputs)[1]), 2),
        [1, 1, tf.shape(outputs)[2]])
    zero_outside = tf.where(mask, outputs, tf.zeros_like(outputs))
    outputs = tf.reduce_sum(zero_outside, axis=1)
    return outputs

  def _add_fc_layers(final_state):
    """Adds a fully connected layer."""
    return tf.layers.dense(final_state, params.num_classes)

  # Build the model.
  inks, lengths, labels = _get_input_tensors(features, labels)
  convolved, lengths = _add_conv_layers(inks, lengths)
  final_state = _add_rnn_layers(convolved, lengths)
  logits = _add_fc_layers(final_state)

  # Compute current predictions.
  predictions = tf.argmax(logits, axis=1)

  if mode == tf.estimator.ModeKeys.PREDICT:
    # Return the predictions and the specification for serving a SavedModel
    export_outputs = {
        'predict': tf.estimator.export.PredictOutput({"logits": logits, "predictions": predictions})
    }
    return tf.estimator.EstimatorSpec(
        mode=mode,
        predictions={"logits": logits, "predictions": predictions},
        export_outputs=export_outputs)

  # Add the loss.
  cross_entropy = tf.reduce_mean(
      tf.nn.sparse_softmax_cross_entropy_with_logits(
          labels=labels, logits=logits))
  # Add the optimizer.
  train_op = tf.contrib.layers.optimize_loss(
      loss=cross_entropy,
      global_step=tf.train.get_global_step(),
      learning_rate=params.learning_rate,
      optimizer="Adam",
      # some gradient clipping stabilizes training in the beginning.
      clip_gradients=params.gradient_clipping_norm,
      summaries=["learning_rate", "loss", "gradients", "gradient_norm"])

  return tf.estimator.EstimatorSpec(
      mode=mode,
      predictions={"logits": logits, "predictions": predictions},
      loss=cross_entropy,
      train_op=train_op,
      eval_metric_ops={"accuracy": tf.metrics.accuracy(labels, predictions)})

@app.route('/predict-top', methods=['POST'])
def predict_top():
  """
  Predicts the top n classes of a drawing.
  Input is a JSON dictionary containing the drawing to be classified.
  Outputs a JSON dictionary containing the top n classes.
  """
  top_n = fl.request.args.get('n', default=3, type=int)

  input_njson = fl.request.get_json()

  classes = get_classes()

  for prediction in predictions(input_njson):
    # Get the top n classes, in descending order of probability
    top_classes = np.argsort(prediction['logits'])[-top_n:][::-1]
    for i in top_classes:
      print(classes[i], ": ", prediction['logits'][i])
    return fl.jsonify({'classes': [classes[i] for i in top_classes]})

@app.route('/predict', methods=['POST'])
def predict():
  """
  Predicts the class of a drawing. 
  Input is a JSON dictionary containing the drawing to be classified.
  Outputs a JSON dictionary containing the predicted class.
  """
  input_njson = fl.request.get_json()

  classes = get_classes()

  for prediction in predictions(input_njson):
    print("Predicted: ", classes[prediction['predictions']])
    return fl.jsonify({'class': classes[prediction['predictions']]})

def predictions(input_njson):
  """
  Predicts the class of a drawing.

  Args:
    input_njson: A JSON dictionary containing the drawing to be classified.

  Returns:
    A dictionary containing the predicted class and the probabilities of each class.
  """
  input_njson_str = json.dumps(input_njson)

  model_params = tf.contrib.training.HParams(
    num_layers=FLAGS.num_layers,
    num_nodes=FLAGS.num_nodes,
    batch_size=1,
    num_conv=ast.literal_eval(FLAGS.num_conv),
    conv_len=ast.literal_eval(FLAGS.conv_len),
    num_classes=len(get_classes()),
    learning_rate=FLAGS.learning_rate,
    gradient_clipping_norm=FLAGS.gradient_clipping_norm,
    cell_type=FLAGS.cell_type,
    batch_norm=FLAGS.batch_norm,
    dropout=FLAGS.dropout)

  estimator = tf.estimator.Estimator(model_fn, params=model_params, model_dir=FLAGS.model_dir)

  predictions = estimator.predict(input_fn=get_input_fn(parse_line(input_njson_str)))

  return predictions


if __name__ == "__main__":
  parser = argparse.ArgumentParser()
  parser.register("type", "bool", lambda v: v.lower() == "true")
  parser.add_argument(
      "--training_data",
      type=str,
      default="",
      help="Path to training data (tf.Example in TFRecord format)")
  parser.add_argument(
      "--eval_data",
      type=str,
      default="",
      help="Path to evaluation data (tf.Example in TFRecord format)")
  parser.add_argument(
      "--classes_file",
      type=str,
      default="",
      help="Path to a file with the classes - one class per line")
  parser.add_argument(
      "--num_layers",
      type=int,
      default=3,
      help="Number of recurrent neural network layers.")
  parser.add_argument(
      "--num_nodes",
      type=int,
      default=128,
      help="Number of node per recurrent network layer.")
  parser.add_argument(
      "--num_conv",
      type=str,
      default="[48, 64, 96]",
      help="Number of conv layers along with number of filters per layer.")
  parser.add_argument(
      "--conv_len",
      type=str,
      default="[5, 5, 3]",
      help="Length of the convolution filters.")
  parser.add_argument(
      "--cell_type",
      type=str,
      default="lstm",
      help="Cell type used for rnn layers: cudnn_lstm, lstm or block_lstm.")
  parser.add_argument(
      "--batch_norm",
      type="bool",
      default="False",
      help="Whether to enable batch normalization or not.")
  parser.add_argument(
      "--learning_rate",
      type=float,
      default=0.0001,
      help="Learning rate used for training.")
  parser.add_argument(
      "--gradient_clipping_norm",
      type=float,
      default=9.0,
      help="Gradient clipping norm used during training.")
  parser.add_argument(
      "--dropout",
      type=float,
      default=0.3,
      help="Dropout used for convolutions and bidi lstm layers.")
  parser.add_argument(
      "--steps",
      type=int,
      default=100000,
      help="Number of training steps.")
  parser.add_argument(
      "--model_dir",
      type=str,
      default="",
      help="Path for storing the model checkpoints.")
  parser.add_argument(
      "--self_test",
      type="bool",
      default="False",
      help="Whether to enable batch normalization or not.")

  FLAGS, unparsed = parser.parse_known_args()
  app.run(host='0.0.0.0', port=5000)
