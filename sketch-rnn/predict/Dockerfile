# Use an official TensorFlow runtime as a parent image
FROM tensorflow/tensorflow:1.15.5-gpu-py3

# Set the working directory in the container
WORKDIR /usr/src/app

# Copy the current directory contents into the container at /usr/src/app
COPY . .

# Install any needed packages specified in requirements.txt
RUN pip install --no-cache-dir -r requirements.txt

# Run train_model.py when the container launches
CMD ["python", "./predict.py", "--model_dir", "model_cp", "--classes_file", "data/training.tfrecord.classes", "--cell_type",  "block_lstm", "--classes_file", "data/training.tfrecord.classes"]
