using Windows.Graphics.Display;
using Windows.Media.Capture;

namespace UWPApp.Extensions
{
    public static class VideoRotationUtilities
    {
        public static VideoRotation FromDisplayOrientation(DisplayOrientations orientation, bool mirrored = false)
        {
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                    return mirrored ? VideoRotation.Clockwise270Degrees : VideoRotation.Clockwise90Degrees;

                case DisplayOrientations.LandscapeFlipped:
                    return VideoRotation.Clockwise180Degrees;

                case DisplayOrientations.PortraitFlipped:
                    return mirrored ? VideoRotation.Clockwise90Degrees : VideoRotation.Clockwise270Degrees;

                default: // Including DisplayOrientations.Landscape
                    return VideoRotation.None;
            }
        }
    }
}
