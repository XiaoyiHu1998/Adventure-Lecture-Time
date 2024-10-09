docker build -t my-tensorflow-app .
docker run --gpus all --rm --runtime=nvidia -v "%CD%\..\data:/usr/src/app/data" -v "%CD%\..\model_cp:/usr/src/app/model_cp" -it my-tensorflow-app
