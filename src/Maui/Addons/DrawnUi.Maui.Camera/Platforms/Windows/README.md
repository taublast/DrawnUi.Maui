# SkiaCamera Windows Implementation

### Key Components

1. **SkiaCamera.Windows.cs**
   - Platform-specific partial class implementation
   - Handles zoom, flash, metadata creation
   - Manages native camera lifecycle

2. **NativeCamera.cs**
   - Implements `INativeCamera` interface
   - Uses Windows MediaCapture APIs
   - Handles frame processing and conversion to SkiaSharp

## Features Implemented

### ✅ Core Functionality
- [x] Camera preview with real-time frames
- [x] Front/back camera switching
- [x] Photo capture (JPEG format)
- [x] Flash control (if supported by hardware)
- [x] Zoom control (hardware zoom when available)
- [x] Device orientation handling
- [x] Permission management
- [x] Image saving to Pictures library

### ✅ DrawnUi Integration
- [x] SKImage conversion for SkiaSharp rendering
- [x] Event handling (CaptureSuccess, CaptureFailed, Zoomed)
- [x] Proper disposal and resource management
- [x] Thread-safe preview frame updates

### ✅ Windows-Specific Features
- [x] MediaFrameReader for efficient frame processing
- [x] Automatic camera device detection
- [x] Windows Storage API integration
- [x] Safe managed memory access (no unsafe code)

## Technical Implementation Details

### Frame Processing Pipeline

1. **MediaFrameReader** captures frames from camera
2. **SoftwareBitmap** is converted to **WriteableBitmap** (safe managed approach)
3. **PixelBuffer** data is accessed via managed stream
4. **SKImage** is created from pixel data using SkiaSharp
5. **CapturedImage** wrapper is created and passed to DrawnUi
 
 

