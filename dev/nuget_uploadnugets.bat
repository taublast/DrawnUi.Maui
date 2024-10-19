@echo off
setlocal enabledelayedexpansion

REM Check if API key is provided as a parameter
if "%~1"=="" (
    REM Ask for API key
    set /p "APIKEY=Please enter your GitHub API key: "
) else (
    set "APIKEY=%~1"
)

REM Define the source directory for the packages
set "source_dir=E:\Nugets"

REM Define the file mask for the packages
REM set "file_mask=DrawnUi.Maui*.1.2.3.8*.nupkg"
set "file_mask=AppoMobi.Maui.DrawnUi.1.2.3.8*.*nupkg"

REM Loop through each package file in the source directory
for %%f in ("%source_dir%\%file_mask%") do (
    echo Uploading %%f to NUGET.ORG with API key.
    nuget push %%f -Source https://api.nuget.org/v3/index.json -ApiKey %APIKEY%
    if errorlevel 1 (
        echo An error occurred while uploading %%f.
    ) else (
        echo %%f uploaded successfully.
    )
)

pause
endlocal

 