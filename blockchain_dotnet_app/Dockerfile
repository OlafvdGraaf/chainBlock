FROM microsoft/dotnet:2.0.4-runtime-nanoserver-1709 AS base  
   
WORKDIR /app  
COPY /bin/Debug/netcoreapp2.0/publish/ .  
   
ENTRYPOINT ["dotnet", "blockchain_dotnet_app.dll"]  