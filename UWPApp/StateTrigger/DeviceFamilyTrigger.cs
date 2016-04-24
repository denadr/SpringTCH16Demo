using Windows.System.Profile;
using Windows.UI.Xaml;

namespace UWPApp.StateTrigger
{
    public class DeviceFamilyTrigger : StateTriggerBase
    {
        public string Family
        {
            set
            { // Activates the trigger if the device family equals the requested value.
                SetActive(AnalyticsInfo.VersionInfo.DeviceFamily.Equals(value));
            }
        }
    }
}
