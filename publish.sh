#/bin/bash
rm -rf publish
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -o publish/ GarLoader.MySqlUploader

