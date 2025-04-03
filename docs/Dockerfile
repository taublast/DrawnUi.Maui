# Use the .NET SDK image 
FROM mcr.microsoft.com/dotnet/sdk:7.0

# Install DocFX
RUN dotnet tool install -g docfx

# Set the working directory
WORKDIR /docs

# Copy the documentation files 
COPY . .

# Build the documentation
RUN docfx build

# Expose port for serving
EXPOSE 8080

# Default command - serve the generated documentation
ENTRYPOINT ["docfx", "serve", "_site"]