# chainBlock
chainBlock with Docker for Proejct 7/8

to run the app with Visual Studio:
click start

to run the app with docker:
start up a cmd or ps

cd into the app (library with Dockerfile)
run the following commands:

dotnet publish

docker build -t blockchainimage .

docker run --name blockchaincontainer blockchainimage
