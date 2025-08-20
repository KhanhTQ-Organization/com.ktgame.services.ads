using System;
using com.ktgame.ads.core;

namespace com.ktgame.ads.max_applovin
{
    public class MaxApplovinAppOpen : IAppOpenAdapter
    {
        protected string UnitId { private set; get; }
        public event Action<AdError> OnLoadFailed;
        public event Action OnLoadSucceeded;
        public event Action<AdError> OnShowFailed;
        public event Action<AdPlacement> OnShowSucceeded;
        public event Action<AdPlacement> OnClicked;
        public event Action OnClosed;
        public event Action<ImpressionData> OnImpressionSuccess;
        public event Action<AppState> OnAppStateChanged;
        
#if MAX_APPLOVIN
        public bool IsReady => MaxSdk.IsAppOpenAdReady(UnitId);
#else
        public bool IsReady => true;
#endif

        private AdPlacement AdPlacement { get; }

        public MaxApplovinAppOpen(string unitId)
        {
            UnitId = unitId;
            AdPlacement = new AdPlacement("AppOpen");
#if MAX_APPLOVIN
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += LoadSucceededHandler;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += LoadFailedHandler;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += ShowSucceededHandler;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += ShowFailedHandler;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += ClickedHandler;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += AdRevenuePaidHandler;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += ClosedHandler;
#endif
        }

        public void Load()
        {
#if MAX_APPLOVIN
            //MaxSdkUnityEditor.LoadAppOpenAd(UnitId);
#endif
        }

        public void Show()
        {
#if MAX_APPLOVIN
            MaxSdk.ShowAppOpenAd(UnitId);
#endif
        }
        
#if MAX_APPLOVIN
        private void LoadSucceededHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnLoadSucceeded?.Invoke();
        }

        private void LoadFailedHandler(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            var adError = errorInfo.Code.ToErrorCode(AdPlacement);
            OnLoadFailed?.Invoke(adError);
        }

        private void ShowSucceededHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnShowSucceeded?.Invoke(AdPlacement);
        }

        private void ShowFailedHandler(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            var adError = errorInfo.Code.ToErrorCode(AdPlacement);
            OnShowFailed?.Invoke(adError);
        }

        private void ClickedHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnClicked?.Invoke(AdPlacement);
        }

        private void ClosedHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnClosed?.Invoke();
        }

        private void AdRevenuePaidHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var impressionData = adInfo.ToImpressionData(AdFormat.AppOpen);
            OnImpressionSuccess?.Invoke(impressionData);
        }
#endif
    }
}