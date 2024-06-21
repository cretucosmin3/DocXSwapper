#!/bin/bash

# Generic
APP_NAME=DocXSwapper
CONFIG_PATH=../Config.conf

# Linux
LIN_PROGRAM_PATH="../bin/Release/net8.0/linux-x64/publish/$APP_NAME"
LIN_DESTINATION="../Downloads/Linux/"

# Windows
WIN_PROGRAM_PATH="../bin/Release/net8.0/win-x64/publish/$APP_NAME.exe"
WIN_DESTINATION="../Downloads/Windows/"

# Publish the project
dotnet publish ../ -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
dotnet publish ../ -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true

# Move linux file
cp "$LIN_PROGRAM_PATH" "$LIN_DESTINATION"

# Move windows file
cp "$WIN_PROGRAM_PATH" "$WIN_DESTINATION"

# Make linux file executable
chmod u+x $LIN_DESTINATION$APP_NAME

# Print completion message
echo "Done."