#!/bin/bash

# Script to build and serve the documentation using Docker

# Build documentation image
echo "Building documentation Docker image..."
docker build -t drawnui-maui-docs .

# Check if we should serve the docs
if [ "$1" = "--serve" ]; then
    echo "Starting documentation server at http://localhost:8080"
    echo "Press Ctrl+C to stop the server"
    docker run --rm -it -p 8080:8080 drawnui-maui-docs
else
    # Just build the docs
    echo "Building documentation..."
    docker run --rm -it -v $(pwd)/_site:/docs/_site drawnui-maui-docs docfx build
    echo "Documentation built successfully!"
    echo "Output directory: _site"
    echo "Run with --serve to start a documentation server"
fi