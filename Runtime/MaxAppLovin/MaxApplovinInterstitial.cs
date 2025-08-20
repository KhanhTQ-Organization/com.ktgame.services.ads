using System;
using System.Collections.Generic;

#if APPSFLYER_ANALYTICS
using AppsFlyerSDK;
#endif

using com.ktgame.ads.core;

namespace com.ktgame.ads.max_applovin
{
    public class MaxApplovinInterstitial : IInterstitialAdapter
    {
        protected string UnitId { private set; get; }
        public event Action OnLoadSucceeded;
        public event Action<AdError> OnLoadFailed;
        public event Action<AdError> OnShowFailed;
        public event Action<AdPlacement> OnShowSucceeded;
        public event Action<AdPlacement> OnClicked;
        public event Action OnClosed;
        public event Action<ImpressionData> OnImpressionSuccess;
#if MAX_APPLOVIN
        public bool IsReady => MaxSdk.IsInterstitialReady(UnitId);
#else
        public bool IsReady => true;
#endif
        protected AdPlacement AdPlacement { private set; get; }

        public MaxApplovinInterstitial(string unitId)
        {
            UnitId = unitId;
#if MAX_APPLOVIN
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += LoadSucceededHandler;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += LoadFailedHandler;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += ShowFailedHandler;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += ClosedHandler;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += ClickedHandler;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += AdRevenuePaidHandler;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += ShowSucceededHandler;
#endif
        }

        public void Load()
        {
#if MAX_APPLOVIN
            MaxSdk.LoadInterstitial(UnitId);
#endif
        }

        public void Show(AdPlacement placement)
        {
            AdPlacement = placement;
#if MAX_APPLOVIN
            MaxSdk.ShowInterstitial(UnitId, placement.Location);
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
#if APPSFLYER_ANALYTICS
            AppsFlyer.sendEvent("af_inters_load_fail  ", null);
#endif
        }

        private void ShowSucceededHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnShowSucceeded?.Invoke(AdPlacement);
#if APPSFLYER_ANALYTICS
            AppsFlyer.sendEvent("af_inters_displayed ", null);
#endif
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
            var impressionData = adInfo.ToImpressionData(AdFormat.Interstitial);
            OnImpressionSuccess?.Invoke(impressionData);
#if APPSFLYER_ANALYTICS
            Dictionary<string, string> additionalParams = new Dictionary<string, string>
            {
                { AdRevenueScheme.COUNTRY, MaxSdk.GetSdkConfiguration().CountryCode },
                { AdRevenueScheme.AD_UNIT, adInfo.AdUnitIdentifier },
                { AdRevenueScheme.AD_TYPE, adInfo.AdFormat },
                { AdRevenueScheme.PLACEMENT, adInfo.Placement }
            };
            var logRevenue = new AFAdRevenueData(adInfo.NetworkName, MediationNetwork.ApplovinMax, "USD", adInfo.Revenue);
            AppsFlyer.logAdRevenue(logRevenue, additionalParams);
#endif
        }
#endif
    }
}