namespace AppoMobi.Touch
{
    public static class Settings
    {
 
 
        /// <summary>
        /// The minimum DeltaAngle before a Rotating event is raised.
        /// The default value is 0.5. Set it to a higher value to get fewer events or a lower value to be more accurate.
        /// </summary>
        public static double MinimumDeltaAngle
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum distance a finger has to be moved before a Panning event is raised.
        /// The default value is 2. Set it to a higher value to get fewer events or a lower value to be more accurate.
        /// </summary>
        public static double MinimumDeltaDistance
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum change in Scale before a Pinching event is raised.
        /// The default value is 0.01 which means that it has to change by at least 1%. Set it to a higher value to get fewer events or a lower value to be more accurate.
        /// </summary>
        public static double MinimumDeltaScale
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum distance which two coordinates must be apart in order to calculate a DeltaScale*.
        /// The default value is 10. If DistanceX/Y is lower than that, then DeltaScaleX/Y will be 1.
        /// </summary>
        public static double MinimumDistanceForScale
        {
            get;
            set;
        }

        /// <summary>
        /// Milliseconds after a Tapping event that will be waited until Tapped is raised. If a second Tapping comes within this timeframe, then DoubleTapped will be raised instead.
        /// If more than two taps come with less then this timeframe between them, neither Tapped nor DoubleTapped will be raised.
        /// It will only be indicated in TapEventArgs.NumberOfTaps in the Tapping event.
        /// </summary>
        public static int MsUntilTapped
        {
            get;
            set;
        }

        static Settings()
        {
 
            Settings.MsUntilTapped = 250;
            Settings.MinimumDeltaDistance = 2;
            Settings.MinimumDeltaScale = 0.01;
            Settings.MinimumDeltaAngle = 0.5;
            Settings.MinimumDistanceForScale = 10;
        }
    }
}