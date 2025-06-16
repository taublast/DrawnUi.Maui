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
        /// Edge-preserving rescaling optimized for sharp graphics and pixel-perfect content, RescalingQuality not used here: "I'm working with pixel art/UI graphics".
        /// Best for: Pixel art, very small icons (16x16, 32x32), screenshots, text graphics.
        /// </summary>
        EdgePreserving,

        /// <summary>
        /// Multi-pass progressive rescaling for superior quality on significant size changes: "I want good quality".
        /// Best for: Icons, logos, transparent graphics with sharp edges, large size reductions (2x smaller).
        /// </summary>
        MultiPass,
    }
}
