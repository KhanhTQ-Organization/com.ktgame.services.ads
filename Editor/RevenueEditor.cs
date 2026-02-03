using com.ktgame.core;
using com.ktgame.core.editor;
using Sirenix.OdinInspector;

namespace com.ktgame.services.ads.editor
{
    public class RevenueEditor
    {
        private KTSettingSO _setting;
        private RevenueAdSetting _revenueSetting;

        public RevenueEditor(KTSettingSO setting, RevenueAdSetting revenueSetting)
        {
            _setting = setting;
            _revenueSetting = revenueSetting;
        }
        
        [PropertySpace(20)]
        [Title("Analytics Setting", Bold = true)]
        [LabelText("Analytics SDK"), LabelWidth(150), ShowInInspector, EnumToggleButtons]
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
            }
        }
        
        [ShowIf("@(AnalyticsSDKProvider.HasFlag(AnalyticsProvider.Firebase))"),
         TabGroup("Platform", "Android", SdfIconType.Robot),
         ShowInInspector, HideReferenceObjectPicker,
         BoxGroup("Platform/Android/Firebase Revenue"),
         InlineEditor(Expanded = true),
         HideLabel, Indent(1)]
        public RevenueAdSetting FirebaseAndroid
        {
            get => _revenueSetting;
            set => _revenueSetting = value;
        }
        
        [ShowIf("@(AnalyticsSDKProvider.HasFlag(AnalyticsProvider.Adjust))"),
         TabGroup("Platform", "Android", SdfIconType.Robot),
         ShowInInspector, HideReferenceObjectPicker,
         BoxGroup("Platform/Android/Adjust Revenue"),
         InlineEditor(Expanded = true),
         HideLabel, Indent(1)]
        public RevenueAdSetting AdjustAndroid
        {
            get => _revenueSetting;
            set => _revenueSetting = value;
        }
        
        [ShowIf("@(AnalyticsSDKProvider.HasFlag(AnalyticsProvider.AppsFlyer))"),
         TabGroup("Platform", "Android", SdfIconType.Robot),
         ShowInInspector, HideReferenceObjectPicker,
         BoxGroup("Platform/Android/AppsFlyer Revenue"),
         InlineEditor(Expanded = true),
         HideLabel, Indent(1)]
        public RevenueAdSetting AppsFlyerAndroid
        {
            get => _revenueSetting;
            set => _revenueSetting = value;
        }
        
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
