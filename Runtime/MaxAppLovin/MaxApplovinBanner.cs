using System;
using UnityEngine;
using com.ktgame.ads.core;

namespace com.ktgame.ads.max_applovin
{
    public class MaxApplovinBanner : IBannerAdapter
    {
        protected string UnitId { private set; get; }
        public event Action<AdError> OnLoadFailed;
        public event Action<AdPlacement> OnLoadSucceeded;
        public event Action<ImpressionData> OnImpressionSuccess;

        protected AdPlacement AdPlacement { private set; get; }
        protected BannerSize AdSize { private set; get; }
        protected BannerPosition AdPosition { private set; get; }

        public MaxApplovinBanner(string unitId, BannerSize bannerSize, BannerPosition bannerPosition)
        {
            UnitId = unitId;
            AdSize = bannerSize;
            AdPosition = bannerPosition;
            AdPlacement = new AdPlacement("Banner");
#if MAX_APPLOVIN
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += LoadSucceedHandler;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += LoadFailedHandler;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += AdRevenuePaidHandler;
#endif
        }

        public void Load()
        {
#if MAX_APPLOVIN    
            MaxSdk.CreateBanner(UnitId, AdPosition.ToMaxApplovinBannerPosition());
            MaxSdk.SetBannerBackgroundColor(UnitId, Color.black);
            MaxSdk.SetBannerPlacement(UnitId, AdPlacement.Location);
#endif
        }

        public void SetBackgroundColor(Color color)
        {
#if MAX_APPLOVIN    
            MaxSdk.SetBannerBackgroundColor(UnitId, color);
#endif
        }

        public void Show()
        {
#if MAX_APPLOVIN    
			MaxSdk.UpdateBannerPosition(UnitId, AdPosition.ToMaxApplovinBannerPosition());
            MaxSdk.ShowBanner(UnitId);
#endif
        }

        public void Hide()
        {
#if MAX_APPLOVIN    
            MaxSdk.HideBanner(UnitId);
#endif
        }

        public void Destroy()
        {
#if MAX_APPLOVIN    
            MaxSdk.DestroyBanner(UnitId);
#endif
        }

#if MAX_APPLOVIN    
        private void AdRevenuePaidHandler(string adUnitIdentifier, MaxSdkBase.AdInfo adInfo)
        {
            var impressionData = adInfo.ToImpressionData(AdFormat.Banner);
            OnImpressionSuccess?.Invoke(impressionData);
        }

        private void LoadFailedHandler(string adUnitIdentifier, MaxSdkBase.ErrorInfo errorInfo)
        {
            OnLoadFailed?.Invoke(errorInfo.Code.ToErrorCode(AdPlacement));
        }

        private void LoadSucceedHandler(string adUnitIdentifier, MaxSdkBase.AdInfo adInfo)
        {
            OnLoadSucceeded?.Invoke(AdPlacement);
        }
#endif
    }
}