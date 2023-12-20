@echo off

:: Licensed under the MIT license. See LICENSE file in the project root for full license information.

:: OpenFGA CLI Executable
set OPENFGA_CLI_EXECUTABLE=C:\Users\philipp\apps\fga_0.2.1_windows_amd64\fga.exe

:: Parameters for the Model Transformation
set FGA_MODEL_FILENAME=%~dp0/src/Server/RebacExperiments.Server.Api/Resources/github-model.fga
set JSON_MODEL_FILENAME=%~dp0/src/Server/RebacExperiments.Server.Api/Resources/github-model.json


:: Run the "fga model transform" Command
%OPENFGA_CLI_EXECUTABLE% model transform --input-format fga --file %FGA_MODEL_FILENAME%