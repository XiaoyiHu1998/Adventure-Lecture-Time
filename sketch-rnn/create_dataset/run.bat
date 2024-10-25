docker build -t my-tensorflow-app .
docker run --rm -v "%CD%\..\data:/usr/src/app/data" -v "%CD%\sketch_data:/usr/src/app/sketch_data" -it my-tensorflow-app
