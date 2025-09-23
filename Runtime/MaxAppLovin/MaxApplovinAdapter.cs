using System;
using com.ktgame.ads.core;

namespace com.ktgame.ads.max_applovin
{
	public class MaxApplovinAdapter : IAdAdapter
	{
		public IBannerAdapter Banner { private set; get; }
		public IInterstitialAdapter Interstitial { private set; get; }
		public IInterstitialAdapter InterstitialImage { private set; get; }
		public IRewardVideoAdapter RewardVideo { private set; get; }
		public IAppOpenAdapter AppOpen { private set; get; }
		public IMRecAdapter MRec { private set; get; }
		public INativeAdapter Native { private set; get; }
		public INativeAdapter NativeInter { private set; get; }

		public MaxApplovinAdapter(string sdkKey)
		{
#if MAX_APPLOVIN
			//MaxSdk.SetSdkKey(sdkKey);
#endif
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
#if MAX_APPLOVIN
			MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
			{
				onComplete?.Invoke(true);
			};
			MaxSdk.InitializeSdk();
#endif
		}

		public void SetUserId(string userId)
		{
#if MAX_APPLOVIN
			MaxSdk.SetUserId(userId);
#endif
		}

		public void SetVerboseLogging(bool enabled)
		{
#if MAX_APPLOVIN
			MaxSdk.SetVerboseLogging(enabled);
#endif
		}

		public void SetTestDeviceIds(params string[] ids)
		{
#if MAX_APPLOVIN
			MaxSdk.SetTestDeviceAdvertisingIdentifiers(ids);
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