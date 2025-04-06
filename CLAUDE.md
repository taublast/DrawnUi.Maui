# DrawnUi Development Guidelines

## Build Commands
- Build solution: `dotnet build DrawnUi.sln`
- Run tests: `dotnet test src/test/UnitTests/UnitTests.csproj`
- Run single test: `dotnet test src/test/UnitTests/UnitTests.csproj --filter "FullyQualifiedName=UnitTests.TestClassName.TestMethodName"`
- Clean bin/obj folders: `pwsh src/DeleteBinObj.ps1`

## Code Style Guidelines
- Use C# 9+ features and .NET MAUI conventions
- Prefix Skia-based controls with "Skia" (e.g., SkiaLabel, SkiaButton)
- Follow naming conventions: PascalCase for public members, camelCase for private/internal
- Implement proper interfaces for control behaviors (ISkiaGestureListener, ISkiaCell, etc.)
- Place file resources in MAUI app Resources/Raw folder
- For platform-specific code, use partial classes with platform suffixes (.Android.cs, .iOS.cs)
- Use nullable reference types appropriately
- Cache UI elements properly according to their nature (Operations for text/SVG, Image for static content, GPU for optimized rendering)
- Always document public APIs with XML comments
- Properly handle disposable resources with using statements or IDisposable implementation
- Methods that could be reused in sub-classed controls must be virtual.


## Version Information
- .NET 9.0 with SkiaSharp 3 for latest development
- Configuration in src/Directory.Build.props controls SkiaSharp version (v2 vs v3)
- We do not write new v2 specific code anymore, v2 goes obsolete.