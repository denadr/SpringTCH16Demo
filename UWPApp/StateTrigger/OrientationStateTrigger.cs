using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UWPApp.StateTrigger
{
    public class OrientationStateTrigger : StateTriggerBase
    {
        public ApplicationViewOrientation Orientation { get; set; }

        public OrientationStateTrigger()
        {
            Window.Current.SizeChanged += (s, e) =>
                // Activates the trigger if the current view orientation equals the requested orientation,
                // each time the size of the application view changes.
                SetActive(ApplicationView.GetForCurrentView().Orientation.Equals(Orientation));
        }
    }
}
