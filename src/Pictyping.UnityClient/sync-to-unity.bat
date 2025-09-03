@echo off
setlocal enabledelayedexpansion

echo =====================================
echo Pictyping Unity Client Sync Script
echo =====================================
echo.

REM Check if Unity project path is provided
if "%~1"=="" (
    echo Usage: sync-to-unity.bat [Unity Project Path]
    echo Example: sync-to-unity.bat "C:\MyUnityProject"
    echo.
    echo Please provide the path to your Unity project.
    pause
    exit /b 1
)

set UNITY_PROJECT_PATH=%~1
set SOURCE_PATH=%~dp0Runtime
set DEST_PATH=%UNITY_PROJECT_PATH%\Assets\Pictyping.UnityClient

echo Unity Project Path: %UNITY_PROJECT_PATH%
echo Source Path: %SOURCE_PATH%
echo Destination Path: %DEST_PATH%
echo.

REM Check if Unity project exists
if not exist "%UNITY_PROJECT_PATH%" (
    echo Error: Unity project path does not exist: %UNITY_PROJECT_PATH%
    pause
    exit /b 1
)

REM Check if Assets folder exists
if not exist "%UNITY_PROJECT_PATH%\Assets" (
    echo Error: Assets folder not found. Please make sure this is a Unity project: %UNITY_PROJECT_PATH%
    pause
    exit /b 1
)

REM Check if source files exist
if not exist "%SOURCE_PATH%" (
    echo Error: Unity client files not found at: %SOURCE_PATH%
    echo Please run generate.bat first to generate the Unity client, or check the Runtime folder.
    pause
    exit /b 1
)

echo Copying Unity client files...

REM Remove existing destination folder if it exists
if exist "%DEST_PATH%" (
    echo Removing existing Unity client folder...
    rmdir /s /q "%DEST_PATH%"
)

REM Create destination directory
mkdir "%DEST_PATH%"

REM Copy all files from source to destination
echo Copying files from source to Unity project...
xcopy "%SOURCE_PATH%\*" "%DEST_PATH%\" /s /e /y

if %errorlevel% equ 0 (
    echo.
    echo =====================================
    echo Sync completed successfully!
    echo =====================================
    echo.
    echo Unity client has been copied to:
    echo %DEST_PATH%
    echo.
    echo Next steps:
    echo 1. Open Unity and refresh the project
    echo 2. Install 'Newtonsoft.Json for Unity' from Package Manager if not already installed
    echo 3. Check the Assembly Definition file for proper dependencies
    echo 4. Review the sample code in Samples~/BasicUsage/
    echo.
    echo For detailed integration guide, see UNITY_INTEGRATION.md
) else (
    echo.
    echo Error: Failed to copy files. Error code: %errorlevel%
    pause
    exit /b 1
)

pause