using com.ktgame.ads.core;
using com.ktgame.ads.core.extensions;
using com.ktgame.core;
using com.ktgame.core.di;
using com.ktgame.services.remote_config;
using com.ktgame.services.scene;

#if MAX_APPLOVIN
using com.ktgame.ads.max_applovin;
#endif

#if ADMOB
using com.ktgame.ads.admob;
using GoogleMobileAds.Ump.Api;
#endif

#if ADJUST_ANALYTICS
using com.ktgame.services.ads.adjust_ad_revenue;
#endif

#if FIREBASE_ANALYTICS
using com.ktgame.services.ads.firebase_ad_revenue;
#endif

using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.ktgame.services.ads
{
	[Service(typeof(IAdService))]
	public class AdService : MonoBehaviour, IAdService
	{
		[Inject] private IRemoteConfigService _remoteConfigService;
		
		public int Priority => 2;
		public bool Initialized { get; set; }
		public IAdAdapter Ad { private set; get; }
		public IAdAdapter AdBackFill { private set; get; }

		private IBannerAdapter _bannerAdapter;
		private IInterstitialAdapter _interstitialAdapter;
		private IRewardVideoAdapter _rewardVideoAdapter;
		private IAppOpenAdapter _appOpenAdapter;
		private IMRecAdapter _mRecAdapter;
		
		private IBannerAdapter _bannerAdBackFillAdapter;
		private IInterstitialAdapter _interstitialBackFillAdapter;
		private IInterstitialAdapter _interstitialImageBackFillAdapter;
		private IRewardVideoAdapter _rewardVideoBackFillAdapter;
		private IAppOpenAdapter _appOpenBackFillAdapter;
		private INativeAdapter _nativeAdapterBackFill;
		private INativeAdapter _nativeInterAdapterBackFill;

		private AdServiceSettings _settings;
#if ADMOB	
		private ConsentForm _consentForm;
#endif
		public async UniTask OnInitialize(IArchitecture architecture)
		{
			_settings = AdServiceSettings.Instance;
#if ADMOB
			ConsentRequestParameters request = new ConsentRequestParameters
			{
				TagForUnderAgeOfConsent = false,
			};

			ConsentInformation.Update(request, OnConsentInfoUpdated);
#endif
			await UniTask.DelaySeconds(1f);
			await SetAdsMaxAppLovin();
			await SetAdsBackFill();
			
			Initialized = true;
		}

		private async UniTask SetAdsMaxAppLovin()
		{
#if UNITY_ANDROID && MAX_APPLOVIN
			Ad = new MaxApplovinAdapter(_settings.AndroidMaxApplovinAppKey);

			if (!string.IsNullOrEmpty(_settings.AndroidMaxApplovinBannerUnitId))
			{
				_bannerAdapter = new MaxApplovinBanner(_settings.AndroidMaxApplovinBannerUnitId, _settings.BannerSize,
					_settings.BannerPosition);
			}

			if (!string.IsNullOrEmpty(_settings.AndroidMaxApplovinInterstitialUnitId))
			{
				_interstitialAdapter = new MaxApplovinInterstitial(_settings.AndroidMaxApplovinInterstitialUnitId);
			}

			if (!string.IsNullOrEmpty(_settings.AndroidMaxApplovinRewardedVideoUnitId))
			{
				_rewardVideoAdapter = new MaxApplovinRewardVideo(_settings.AndroidMaxApplovinRewardedVideoUnitId);
			}

			if (!string.IsNullOrEmpty(_settings.AndroidMaxApplovinMRecUnitId))
			{
				_mRecAdapter = new MaxApplovinMRec(_settings.AndroidMaxApplovinMRecUnitId, _settings.MRecDp,
					_settings.MRecPosition);
			}

			_appOpenAdapter = NullAppOpenAdapter.Instance;
#elif UNITY_IOS && MAX_APPLOVIN
			Ad = new MaxApplovinAdapter(_settings.IOSMaxApplovinAppKey);

			if (!string.IsNullOrEmpty(_settings.IOSMaxApplovinBannerUnitId))
			{
				_bannerAdapter = new MaxApplovinBanner(_settings.IOSMaxApplovinBannerUnitId, _settings.BannerSize,
					_settings.BannerPosition);
			}

			if (!string.IsNullOrEmpty(_settings.IOSMaxApplovinInterstitialUnitId))
			{
				_interstitialAdapter = new MaxApplovinInterstitial(_settings.IOSMaxApplovinInterstitialUnitId);
			}

			if (!string.IsNullOrEmpty(_settings.IOSMaxApplovinRewardedVideoUnitId))
			{
				_rewardVideoAdapter = new MaxApplovinRewardVideo(_settings.IOSMaxApplovinRewardedVideoUnitId);
			}

			if (!string.IsNullOrEmpty(_settings.IOSMaxApplovinRewardedVideoUnitId))
			{
				_appOpenAdapter = new MaxApplovinAppOpen(_settings.IOSMaxApplovinAppOpenUnitId);
			}

			if (!string.IsNullOrEmpty(_settings.IOSMaxApplovinMRecUnitId))
			{
				_mRecAdapter = new MaxApplovinMRec(_settings.IOSMaxApplovinMRecUnitId,_settings.MRecDp,_settings.MRecPosition);
			}
#else
			Ad = new NullAdAdapter();
			_bannerAdapter = NullBannerAdapter.Instance;
			_interstitialAdapter = NullInterstitialAdapter.Instance;
			_rewardVideoAdapter = NullRewardVideoAdapter.Instance;
			_appOpenAdapter = NullAppOpenAdapter.Instance;
			_mRecAdapter = NullMRecAdapter.Instance;
#endif

			var requestStrategy = new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay);
			if (_bannerAdapter != null)
			{
				_bannerAdapter = new AutoRequestBanner(requestStrategy, _bannerAdapter);
			}

			if (_interstitialAdapter != null)
			{
				_interstitialAdapter = new AutoRequestInterstitial(requestStrategy, _interstitialAdapter);
			}

			if (_rewardVideoAdapter != null)
			{
				_rewardVideoAdapter = new AutoRequestRewardVideo(requestStrategy, _rewardVideoAdapter);
			}

			if (_appOpenAdapter != null)
			{
				_appOpenAdapter = new AutoRequestAppOpen(requestStrategy, _appOpenAdapter);
			}

			if (_mRecAdapter != null)
			{
				_mRecAdapter = new AutoRequestMRec(requestStrategy, _mRecAdapter);
			}

#if FIREBASE_ANALYTICS
            if (_bannerAdapter != null)
            {
                _bannerAdapter = new FirebaseAdRevenueBanner(_bannerAdapter);    
            }

            if (_interstitialAdapter != null)
            {
                _interstitialAdapter = new FirebaseAdRevenueInterstitial(_interstitialAdapter);    
            }

            if (_rewardVideoAdapter != null)
            {
                _rewardVideoAdapter = new FirebaseAdRevenueRewardVideo(_rewardVideoAdapter);    
            }
			
			if (_mRecAdapter != null)
			{
				_mRecAdapter = new FirebaseAdRevenueMRec(_mRecAdapter);    
			}
#endif

#if ADJUST_ANALYTICS
            if (_bannerAdapter != null)
            {
                _bannerAdapter = new AdjustAdRevenueBanner(_bannerAdapter);    
            }

            if (_interstitialAdapter != null)
            {
                _interstitialAdapter = new AdjustAdRevenueInterstitial(_interstitialAdapter);    
            }

            if (_rewardVideoAdapter != null)
            {
                _rewardVideoAdapter = new AdjustAdRevenueRewardVideo(_rewardVideoAdapter);    
            }

			if (_mRecAdapter != null)
			{
				_mRecAdapter = new AdjustAdRevenueMRec(_mRecAdapter);
			}
#endif

			if (_bannerAdapter != null)
			{
				Ad.SetBanner(_bannerAdapter);
			}

			if (_interstitialAdapter != null)
			{
				Ad.SetInterstitial(_interstitialAdapter);
			}

			if (_rewardVideoAdapter != null)
			{
				Ad.SetRewardVideo(_rewardVideoAdapter);
			}

			if (_appOpenAdapter != null)
			{
				Ad.SetAppOpen(_appOpenAdapter);
			}

			if (_mRecAdapter != null)
			{
				Ad.SetMRec(_mRecAdapter);
			}
			
			Ad.Initialize(isInitialized =>
			{
				if (isInitialized)
				{
					requestStrategy.Request();
				}

				Initialized = true;
			});
		}

		private async UniTask SetAdsBackFill()
		{
#if UNITY_ANDROID && ADMOB
			
			AdBackFill = new AdMobAdapter(_settings.AndroidMaxApplovinAppKey);

			if (!string.IsNullOrEmpty(_settings.AndroidAdmobBannerUnitId))
			{
				_bannerAdBackFillAdapter = new AdMobCollapsibleBanner(_settings.AndroidAdmobBannerUnitId, _settings.BannerSize, _settings.BannerPosition);
			}

			if (!string.IsNullOrEmpty(_settings.AndroidAdmobInterstitialUnitId))
			{
				_interstitialBackFillAdapter = new AdMobInterstitial(_settings.AndroidAdmobInterstitialUnitId);
			}
			
			var idImage = _remoteConfigService.GetValue(RemoteConfigKey.intertitial_image_ad_id).String;
			if (!string.IsNullOrEmpty(_settings.AndroidAdmobInterstitialUnitId))
			{
				_interstitialImageBackFillAdapter = new AdMobInterstitial(idImage);
			}

			if (!string.IsNullOrEmpty(_settings.AndroidAdmobRewardedVideoUnitId))
			{
				_rewardVideoBackFillAdapter = new AdmobRewardVideo(_settings.AndroidAdmobRewardedVideoUnitId);
			}

			var idAOA = _remoteConfigService.GetValue(RemoteConfigKey.open_ad_id).String;
			if (!string.IsNullOrEmpty(idAOA))
			{
				_appOpenBackFillAdapter = new AdMobAppOpen(idAOA);
			}
			else if (!string.IsNullOrEmpty(_settings.AndroidAdmobAppOpenUnitId))
			{
				_appOpenBackFillAdapter = new AdMobAppOpen(_settings.AndroidAdmobAppOpenUnitId);
			}

			var idNative = _remoteConfigService.GetValue(RemoteConfigKey.native_ad_id).String;
			Debug.Log(idNative);
			if (!string.IsNullOrEmpty(idNative))
			{
				_nativeAdapterBackFill = new AdMobNative(idNative);
			}
			else if (!string.IsNullOrEmpty(_settings.AndroidAdmobNativeUnitId))
			{
				_nativeAdapterBackFill = new AdMobNative(_settings.AndroidAdmobNativeUnitId);
			}
			
			var idNativeInter = _remoteConfigService.GetValue(RemoteConfigKey.native_inter_ad_id).String;
			if (!string.IsNullOrEmpty(idNativeInter))
			{
				_nativeInterAdapterBackFill = new AdMobNative(idNativeInter);
			}
			else if (!string.IsNullOrEmpty(_settings.AndroidAdmobNativeInterUnitId))
			{
				_nativeInterAdapterBackFill = new AdMobNative(_settings.AndroidAdmobNativeInterUnitId);
			}
			
#elif UNITY_IOS && ADMOB
			AdBackFill = new AdMobAdapter(_settings.IOSAdmobAppOpenUnitId);

			if (!string.IsNullOrEmpty(_settings.IOSAdmobBannerUnitId))
			{
				_bannerAdBackFillAdapter = new AdMobCollapsibleBanner(_settings.IOSAdmobBannerUnitId, _settings.BannerSize, _settings.BannerPosition);
			}

			if (!string.IsNullOrEmpty(_settings.IOSAdmobInterstitialUnitId))
			{
				_interstitialBackFillAdapter = new AdMobInterstitial(_settings.IOSAdmobInterstitialUnitId);
			}

			if (!string.IsNullOrEmpty(_settings.IOSAdmobInterstitialUnitId))
			{
				_interstitialImageBackFillAdapter = new AdMobInterstitial(_settings.IOSAdmobInterstitialUnitId);
			}

			if (!string.IsNullOrEmpty(_settings.IOSAdmobRewardedVideoUnitId))
			{
				_rewardVideoBackFillAdapter = new AdmobRewardVideo(_settings.IOSAdmobRewardedVideoUnitId);
			}

			if (!string.IsNullOrEmpty(_settings.IOSAdmobAppOpenUnitId))
			{
				_appOpenBackFillAdapter = new AdMobAppOpen(_settings.IOSAdmobAppOpenUnitId);
			}
#else
			AdBackFill = new NullAdAdapter();
			_bannerAdBackFillAdapter = NullBannerAdapter.Instance;
			_interstitialBackFillAdapter = NullInterstitialAdapter.Instance;
			_interstitialImageBackFillAdapter = NullInterstitialAdapter.Instance;
			_rewardVideoBackFillAdapter = NullRewardVideoAdapter.Instance;
			_appOpenBackFillAdapter = NullAppOpenAdapter.Instance;
			_nativeAdapterBackFill = NullNativeAdapter.Instance;
			_nativeInterAdapterBackFill = NullNativeAdapter.Instance;
#endif
			var requestStrategy = new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay);

			if (_bannerAdBackFillAdapter != null)
			{
				_bannerAdBackFillAdapter = new AutoRequestBanner(requestStrategy, _bannerAdBackFillAdapter);
			}

			if (_interstitialBackFillAdapter != null)
			{
				_interstitialBackFillAdapter = new AutoRequestInterstitial(requestStrategy, _interstitialBackFillAdapter);
			}
			
			if (_interstitialImageBackFillAdapter != null)
			{
				_interstitialImageBackFillAdapter = new AutoRequestInterstitial(requestStrategy, _interstitialImageBackFillAdapter);
			}

			if (_rewardVideoBackFillAdapter != null)
			{
				_rewardVideoBackFillAdapter = new AutoRequestRewardVideo(requestStrategy, _rewardVideoBackFillAdapter);
			}

			if (_appOpenBackFillAdapter != null)
			{
				_appOpenBackFillAdapter = new AutoRequestAppOpen(requestStrategy, _appOpenBackFillAdapter);
			}

			if (_nativeAdapterBackFill != null)
			{
				_nativeAdapterBackFill = new AutoRequestNative(requestStrategy, _nativeAdapterBackFill);
			}

			if (_nativeInterAdapterBackFill != null)
			{
				_nativeInterAdapterBackFill = new AutoRequestNative(requestStrategy, _nativeInterAdapterBackFill);
			}
			
#if FIREBASE_ANALYTICS
            if (_bannerAdBackFillAdapter != null)
            {
                _bannerAdBackFillAdapter = new FirebaseAdRevenueBanner(_bannerAdapter);    
            }

            if (_interstitialBackFillAdapter != null)
            {
                _interstitialBackFillAdapter = new FirebaseAdRevenueInterstitial(_interstitialBackFillAdapter);    
            }
			
			if (_interstitialImageBackFillAdapter != null)
            {
				_interstitialImageBackFillAdapter = new FirebaseAdRevenueInterstitial(_interstitialImageBackFillAdapter);    
            }

            if (_rewardVideoBackFillAdapter != null)
            {
                _rewardVideoBackFillAdapter = new FirebaseAdRevenueRewardVideo(_rewardVideoBackFillAdapter);    
            }

			if (_nativeAdapterBackFill != null)
			{
				_nativeAdapterBackFill = new FirebaseAdRevenueNative(_nativeAdapterBackFill);
			}
			
			if (_nativeInterAdapterBackFill != null)
			{
				_nativeInterAdapterBackFill = new FirebaseAdRevenueNative(_nativeInterAdapterBackFill);
			}
#endif

			if (_bannerAdBackFillAdapter != null)
			{
				AdBackFill.SetBanner(_bannerAdBackFillAdapter);
			}

			if (_interstitialBackFillAdapter != null)
			{
				AdBackFill.SetInterstitial(_interstitialBackFillAdapter);
			}
			
			if (_interstitialImageBackFillAdapter != null)
			{
				AdBackFill.SetInterstitialImage(_interstitialImageBackFillAdapter);
			}

			if (_rewardVideoBackFillAdapter != null)
			{
				AdBackFill.SetRewardVideo(_rewardVideoBackFillAdapter);
			}

			if (_appOpenBackFillAdapter != null)
			{
				AdBackFill.SetAppOpen(_appOpenBackFillAdapter);
			}

			if (_nativeAdapterBackFill != null)
			{
				AdBackFill.SetNative(_nativeAdapterBackFill);
			}
			
			if (_nativeInterAdapterBackFill != null)
			{
				AdBackFill.SetNativeInter(_nativeInterAdapterBackFill);
			}
			
#if ADMOB
			AdBackFill.Initialize(isInitialized =>
			{
				if (isInitialized)
				{
					requestStrategy.Request();
				}

				Initialized = true;
			});
#endif
		}
		
#if ADMOB
		private void OnConsentInfoUpdated(FormError error)
		{
			if (error != null)
			{
				UnityEngine.Debug.LogError(error);
				return;
			}

			if (ConsentInformation.IsConsentFormAvailable())
			{
				LoadConsentForm();
			}
		}

		private void LoadConsentForm()
		{
			ConsentForm.Load(OnLoadConsentForm);
		}
		
		private void OnLoadConsentForm(ConsentForm consentForm, FormError error)
		{
			if (error != null)
			{
				Debug.LogError(error);
				return;
			}

			_consentForm = consentForm;

			if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
			{
				_consentForm.Show(OnShowForm);
			}
		}
		
		private void OnShowForm(FormError error)
		{
			if (error != null)
			{
				Debug.LogError(error);
				return;
			}

			LoadConsentForm();
		}
#endif
		public void SetPause(bool pause)
		{
			Ad?.SetPause(pause);
		}
	}
}