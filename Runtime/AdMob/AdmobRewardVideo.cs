using System;
using System.Collections.Generic;
using com.ktgame.ads.admob;
using com.ktgame.ads.core;
#if ADMOB
using GoogleMobileAds.Api;
using AppsFlyerSDK;
#endif
using AdError = com.ktgame.ads.core.AdError;
using AdFormat = com.ktgame.ads.core.AdFormat;

namespace com.ktgame.ads.max_applovin
{
	public class AdmobRewardVideo : IRewardVideoAdapter
	{
		public event Action OnLoadSucceeded;
		public event Action<AdError> OnLoadFailed;
		public event Action<AdError> OnShowFailed;
		public event Action OnVideoOpened;
		public event Action OnVideoClosed;
		public event Action OnVideoClicked;
		public event Action<AdPlacement> OnRewarded;
		public event Action<ImpressionData> OnImpressionSuccess;
		public event Action<ImpressionData> OnPain;

#if ADMOB
		public bool IsReady => RewardedAd != null && RewardedAd.CanShowAd();
#else
        public bool IsReady => true;
#endif
		
		protected string UnitId { private set; get; }
		protected AdPlacement AdPlacement { private set; get; }
#if ADMOB
		protected RewardedAd RewardedAd { private set; get; }
#endif

		public AdmobRewardVideo(string unitId)
		{
			UnitId = unitId;
			AdPlacement = new AdPlacement("RewardVideo");
		}

		public void Load()
		{
#if ADMOB
			AdRequest request = new AdRequest();
			RewardedAd.Load(UnitId, request, (ad, error) =>
			{
				if (error != null)
				{
					var adError = AdMobExtensions.ToAdError(error, AdPlacement);
					OnLoadFailed?.Invoke(adError);
					return;
				}

				if (ad == null)
				{
					OnLoadFailed?.Invoke(new AdError(AdPlacement, AdErrorCode.NoFill, "No ads are currently eligible for your device."));
					return;
				}
				
				RewardedAd = ad;
				OnLoadSucceeded?.Invoke();
				
				RewardedAd.OnAdImpressionRecorded += ImpressionSuccessHandler;
				RewardedAd.OnAdClicked += ClickedHandler;
				RewardedAd.OnAdFullScreenContentOpened += ShowSucceededHandler;
				RewardedAd.OnAdFullScreenContentClosed += ClosedHandler;
				RewardedAd.OnAdFullScreenContentFailed += ShowFailedHandler;
				RewardedAd.OnAdPaid += AdRevenuePaidHandler;
			});
#endif
		}
		
		private void ImpressionSuccessHandler()
		{
			OnImpressionSuccess?.Invoke(new ImpressionData(AdPlatform.Admob, "", UnitId, AdFormat.RewardedVideo, "Rewards", "USD", 0));
		}

		public void Show(AdPlacement adPlacement)
		{
			AdPlacement = adPlacement;
#if ADMOB
			RewardedAd.Show(OnShowRewarded);
#endif
		}

#if ADMOB
		

		private void OnShowRewarded(Reward obj)
		{
			
		}

		private void AdRevenuePaidHandler(AdValue adValue)
		{
			var impressionData = adValue.ToImpressionData(UnitId, AdFormat.RewardedVideo);
			OnPain?.Invoke(impressionData);
		}
		
		private void ShowFailedHandler(GoogleMobileAds.Api.AdError error)
		{
			var adError = AdMobExtensions.ToAdError(error, AdPlacement);
			OnShowFailed?.Invoke(adError);
		}

		private void ShowSucceededHandler()
		{
			OnVideoOpened?.Invoke();
		}

		private void ClickedHandler()
		{
			OnVideoClicked?.Invoke();
		}

		private void ClosedHandler()
		{
			OnVideoClosed?.Invoke();
		}
#endif
	}
}
