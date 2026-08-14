using com.ktgame.core;
using com.ktgame.core.editor;
using Sirenix.OdinInspector;

namespace com.ktgame.services.ads.editor
{
    public class RevenueEditor
    {
        private KTSettingSO _setting;
        private RevenueAdSetting _revenueSetting;

        [Title("Analytics Configuration", "Manage your analytics SDKs and ad revenue tracking.", TitleAlignments.Centered)]
        [InfoBox("Select your analytics providers. Revenue tracking requires configuring the supported ad formats for each provider.", InfoMessageType.Info)]
        [PropertyOrder(-10)]
        [ShowInInspector, HideLabel, DisplayAsString(false)]
        private string _header = "";

        public RevenueEditor(KTSettingSO setting, RevenueAdSetting revenueSetting)
        {
            _setting = setting;
            _revenueSetting = revenueSetting;
            FirebaseAndroid = revenueSetting;
        }
        
        [BoxGroup("Analytics Strategy", CenterLabel = true)]
        [PropertyOrder(0)]
        [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
        [LabelText("Active SDKs"), LabelWidth(150), ShowInInspector, EnumToggleButtons]
        public AnalyticsProvider AnalyticsSDKProvider
        {
            get => _setting.AnalyticsProvider;
            set
            {
                if (((_setting.AnalyticsProvider & AnalyticsProvider.AppsFlyer) != 0) && (value & AnalyticsProvider.Adjust) != 0)
                {
                    _setting.AnalyticsProvider = value;
                    _setting.AnalyticsProvider &= ~AnalyticsProvider.AppsFlyer;
                }
                else if (((_setting.AnalyticsProvider & AnalyticsProvider.Adjust) != 0) && (value & AnalyticsProvider.AppsFlyer) != 0)
                {
                    _setting.AnalyticsProvider = value;
                    _setting.AnalyticsProvider &= ~AnalyticsProvider.Adjust;
                }
                else
                {
                    _setting.AnalyticsProvider = value;
                }
                
                if (_revenueSetting != null)
                {
                    _revenueSetting.ActiveProviders = _setting.AnalyticsProvider;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(_revenueSetting);
#endif
                }
                
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(_setting);
#endif
            }
        }
        
        [BoxGroup("Ad Revenue Tracking", CenterLabel = true)]
        [PropertyOrder(10)]
        [TabGroup("Ad Revenue Tracking/Platform", "Android", SdfIconType.Robot)]
        [ShowInInspector, HideReferenceObjectPicker, HideLabel, Indent(1)]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        public RevenueAdSetting FirebaseAndroid;
        
        private void ForceProvider(AnalyticsProvider provider)
        {
            if (_revenueSetting == null) return;
            if (_revenueSetting.Providers == null) return;

            foreach (var p in _revenueSetting.Providers)
            {
                if (p != null && p.Provider != provider)
                {
                    p.Provider = provider;
                }
            }
        }
    }
}
