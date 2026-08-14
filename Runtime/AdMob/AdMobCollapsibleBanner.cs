using System;
using com.ktgame.ads.core;
using UnityEngine;

#if ADMOB
using GoogleMobileAds.Api;
#endif

namespace com.ktgame.ads.admob
{
	public class AdMobCollapsibleBanner : IBannerAdapter
	{
		public event Action<AdError> OnLoadFailed;
		public event Action<AdPlacement> OnLoadSucceeded;
		public event Action<ImpressionData> OnImpressionSuccess;

		protected string UnitId { private set; get; }
		protected AdPlacement AdPlacement { private set; get; }
		protected com.ktgame.ads.core.BannerSize AdSize { private set; get; }
		protected BannerPosition AdPosition { private set; get; }
		
#if ADMOB
		private BannerView _bannerView;
#endif

		public AdMobCollapsibleBanner(string unitId, com.ktgame.ads.core.BannerSize bannerSize, BannerPosition bannerPosition)
		{
			UnitId = unitId;
			AdSize = bannerSize;
			AdPosition = bannerPosition;
			AdPlacement = new AdPlacement("Banner");
		}

		public void Load()
		{
#if ADMOB
			Destroy();

			GoogleMobileAds.Api.AdSize adSize = GoogleMobileAds.Api.AdSize.Banner;
			if (AdSize == com.ktgame.ads.core.BannerSize.SmartBanner)
			{
				adSize = GoogleMobileAds.Api.AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(GoogleMobileAds.Api.AdSize.FullWidth);
			}
			
			GoogleMobileAds.Api.AdPosition position = GoogleMobileAds.Api.AdPosition.Bottom;
			string collapsiblePos = "bottom";
			if (AdPosition == BannerPosition.Top)
			{
				position = GoogleMobileAds.Api.AdPosition.Top;
				collapsiblePos = "top";
			}

			_bannerView = new BannerView(UnitId, adSize, position);

			_bannerView.OnBannerAdLoaded += () =>
			{
				UnityMainThreadDispatcher.Instance.Enqueue(() =>
				{
					OnLoadSucceeded?.Invoke(AdPlacement);
				});
			};

			_bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
			{
				UnityMainThreadDispatcher.Instance.Enqueue(() =>
				{
					var adError = AdMobExtensions.ToAdError(error, AdPlacement);
					OnLoadFailed?.Invoke(adError);
				});
			};

			_bannerView.OnAdPaid += (AdValue adValue) =>
			{
				UnityMainThreadDispatcher.Instance.Enqueue(() =>
				{
					var impressionData = adValue.ToImpressionData(UnitId, AdFormat.Banner);
					OnImpressionSuccess?.Invoke(impressionData);
				});
			};

			var adRequest = new AdRequest();
			adRequest.Extras.Add("collapsible", collapsiblePos);
			
			_bannerView.LoadAd(adRequest);
#endif
		}

		public void Show()
		{
#if ADMOB
			if (_bannerView != null)
			{
				_bannerView.Show();
			}
			else
			{
				Load();
			}
#endif
		}

		public void Hide()
		{
#if ADMOB
			if (_bannerView != null)
			{
				_bannerView.Hide();
			}
#endif
		}

		public void Destroy()
		{
#if ADMOB
			if (_bannerView != null)
			{
				_bannerView.Destroy();
				_bannerView = null;
			}
#endif
		}
	}
}