using UnityEditor;

namespace com.ktgame.services.ads.editor
{
    public class PackageInstaller
    {
        [MenuItem("Ktgame/Services/Settings/Ads")]
        private static void SelectionSettings()
        {
            Selection.activeObject = AdServiceSettings.Instance;
        }
        
        [MenuItem("Ktgame/Services/Settings/Revenue")]
        private static void SelectionSettingsTrackingFirebase()
        {
            Selection.activeObject = RevenueAdSetting.Instance;
        }
    }
}
