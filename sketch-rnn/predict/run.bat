docker build -t my-tensorflow-app .
docker run --rm -p 5000:5000 -v "%CD%\..\data:/usr/src/app/data" -v "%CD%\..\model_cp:/usr/src/app/model_cp" -it my-tensorflow-app
