# Prerequisites
- Docker needs to be installed
- AUTOMATIC1111 needs to be installed
- Controlnet needs to be installed for AUTOMATIC1111, follow the instructions in the [Controlnet repository](https://github.com/Mikubill/sd-webui-controlnet?tab=readme-ov-file#installation)
- Download the [control_v11p_sd15_scribble_fp16](https://huggingface.co/comfyanonymous/ControlNet-v1-1_fp16_safetensors/blob/main/control_v11p_sd15_scribble_fp16.safetensors) model and place it in the `models/ControlNet` folder of AUTOMATIC1111
- Download the [v1-5-pruned-emaonly](https://huggingface.co/wsj1995/stable-diffusion-models/blob/4bdfc26ef64249f9d55d28e0db9a0ee638c6a309/v1-5-pruned-emaonly.safetensors) model and place it in the `models/Stable-diffusion` folder of AUTOMATIC1111

# Running
- Run the run.bat file in `sketch-rnn/predict` to start the sketch-rnn server
- Run AUTOMATIC1111 with the `--api` command line argument
