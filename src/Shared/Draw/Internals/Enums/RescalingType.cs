namespace DrawnUi.Draw
{
    /// <summary>
    /// Rescaling method types for different image processing approaches
    /// </summary>
    public enum RescalingType
    {
        /// <summary>
        /// Standard SkiaSharp rescaling:  "I just need it to work fast".
        /// Best for: Minor size adjustments, performance-focused.
        /// </summary>
        Default,

        /// <summary>
        /// Multi-pass progressive rescaling for superior quality on significant size changes: "I want good quality".
        /// Best for: Icons, logos, transparent graphics with sharp edges, large size reductions (2x smaller).
        /// </summary>
        MultiPass,

        /// <summary>
        /// Gamma-corrected rescaling that processes in linear color space for photographic quality: "I'm working with photos professionally".
        /// Best for: Professional photography and color-critical work, color accuracy, gradients and smooth color transitions.
        /// Note: Slower than other methods.
        /// </summary>
        GammaCorrection,

        /// <summary>
        /// Edge-preserving rescaling optimized for sharp graphics and pixel-perfect content, RescalingQuality not used here: "I'm working with pixel art/UI graphics".
        /// Best for: Pixel art, very small icons (16x16, 32x32), screenshots, text graphics.
        /// </summary>
        EdgePreserving,
    }
}
