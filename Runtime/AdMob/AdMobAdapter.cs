using System;
using com.ktgame.ads.core;
#if ADMOB
using GoogleMobileAds.Api;
#endif
using UnityEngine;

namespace com.ktgame.ads.admob
{
	public class AdMobAdapter : IAdAdapter
	{
		public IBannerAdapter Banner { private set; get; }
		public IInterstitialAdapter Interstitial { private set; get; }
		public IInterstitialAdapter InterstitialImage { private set; get; }
		public IRewardVideoAdapter RewardVideo { private set; get; }
		public IAppOpenAdapter AppOpen { private set; get; }
		public IMRecAdapter MRec { private set; get; }
		public INativeAdapter Native { private set; get; }
		public INativeAdapter NativeInter { private set; get; }

		public AdMobAdapter(string sdkKey)
		{
			Banner = NullBannerAdapter.Instance;
			Interstitial = NullInterstitialAdapter.Instance;
			InterstitialImage = NullInterstitialAdapter.Instance;
			RewardVideo = NullRewardVideoAdapter.Instance;
			AppOpen = NullAppOpenAdapter.Instance;
			MRec = NullMRecAdapter.Instance;
			Native = NullNativeAdapter.Instance;
		}

		public void Initialize(Action<bool> onComplete)
		{
#if ADMOB
			MobileAds.Initialize(initStatus =>
			{
				if (initStatus == null)
				{
					Debug.LogError("Google Mobile Ads initialization failed.");
					return;
				}
				var adapterStatusMap = initStatus.getAdapterStatusMap();
				if (adapterStatusMap != null)
				{
					foreach (var item in adapterStatusMap)
					{
						Debug.Log(string.Format("Adapter {0} is {1}", item.Key, item.Value.InitializationState));
					}
				}
				
				MobileAds.RaiseAdEventsOnUnityMainThread = true;
				MobileAds.SetiOSAppPauseOnBackground(true);
				onComplete?.Invoke(true);
			});
#endif
		}

		public void SetPause(bool pause) { }

		public void SetBanner(IBannerAdapter bannerAdapter)
		{
			Banner = bannerAdapter;
		}

		public void SetInterstitial(IInterstitialAdapter interstitialAdapter)
		{
			Interstitial = interstitialAdapter;
		}

		public void SetInterstitialImage(IInterstitialAdapter interstitialAdapter)
		{
			InterstitialImage = interstitialAdapter;
		}

		public void SetRewardVideo(IRewardVideoAdapter rewardVideoAdapter)
		{
			RewardVideo = rewardVideoAdapter;
		}

		public void SetAppOpen(IAppOpenAdapter appOpenAdapter)
		{
			AppOpen = appOpenAdapter;
		}

		public void SetMRec(IMRecAdapter mRecAdapter)
		{
			MRec = mRecAdapter;
		}

		public void SetNative(INativeAdapter nativeAdapter)
		{
			Native = nativeAdapter;
		}

		public void SetNativeInter(INativeAdapter nativeInterAdapter)
		{
			NativeInter = nativeInterAdapter;
		}
	}
}