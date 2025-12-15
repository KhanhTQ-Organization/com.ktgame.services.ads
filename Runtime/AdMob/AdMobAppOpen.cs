using System;
using com.ktgame.ads.core;
using UnityEngine;

#if ADMOB
using GoogleMobileAds.Api;
#endif

using AdError = com.ktgame.ads.core.AdError;
using AdFormat = com.ktgame.ads.core.AdFormat;

namespace com.ktgame.ads.admob
{
    public class AdMobAppOpen : IAppOpenAdapter
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
        private AdPlacement AdPlacement { get; }
        
#if ADMOB
        protected AppOpenAd AppOpenAd { private set; get; }
#endif
        
#if ADMOB
        public bool IsReady => AppOpenAd != null;
#else
        public bool IsReady => true;
#endif
        
        public AdMobAppOpen(string unitId)
        {
            UnitId = unitId;
            AdPlacement = new AdPlacement("AppOpen");
        }
        
        public void Load()
        {
#if ADMOB
            AdRequest request = new AdRequest();
            AppOpenAd.Load(UnitId, request, (ad, error) =>
            {
                if (error != null)
                {
                    var adError = AdMobExtensions.ToAdError(error, AdPlacement);
                    //OnLoadFailed?.Invoke(adError);
                    return;
                }

                AppOpenAd = ad;
                OnLoadSucceeded?.Invoke();
                AppOpenAd.OnAdImpressionRecorded += ImpressionSuccessHandler;
                AppOpenAd.OnAdClicked += ClickedHandler;
                AppOpenAd.OnAdFullScreenContentOpened += ShowSucceededHandler;
                AppOpenAd.OnAdFullScreenContentClosed += ClosedHandler;
                AppOpenAd.OnAdFullScreenContentFailed += ShowFailedHandler;
                AppOpenAd.OnAdPaid += AdRevenuePaidHandler;
            });
#endif
        }

        public void Show()
        {
            if (IsReady)
            {
#if ADMOB
                AppOpenAd.Show();
#endif
            }
            else
            {
                OnShowFailed?.Invoke(new AdError(AdPlacement,AdErrorCode.NoAdToShow,"An error occurred while trying to show the ad."));
            }
        }
        
#if ADMOB
        private void AdRevenuePaidHandler(AdValue adValue)
        {
            var impressionData = adValue.ToImpressionData(UnitId, AdFormat.AppOpen);
            OnImpressionSuccess?.Invoke(impressionData);
        }

        private void ImpressionSuccessHandler()
        {
          OnImpressionSuccess?.Invoke(new ImpressionData(AdPlatform.Admob, "unknown", UnitId, AdFormat.AppOpen, "AppOpen", "USD", 0));
        }

        private void ClickedHandler()
        {
            OnClicked?.Invoke(AdPlacement);
        }

        private void ShowSucceededHandler()
        {
            OnShowSucceeded?.Invoke(AdPlacement);
        }

        private void ClosedHandler()
        {
            OnClosed?.Invoke();
        }

        private void ShowFailedHandler(GoogleMobileAds.Api.AdError adError)
        {
            var error = AdMobExtensions.ToAdError(adError, AdPlacement);
            OnShowFailed?.Invoke(error);
        }
#endif
    }
}