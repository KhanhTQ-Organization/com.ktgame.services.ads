using System;
using System.Collections.Generic;

#if APPSFLYER_ANALYTICS
using AppsFlyerSDK;
#endif

using com.ktgame.ads.core;
using UnityEngine;

namespace com.ktgame.ads.max_applovin
{
    public class MaxApplovinRewardVideo : IRewardVideoAdapter
    {
        protected string UnitId { private set; get; }
        public event Action OnLoadSucceeded;
        public event Action<AdError> OnLoadFailed;
        public event Action<AdError> OnShowFailed;
        public event Action OnVideoOpened;
        public event Action OnVideoClosed;
        public event Action OnVideoClicked;
        public event Action<AdPlacement> OnRewarded;
        public event Action<ImpressionData> OnImpressionSuccess;

#if MAX_APPLOVIN
        public bool IsReady => MaxSdk.IsRewardedAdReady(UnitId);
#else
        public bool IsReady => true;
#endif
        protected AdPlacement AdPlacement { private set; get; }

        public MaxApplovinRewardVideo(string unitId)
        {
            UnitId = unitId;
#if MAX_APPLOVIN
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += LoadSucceededHandler;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += LoadFailedHandler;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += ShowFailedHandler;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += ShowSucceededHandler;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += ClickedHandler;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += ClosedHandler;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += ReceivedRewardHandler;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += RevenuePaidHandler;
#endif
        }

        public void Load()
        {
#if MAX_APPLOVIN
            MaxSdk.LoadRewardedAd(UnitId);
#endif
        }

        public void Show(AdPlacement adPlacement)
        {
            AdPlacement = adPlacement;
#if MAX_APPLOVIN
            MaxSdk.ShowRewardedAd(UnitId, AdPlacement.Location);
#endif
        }
        
#if MAX_APPLOVIN
        private void RevenuePaidHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            var impressionData = adInfo.ToImpressionData(AdFormat.RewardedVideo);
            OnImpressionSuccess?.Invoke(impressionData);
        }

        private void LoadSucceededHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnLoadSucceeded?.Invoke();
        }

        private void LoadFailedHandler(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            var adError = errorInfo.Code.ToErrorCode(AdPlacement);
            OnLoadFailed?.Invoke(adError);
        }

        private void ShowFailedHandler(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            var adError = errorInfo.Code.ToErrorCode(AdPlacement);
            OnShowFailed?.Invoke(adError);
        }

        private void ShowSucceededHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnVideoOpened?.Invoke();
        }

        private void ClickedHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnVideoClicked?.Invoke();
        }

        private void ClosedHandler(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnVideoClosed?.Invoke();
        }

        private void ReceivedRewardHandler(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            OnRewarded?.Invoke(AdPlacement);
        }
#endif
    }
}