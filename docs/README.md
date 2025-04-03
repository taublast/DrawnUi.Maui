# DrawnUi Documentation

This folder contains the DocFX-based documentation for DrawnUi.

## Building the Documentation

### Option 1: Using .NET Tool (Recommended)

To build the documentation locally:

1. Install DocFX as a .NET global tool:
   ```
   dotnet tool install -g docfx
   ```

2. Navigate to the docs folder:
   ```
   cd docs
   ```

3. Build the documentation:
   ```
   docfx build
   ```

4. Preview the documentation:
   ```
   docfx serve _site
   ```

### Option 2: Using Docker

If you don't have .NET installed, you can use Docker:

```bash
# From the repository root
docker run --rm -it -v ${PWD}:/app -w /app/docs mcr.microsoft.com/dotnet/sdk:7.0 bash -c "dotnet tool install -g docfx && docfx build"
```

### Option 3: Using NPM Package (Alternative)

For environments where .NET isn't available:

1. Install docfx via npm:
   ```
   npm install -g @tsgkadot/docfx-flavored-markdown
   ```

2. Build the documentation:
   ```
   dfm build
   ```

## Documentation Structure

- `/api/`: Auto-generated API documentation from XML comments
- `/articles/`: Conceptual documentation articles and tutorials
- `/images/`: Images used in the documentation
- `/templates/`: DocFX templates for styling

## Contributing to the Documentation

When contributing to the documentation:

1. For API documentation, add XML comments to the code in the DrawnUi source files
2. For conceptual documentation, edit or create Markdown files in the `/articles/` folder
3. After making changes, build the documentation to verify it renders correctly

## API Documentation Guidelines

When adding XML comments to your code:

- Use the `<summary>` tag to provide a brief description of the class/method/property
- Use the `<param>` tag to document parameters
- Use the `<returns>` tag to document return values
- Use the `<example>` tag to provide usage examples
- Use `<see cref="..."/>` to create links to other types/members