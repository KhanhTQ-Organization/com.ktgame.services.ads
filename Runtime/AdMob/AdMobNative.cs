using System;
using System.Collections.Generic;

#if APPSFLYER_ANALYTICS
using AppsFlyerSDK;
#endif

using com.ktgame.ads.core;
using Cysharp.Threading.Tasks;

#if ADMOB
using GoogleMobileAds.Api;
#endif

using UnityEngine;
using AdError = com.ktgame.ads.core.AdError;
using AdFormat = com.ktgame.ads.core.AdFormat;

namespace com.ktgame.ads.admob
{
	public class AdMobNative : INativeAdapter
	{
		protected string UnitId { private set; get; }
		public event Action<AdPlacement> OnLoadSucceeded;
		public event Action<AdError> OnLoadFailed;
		public event Action<AdPlacement> OnClicked;
		public event Action<AdPlacement> OnShowSucceeded;
		public event Action OnClosed;
		public event Action<ImpressionData> OnImpressionSuccess;
		private AdPlacement AdPlacement { get; }
#if ADMOB_NATIVE
		public NativeAd CurrentNativeAd => _currentNativeAd;
		public Queue<NativeAd> NativeAdsPreload => _nativeAdsPreload;
		public List<NativeAd> NativeAdsTemp => _nativeAdsTemp;
		private readonly Queue<NativeAd> _nativeAdsPreload;
		private readonly List<NativeAd> _nativeAdsTemp;
		
		private AdLoader _adLoader;
		private NativeAd _currentNativeAd;
        public bool IsReady => _nativeAdsPreload.Count > 0 || _nativeAdsTemp.Count > 0;
#else
		public bool IsReady => true;
#endif
		
		private readonly int _maxPreloadCount = 2;
		
		public AdMobNative(string unitId)
		{
			UnitId = unitId;
			AdPlacement = new AdPlacement("Native");
#if ADMOB_NATIVE
			_nativeAdsPreload = new Queue<NativeAd>();
			_nativeAdsTemp = new List<NativeAd>();
#endif
		}

		public void Load()
		{
			LoadAsync().Forget();
		}

		private async UniTask LoadAsync()
		{
#if ADMOB_NATIVE
			await UniTask.DelaySeconds(2);
			if (_nativeAdsPreload.Count >= _maxPreloadCount)
			{
				return;
			}

			Debug.Log("Load");
			
			_adLoader = new AdLoader.Builder(UnitId)
				.ForNativeAd()
				.Build();

			_adLoader.OnNativeAdLoaded += OnLoadSucceededHandler;
			_adLoader.OnAdFailedToLoad += OnLoadFailedHandler;
			_adLoader.OnNativeAdClicked += ClickedHandler;
			_adLoader.OnNativeAdClosed += ClosedHandler;

			AdRequest request = new AdRequest();
			_adLoader.LoadAd(request);
#endif
		}
		
		public void Show()
		{
			if (!IsReady)
			{
				Debug.LogWarning("No Native Ads ready to show.");
				return;
			}
#if ADMOB_NATIVE
			_currentNativeAd = _nativeAdsPreload.Dequeue();
#endif
			OnShowSucceeded?.Invoke(AdPlacement);
			
			Load();
		}

		public void Hide()
		{
			
		}

		public void Destroy()
		{
#if ADMOB_NATIVE
			if (_currentNativeAd != null)
			{
				_currentNativeAd.Destroy();
				_currentNativeAd = null;
			}
			
			while (_nativeAdsPreload.Count > 0)
			{
				var ad = _nativeAdsPreload.Dequeue();
				ad?.Destroy();
			}
#endif
		}
		
#if ADMOB_NATIVE
		private void OnLoadSucceededHandler(object sender, NativeAdEventArgs nativeAdEventArgs)
		{
			var nativeAd = nativeAdEventArgs.nativeAd;
			nativeAd.OnPaidEvent += OnNativeAdImpression;

			_nativeAdsPreload.Enqueue(nativeAd);
			OnLoadSucceeded?.Invoke(AdPlacement);
		}

		private void OnNativeAdImpression(object sender, AdValueEventArgs e)
		{
			double revenue = e.AdValue.Value / 1000000d;
			OnImpressionSuccess?.Invoke(new ImpressionData(AdPlatform.Admob, "", UnitId, AdFormat.Native, "Native", "USD", 0));
			
#if APPSFLYER_ANALYTICS
			Dictionary<string, string> additionalParams = new Dictionary<string, string>
			{
				{ AdRevenueScheme.COUNTRY, _currentNativeAd?.GetResponseInfo().GetMediationAdapterClassName() ?? String.Empty },
				{ AdRevenueScheme.AD_UNIT, _currentNativeAd?.GetResponseInfo().GetLoadedAdapterResponseInfo().AdSourceName },
				{ AdRevenueScheme.AD_TYPE, AdFormat.Native.ToString() },
				{ AdRevenueScheme.PLACEMENT, AdPlacement.ToString() }
			};
			var logRevenue = new AFAdRevenueData("", MediationNetwork.Admost, "USD", revenue);
			AppsFlyer.logAdRevenue(logRevenue, additionalParams);
#endif
		}

		private void OnLoadFailedHandler(object sender, AdFailedToLoadEventArgs adFailedEventArgs)
		{
			var adError = AdMobExtensions.ToAdError(adFailedEventArgs.LoadAdError, AdPlacement);
			OnLoadFailed?.Invoke(adError);
			Debug.LogError($"Native ad failed: {adError.Message}");
		}
		
#endif
		private void ClosedHandler(object sender, EventArgs eventArgs)
		{
			OnClosed?.Invoke();
		}

		private void ClickedHandler(object sender, EventArgs eventArgs)
		{
			OnClicked?.Invoke(AdPlacement);
		}
	}
}