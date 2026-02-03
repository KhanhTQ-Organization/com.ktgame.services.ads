using com.ktgame.core;
using Sirenix.OdinInspector;
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
        
        [Title("Build Setting", Bold = true, HorizontalLine = true)]
        [ShowInInspector]
        public bool DebugMode
        {
            get => IAASettingSO.Instance.IsDebugMode;
            set => IAASettingSO.Instance.IsDebugMode = value;
        }

        [ShowInInspector]
        public bool UseLocalConfig
        {
            get => IAASettingSO.Instance.UseLocalConfig;
            set => IAASettingSO.Instance.UseLocalConfig = value;
        }


        [ShowInInspector]
        public bool DebugLog
        {
            get => IAASettingSO.Instance.IsDebugLog;
            set => IAASettingSO.Instance.IsDebugLog = value;
        }
    }
}
