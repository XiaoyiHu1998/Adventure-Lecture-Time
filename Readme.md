# Prerequisites
- Docker needs to be installed
- AUTOMATIC1111 needs to be installed
- Controlnet needs to be installed for AUTOMATIC1111, follow the instructions in the [Controlnet repository](https://github.com/Mikubill/sd-webui-controlnet?tab=readme-ov-file#installation)
- Download the [control_v11p_sd15_scribble_fp16](https://huggingface.co/comfyanonymous/ControlNet-v1-1_fp16_safetensors/blob/main/control_v11p_sd15_scribble_fp16.safetensors) model and place it in the `models/ControlNet` folder of AUTOMATIC1111
- Download the [v1-5-pruned-emaonly](https://huggingface.co/wsj1995/stable-diffusion-models/blob/4bdfc26ef64249f9d55d28e0db9a0ee638c6a309/v1-5-pruned-emaonly.safetensors) model and place it in the `models/Stable-diffusion` folder of AUTOMATIC1111

# Running
- Docker needs to be running
- Run the `run_predict.bat` file to start the sketch-rnn server
- Run AUTOMATIC1111 with the `--api` command line argument
- Start the `Adventure Lecture Time.exe` in the `Build` folder.

Note: The game works best at an aspect ratio of 16:9.

# Source code
The source code for the game can be found in the `UnityProject` folder.
The source code for the sketch-rnn server that predict the object in a drawing can be found in the `sketch-rnn` folder.
