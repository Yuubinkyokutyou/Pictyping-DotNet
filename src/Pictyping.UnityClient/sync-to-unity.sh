#!/bin/bash

echo "====================================="
echo "Pictyping Unity Client Sync Script"
echo "====================================="
echo

# Check if Unity project path is provided
if [ $# -eq 0 ]; then
    echo "Usage: ./sync-to-unity.sh [Unity Project Path]"
    echo "Example: ./sync-to-unity.sh \"/path/to/MyUnityProject\""
    echo
    echo "Please provide the path to your Unity project."
    exit 1
fi

UNITY_PROJECT_PATH="$1"
SOURCE_PATH="$(dirname "$0")/Runtime"
DEST_PATH="$UNITY_PROJECT_PATH/Assets/Pictyping.UnityClient"

echo "Unity Project Path: $UNITY_PROJECT_PATH"
echo "Source Path: $SOURCE_PATH"
echo "Destination Path: $DEST_PATH"
echo

# Check if Unity project exists
if [ ! -d "$UNITY_PROJECT_PATH" ]; then
    echo "Error: Unity project path does not exist: $UNITY_PROJECT_PATH"
    exit 1
fi

# Check if Assets folder exists
if [ ! -d "$UNITY_PROJECT_PATH/Assets" ]; then
    echo "Error: Assets folder not found. Please make sure this is a Unity project: $UNITY_PROJECT_PATH"
    exit 1
fi

# Check if source files exist
if [ ! -d "$SOURCE_PATH" ]; then
    echo "Error: Unity client files not found at: $SOURCE_PATH"
    echo "Please run ./generate.sh first to generate the Unity client, or check the Runtime folder."
    exit 1
fi

echo "Copying Unity client files..."

# Remove existing destination folder if it exists
if [ -d "$DEST_PATH" ]; then
    echo "Removing existing Unity client folder..."
    rm -rf "$DEST_PATH"
fi

# Create destination directory
mkdir -p "$DEST_PATH"

# Copy all files from source to destination
echo "Copying files from source to Unity project..."
cp -r "$SOURCE_PATH"/* "$DEST_PATH/"

if [ $? -eq 0 ]; then
    echo
    echo "====================================="
    echo "Sync completed successfully!"
    echo "====================================="
    echo
    echo "Unity client has been copied to:"
    echo "$DEST_PATH"
    echo
    echo "Next steps:"
    echo "1. Open Unity and refresh the project"
    echo "2. Install 'Newtonsoft.Json for Unity' from Package Manager if not already installed"
    echo "3. Check the Assembly Definition file for proper dependencies"
    echo "4. Review the sample code in Samples~/BasicUsage/"
    echo
    echo "For detailed integration guide, see UNITY_INTEGRATION.md"
else
    echo
    echo "Error: Failed to copy files."
    exit 1
fi