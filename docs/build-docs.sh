#!/bin/bash

# Build script for DrawnUi documentation

serve=false
if [ "$1" = "--serve" ]; then
    serve=true
fi

echo "Building DrawnUi documentation..."

# Check if DocFX is installed
if ! command -v docfx &> /dev/null; then
    echo "DocFX not found in PATH. Installing DocFX as global tool..."
    dotnet tool install --global docfx
    
    # Check if installation was successful
    if ! command -v docfx &> /dev/null; then
        echo "Failed to install DocFX. Please install it manually."
        exit 1
    fi
fi

# Clean previous build
if [ -d "_site" ]; then
    echo "Cleaning previous build..."
    rm -rf _site
    rm -rf obj
fi

# Build the documentation
echo "Running DocFX build..."
docfx build

# Check if build was successful
if [ $? -ne 0 ]; then
    echo "DocFX build failed!"
    exit 1
fi

echo "Documentation built successfully!"
echo "Output directory: _site"

# Serve the documentation if requested
if [ "$serve" = true ]; then
    echo "Starting documentation server at http://localhost:8080"
    echo "Press Ctrl+C to stop the server"
    docfx serve _site
fi