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
    }
}
