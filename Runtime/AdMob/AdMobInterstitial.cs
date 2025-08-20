using System;
using System.Collections.Generic;

#if APPSFLYER_ANALYTICS
using AppsFlyerSDK;
#endif

using com.ktgame.ads.core;
#if ADMOB
using GoogleMobileAds.Api;
#endif
using AdError = com.ktgame.ads.core.AdError;
using AdFormat = com.ktgame.ads.core.AdFormat;

namespace com.ktgame.ads.admob
{
	public class AdMobInterstitial : IInterstitialAdapter
	{
		public event Action<AdError> OnLoadFailed;
		public event Action OnLoadSucceeded;
		public event Action<AdError> OnShowFailed;
		public event Action<AdPlacement> OnShowSucceeded;
		public event Action<AdPlacement> OnClicked;
		public event Action OnClosed;
		public event Action<ImpressionData> OnImpressionSuccess;
#if ADMOB
		public bool IsReady => InterstitialAd != null && InterstitialAd.CanShowAd();
#else       
		public bool IsReady => true;
#endif

		protected string UnitId { private set; get; }
		protected AdPlacement AdPlacement { private set; get; }
#if ADMOB
		protected InterstitialAd InterstitialAd { private set; get; }
#endif
		
		public AdMobInterstitial(string unitId)
		{
			UnitId = unitId;
			AdPlacement = new AdPlacement("AppOpen");
		}
		
		public void Load()
		{
#if ADMOB
			AdRequest request = new AdRequest();
			InterstitialAd.Load(UnitId, request, (ad, error) =>
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
				
				InterstitialAd = ad;
				OnLoadSucceeded?.Invoke();
				
				InterstitialAd.OnAdImpressionRecorded += ImpressionSuccessHandler;
				InterstitialAd.OnAdClicked += ClickedHandler;
				InterstitialAd.OnAdFullScreenContentOpened += ShowSucceededHandler;
				InterstitialAd.OnAdFullScreenContentClosed += ClosedHandler;
				InterstitialAd.OnAdFullScreenContentFailed += ShowFailedHandler;
				InterstitialAd.OnAdPaid += AdRevenuePaidHandler;
			});
#endif
		}
		public void Show(AdPlacement adPlacement)
		{
			AdPlacement = adPlacement;
#if ADMOB
			InterstitialAd.Show();
#endif
		}
		
#if ADMOB
		private void AdRevenuePaidHandler(AdValue adValue)
		{
			var impressionData = adValue.ToImpressionData(UnitId, AdFormat.AppOpen);
			OnImpressionSuccess?.Invoke(impressionData);
#if APPSFLYER_ANALYTICS	
			double revenue = adValue.Value / 1000000d;
			var adapterResponseInfo = InterstitialAd.GetResponseInfo().GetLoadedAdapterResponseInfo();
			var mediationGroupName = InterstitialAd.GetResponseInfo().GetResponseExtras()["mediation_group_name"];
			
			Dictionary<string, string> additionalParams = new Dictionary<string, string>
			{
				{ "ad_platform", "admob" },
				{ "ad_source", adapterResponseInfo.AdSourceInstanceName },
				{ "ad_unit_name", InterstitialAd.GetAdUnitID() },
				{ "ad_format", "interstitial" },
				{ "placement", InterstitialAd.GetAdUnitID() },
				{ "value", revenue.ToString() },
				{ "currency", "USD" },
			};
			var logRevenue = new AFAdRevenueData(mediationGroupName,MediationNetwork.GoogleAdMob, "USD", revenue);
			AppsFlyer.logAdRevenue(logRevenue, additionalParams);
#endif
		}

		private void ImpressionSuccessHandler()
		{
			OnImpressionSuccess?.Invoke(new ImpressionData(AdPlatform.Admob, "", UnitId, AdFormat.Interstitial, "Interstitial", "USD", 0));
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