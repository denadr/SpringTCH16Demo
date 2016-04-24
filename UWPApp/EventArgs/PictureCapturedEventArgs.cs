using Windows.Storage.Streams;

namespace UWPApp.EventArgs
{
    public class PictureCapturedEventArgs : System.EventArgs
    {
        public IRandomAccessStream Stream { get; private set; }

        public PictureCapturedEventArgs(IRandomAccessStream stream)
        {
            Stream = stream;
        }
    }
}
