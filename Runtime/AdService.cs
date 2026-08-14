using com.ktgame.ads.core;
using com.ktgame.ads.core.extensions;
using com.ktgame.core.di;
using com.ktgame.services.remote_config;

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

#if APPSFLYER_ANALYTICS
using com.ktgame.services.ads.appsflyer_ad_revenue;
#endif

using Cysharp.Threading.Tasks;
using UnityEngine;
using com.ktgame.core;

namespace com.ktgame.services.ads
{
	[Service(typeof(IAdService))]
	public class AdService : MonoBehaviour, IAdService
	{
		[Inject] private IRemoteConfigService _remoteConfigService;
		
#if COLLAPSIBLE
		[Inject] private IGmaNativeInterstitial _gmaNativePopup;
		[Inject] private IGmaNativeCollapsible _gmaNativeCollapsible;
		[Inject] private IMRecCollapsibleAds  _mRecCollapsibleAds;
#endif
		
		public int Priority => 2;
		public bool Initialized { get; set; }
		public IAdAdapter Ad { private set; get; }
		public IAdAdapter AdBackFill { private set; get; }

		private AdServiceSettings _settings;
		private IAASettingSO _iaaSettingSo;
#if ADMOB
		private ConsentForm _consentForm;
#endif
		public async UniTask OnInitialize(IArchitecture architecture)
		{
			_settings = AdServiceSettings.Instance;
			_iaaSettingSo = IAASettingSO.Instance;
#if PRODUCTION
			//_remoteConfigService ??= Game.Instance.GetService<IRemoteConfigService>();
#if COLLAPSIBLE
			_gmaNativePopup ??= Game.Instance.GetService<IGmaNativeInterstitial>();
			_gmaNativeCollapsible  ??= Game.Instance.GetService<IGmaNativeCollapsible>();
			_mRecCollapsibleAds  ??= Game.Instance.GetService<IMRecCollapsibleAds>();
#endif
#endif

#if ADMOB
			ConsentRequestParameters request = new ConsentRequestParameters
			{
				TagForUnderAgeOfConsent = false,
			};

			ConsentInformation.Update(request, OnConsentInfoUpdated);
#endif
			await UniTask.DelaySeconds(1f);
			await SetMainAds();
			await SetBackFillAds();

			Initialized = true;
		}

		private string GetUnitId(string newId, string oldId)
		{
			return string.IsNullOrEmpty(newId) ? oldId : newId;
		}

		private async UniTask SetMainAds()
		{
			switch (_iaaSettingSo.MainMediation)
			{
				case IAAMediationFlag.Max:
					await SetAdsMaxAppLovin(true);
					break;
				case IAAMediationFlag.GMA:
					await SetAdsAdMob(true);
					break;
				case IAAMediationFlag.IronSource:
					Debug.LogWarning("IronSource is selected as Main Mediation but is not fully implemented in AdService yet.");
					await SetNullAds(true);
					break;
				default:
					// Fallback to legacy behavior if MainMediation isn't set properly
					await SetAdsMaxAppLovin(true);
					break;
			}
		}

		private async UniTask SetBackFillAds()
		{
			switch (_iaaSettingSo.BackfillMediation)
			{
				case IAAMediationFlag.Max:
					await SetAdsMaxAppLovin(false);
					break;
				case IAAMediationFlag.GMA:
					await SetAdsAdMob(false);
					break;
				case IAAMediationFlag.IronSource:
					Debug.LogWarning("IronSource is selected as Backfill Mediation but is not fully implemented in AdService yet.");
					await SetNullAds(false);
					break;
				default:
					// Fallback to legacy behavior if BackfillMediation isn't set properly
					await SetAdsAdMob(false);
					break;
			}
		}

		private async UniTask SetNullAds(bool isMain)
		{
			IAdAdapter adapter = new NullAdAdapter();
			ApplyDecoratorsAndInitialize(adapter, null, null, null, null, null, null, null, null, null, null, isMain);
			await UniTask.CompletedTask;
		}

		private async UniTask SetAdsMaxAppLovin(bool isMain)
		{
			IAdAdapter adapter = new NullAdAdapter();
			IBannerAdapter bannerAdapter = null;
			IInterstitialAdapter interstitialAdapter = null;
			IRewardVideoAdapter rewardVideoAdapter = null;
			IAppOpenAdapter appOpenAdapter = null;
			IMRecAdapter mRecAdapter = null;

#if UNITY_ANDROID && MAX_APPLOVIN
			var appKey = GetUnitId(_iaaSettingSo.MaxAndroid.AppID, _settings.AndroidMaxApplovinAppKey);
			adapter = new MaxApplovinAdapter(appKey);

			var appOpenId = GetUnitId(_iaaSettingSo.MaxAndroid.AoaUnitID, _settings.AndroidMaxApplovinAppOpenUnitId);
			if (!string.IsNullOrEmpty(appOpenId)) appOpenAdapter = new MaxApplovinAppOpen(appOpenId);

			var bannerId = GetUnitId(_iaaSettingSo.MaxAndroid.BannerUnitID, _settings.AndroidMaxApplovinBannerUnitId);
			if (!string.IsNullOrEmpty(bannerId)) bannerAdapter = new MaxApplovinBanner(bannerId, _settings.BannerSize, _settings.BannerPosition);

			var interId = GetUnitId(_iaaSettingSo.MaxAndroid.InterstitialUnitID, _settings.AndroidMaxApplovinInterstitialUnitId);
			if (!string.IsNullOrEmpty(interId)) interstitialAdapter = new MaxApplovinInterstitial(interId);

			var rewardId = GetUnitId(_iaaSettingSo.MaxAndroid.RewardUnitID, _settings.AndroidMaxApplovinRewardedVideoUnitId);
			if (!string.IsNullOrEmpty(rewardId)) rewardVideoAdapter = new MaxApplovinRewardVideo(rewardId);

			var mrecId = GetUnitId(_iaaSettingSo.MaxAndroid.MRecUnitID, _settings.AndroidMaxApplovinMRecUnitId);
			if (!string.IsNullOrEmpty(mrecId)) 
			{
				mRecAdapter = new MaxApplovinMRec(mrecId, _settings.MRecDp, _settings.MRecPosition);
#if COLLAPSIBLE
				_mRecCollapsibleAds?.Initialize();
#endif
			}
#elif UNITY_IOS && MAX_APPLOVIN
			var appKey = GetUnitId(_iaaSettingSo.MaxIos.AppID, _settings.IOSMaxApplovinAppKey);
			adapter = new MaxApplovinAdapter(appKey);

			var appOpenId = GetUnitId(_iaaSettingSo.MaxIos.AoaUnitID, _settings.IOSMaxApplovinAppOpenUnitId);
			if (!string.IsNullOrEmpty(appOpenId)) appOpenAdapter = new MaxApplovinAppOpen(appOpenId);

			var bannerId = GetUnitId(_iaaSettingSo.MaxIos.BannerUnitID, _settings.IOSMaxApplovinBannerUnitId);
			if (!string.IsNullOrEmpty(bannerId)) bannerAdapter = new MaxApplovinBanner(bannerId, _settings.BannerSize, _settings.BannerPosition);

			var interId = GetUnitId(_iaaSettingSo.MaxIos.InterstitialUnitID, _settings.IOSMaxApplovinInterstitialUnitId);
			if (!string.IsNullOrEmpty(interId)) interstitialAdapter = new MaxApplovinInterstitial(interId);

			var rewardId = GetUnitId(_iaaSettingSo.MaxIos.RewardUnitID, _settings.IOSMaxApplovinRewardedVideoUnitId);
			if (!string.IsNullOrEmpty(rewardId)) rewardVideoAdapter = new MaxApplovinRewardVideo(rewardId);

			var mrecId = GetUnitId(_iaaSettingSo.MaxIos.MRecUnitID, _settings.IOSMaxApplovinMRecUnitId);
			if (!string.IsNullOrEmpty(mrecId)) mRecAdapter = new MaxApplovinMRec(mrecId, _settings.MRecDp, _settings.MRecPosition);
#endif

			ApplyDecoratorsAndInitialize(adapter, bannerAdapter, interstitialAdapter, null, rewardVideoAdapter, appOpenAdapter, null, mRecAdapter, null, null, null, isMain);
			await UniTask.CompletedTask;
		}

		private async UniTask SetAdsAdMob(bool isMain)
		{
			IAdAdapter adapter = new NullAdAdapter();
			IBannerAdapter bannerAdapter = null;
			IInterstitialAdapter interstitialAdapter = null;
			IInterstitialAdapter interstitialImageAdapter = null;
			IRewardVideoAdapter rewardVideoAdapter = null;
			IAppOpenAdapter appOpenAdapter = null;
			IAppOpenAdapter appOpenResumeAdapter = null;
			IMRecAdapter mRecAdapter = null;
			INativeAdapter nativeAdapter = null;
			INativeAdapter nativeInterAdapter = null;
			INativeAdapter nativeCollapAdapter = null;

#if UNITY_ANDROID && ADMOB
			var appKey = GetUnitId(_iaaSettingSo.GmaAndroid.AppID, _settings.AndroidAdmobAppKey);
			adapter = new AdMobAdapter(appKey);

			var bannerId = GetUnitId(_iaaSettingSo.GmaAndroid.BannerCollapsibleUnitID, _settings.AndroidAdmobBannerUnitId);
			if (!string.IsNullOrEmpty(bannerId)) bannerAdapter = new AdMobCollapsibleBanner(bannerId, _settings.BannerSize, _settings.BannerPosition);

			var interId = GetUnitId(_iaaSettingSo.GmaAndroid.InterstitialUnitID, _settings.AndroidAdmobInterstitialUnitId);
			if (!string.IsNullOrEmpty(interId)) interstitialAdapter = new AdMobInterstitial(interId);

			var interImageId = _remoteConfigService?.GetValue(RemoteConfigKey.inter_image_ad_id).String 
                               ?? GetUnitId(_iaaSettingSo.GmaAndroid.InterstitialImageUnitID, _settings.AndroidAdmobInterstitialImageUnitId);
			if (!string.IsNullOrEmpty(interImageId)) interstitialImageAdapter = new AdMobInterstitial(interImageId);

			var rewardId = GetUnitId(_iaaSettingSo.GmaAndroid.RewardUnitID, _settings.AndroidAdmobRewardedVideoUnitId);
			if (!string.IsNullOrEmpty(rewardId)) rewardVideoAdapter = new AdmobRewardVideo(rewardId);

			var aoaId = _remoteConfigService?.GetValue(RemoteConfigKey.open_ad_id).String 
                        ?? GetUnitId(_iaaSettingSo.GmaAndroid.AoaUnitID, _settings.AndroidAdmobAppOpenUnitId);
			if (!string.IsNullOrEmpty(aoaId)) appOpenAdapter = new AdMobAppOpen(aoaId);

			var aoaResumeId = _remoteConfigService?.GetValue(RemoteConfigKey.open_ad_resume_id).String 
                              ?? GetUnitId(_iaaSettingSo.GmaAndroid.AoaResumeUnitID, _settings.AndroidAdmobAppOpenResumeUnitId);
			if (!string.IsNullOrEmpty(aoaResumeId)) appOpenResumeAdapter = new AdMobAppOpen(aoaResumeId);

#if ADMOB_NATIVE
			var nativeId = _remoteConfigService?.GetValue(RemoteConfigKey.native_ad_id).String 
                           ?? GetUnitId(_iaaSettingSo.GmaAndroid.NativeUnitID, _settings.AndroidAdmobNativeUnitId);
			if (!string.IsNullOrEmpty(nativeId)) nativeAdapter = new AdMobNative(nativeId, AdFormat.Native);

			var nativeInterId = _remoteConfigService?.GetValue(RemoteConfigKey.native_interstitial_ad_id).String;
			if (!string.IsNullOrEmpty(nativeInterId))
			{
				_gmaNativePopup?.Initialize(nativeInterId);
			}
			else
			{
				nativeInterId = GetUnitId(_iaaSettingSo.GmaAndroid.NativeInterstitialUnitID, _settings.AndroidAdmobNativeInterUnitId);
				if (!string.IsNullOrEmpty(nativeInterId)) nativeInterAdapter = new AdMobNative(nativeInterId, AdFormat.NativeCollapsile);
			}
			
			var nativeCollapId = _remoteConfigService?.GetValue(RemoteConfigKey.native_collap_ad_id).String 
                                 ?? GetUnitId(_iaaSettingSo.GmaAndroid.NativeCollapsibleUnitID, _settings.AndroidAdmobNativeCollapsibleUnitId);
			if (!string.IsNullOrEmpty(nativeCollapId))
			{
				_gmaNativeCollapsible?.Initialize(nativeCollapId);
			}
#endif // ADMOB_NATIVE

#elif UNITY_IOS && ADMOB
			var appKey = GetUnitId(_iaaSettingSo.GmaIos.AppID, _settings.IOSAdmobAppKey); // the legacy logic had a bug using appOpenUnitId
			adapter = new AdMobAdapter(appKey);

			var bannerId = GetUnitId(_iaaSettingSo.GmaIos.BannerCollapsibleUnitID, _settings.IOSAdmobBannerUnitId);
			if (!string.IsNullOrEmpty(bannerId)) bannerAdapter = new AdMobCollapsibleBanner(bannerId, _settings.BannerSize, _settings.BannerPosition);

			var interId = GetUnitId(_iaaSettingSo.GmaIos.InterstitialUnitID, _settings.IOSAdmobInterstitialUnitId);
			if (!string.IsNullOrEmpty(interId)) interstitialAdapter = new AdMobInterstitial(interId);

			var interImageId = GetUnitId(_iaaSettingSo.GmaIos.InterstitialImageUnitID, _settings.IOSAdmobInterstitialUnitId); 
			if (!string.IsNullOrEmpty(interImageId)) interstitialImageAdapter = new AdMobInterstitial(interImageId);

			var rewardId = GetUnitId(_iaaSettingSo.GmaIos.RewardUnitID, _settings.IOSAdmobRewardedVideoUnitId);
			if (!string.IsNullOrEmpty(rewardId)) rewardVideoAdapter = new AdmobRewardVideo(rewardId);

			var aoaId = GetUnitId(_iaaSettingSo.GmaIos.AoaUnitID, _settings.IOSAdmobAppOpenUnitId);
			if (!string.IsNullOrEmpty(aoaId)) appOpenAdapter = new AdMobAppOpen(aoaId);
#endif

			ApplyDecoratorsAndInitialize(adapter, bannerAdapter, interstitialAdapter, interstitialImageAdapter, rewardVideoAdapter, appOpenAdapter, appOpenResumeAdapter, mRecAdapter, nativeAdapter, nativeInterAdapter, nativeCollapAdapter, isMain);
			await UniTask.CompletedTask;
		}

		private void ApplyDecoratorsAndInitialize(
			IAdAdapter adapter, IBannerAdapter banner, IInterstitialAdapter inter,
			IInterstitialAdapter interImage, IRewardVideoAdapter reward, IAppOpenAdapter appOpen,
			IAppOpenAdapter appOpenResume, IMRecAdapter mrec, INativeAdapter native,
			INativeAdapter nativeInter, INativeAdapter nativeCollap, bool isMain)
		{
			if (banner != null) banner = new AutoRequestBanner(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), banner);
			if (inter != null) inter = new AutoRequestInterstitial(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), inter);
			if (interImage != null) interImage = new AutoRequestInterstitial(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), interImage);
			if (reward != null) reward = new AutoRequestRewardVideo(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), reward);
			if (appOpen != null) appOpen = new AutoRequestAppOpen(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), appOpen);
			if (appOpenResume != null) appOpenResume = new AutoRequestAppOpen(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), appOpenResume);
			if (mrec != null) mrec = new AutoRequestMRec(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), mrec);
			if (native != null) native = new AutoRequestNative(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), native);
			if (nativeInter != null) nativeInter = new AutoRequestNative(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), nativeInter);
			if (nativeCollap != null) nativeCollap = new AutoRequestNative(new ExponentialCooldown(_settings.MaxRetryAttemptRequest, _settings.BaseRetryDelay), nativeCollap);

#if FIREBASE_ANALYTICS
			if (banner != null) banner = new FirebaseAdRevenueBanner(banner);
			if (inter != null) inter = new FirebaseAdRevenueInterstitial(inter);
			if (interImage != null) interImage = new FirebaseAdRevenueInterstitial(interImage);
			if (reward != null) reward = new FirebaseAdRevenueRewardVideo(reward);
			if (appOpen != null) appOpen = new FirebaseAdRevenueAppOpen(appOpen);
			if (appOpenResume != null) appOpenResume = new FirebaseAdRevenueAppOpen(appOpenResume);
			if (mrec != null) mrec = new FirebaseAdRevenueMRec(mrec);
			if (native != null) native = new FirebaseAdRevenueNative(native);
			if (nativeInter != null) nativeInter = new FirebaseAdRevenueNative(nativeInter);
			if (nativeCollap != null) nativeCollap = new FirebaseAdRevenueNative(nativeCollap);
#endif

#if ADJUST_ANALYTICS
			if (banner != null) banner = new AdjustAdRevenueBanner(banner);
			if (inter != null) inter = new AdjustAdRevenueInterstitial(inter);
			if (interImage != null) interImage = new AdjustAdRevenueInterstitial(interImage);
			if (reward != null) reward = new AdjustAdRevenueRewardVideo(reward);
			if (appOpen != null) appOpen = new AdjustAdRevenueAppOpen(appOpen);
			if (appOpenResume != null) appOpenResume = new AdjustAdRevenueAppOpen(appOpenResume);
			if (mrec != null) mrec = new AdjustAdRevenueMRec(mrec);
			if (native != null) native = new AdjustAdRevenueNative(native);
			if (nativeInter != null) nativeInter = new AdjustAdRevenueNative(nativeInter);
			if (nativeCollap != null) nativeCollap = new AdjustAdRevenueNative(nativeCollap);
#endif

#if APPSFLYER_ANALYTICS
			if (banner != null) banner = new AppsFlyerAdRevenueBanner(banner);
			if (inter != null) inter = new AppsFlyerAdRevenueInterstitial(inter);
			if (interImage != null) interImage = new AppsFlyerAdRevenueInterstitial(interImage);
			if (reward != null) reward = new AppsFlyerAdRevenueRewardsVideo(reward);
			if (appOpen != null) appOpen = new AppsFlyerAdRevenueAppOpen(appOpen);
			if (appOpenResume != null) appOpenResume = new AppsFlyerAdRevenueAppOpen(appOpenResume);
			if (mrec != null) mrec = new AppsFlyerAdRevenueMRec(mrec);
			if (native != null) native = new AppsFlyerAdRevenueNative(native);
			if (nativeInter != null) nativeInter = new AppsFlyerAdRevenueNative(nativeInter);
			if (nativeCollap != null) nativeCollap = new AppsFlyerAdRevenueNative(nativeCollap);
#endif

			if (banner != null) adapter.SetBanner(banner);
			if (inter != null) adapter.SetInterstitial(inter);
			if (interImage != null) adapter.SetInterstitialImage(interImage);
			if (reward != null) adapter.SetRewardVideo(reward);
			if (appOpen != null) adapter.SetAppOpen(appOpen);
			if (appOpenResume != null) adapter.SetAppOpenResume(appOpenResume);
			if (mrec != null) adapter.SetMRec(mrec);
			if (native != null) adapter.SetNative(native);
			if (nativeInter != null) adapter.SetNativeInter(nativeInter);
			if (nativeCollap != null) adapter.SetNativeCollapsible(nativeCollap);

			if (isMain) 
			{
				Ad?.Dispose();
				Ad = adapter;
			}
			else 
			{
				AdBackFill?.Dispose();
				AdBackFill = adapter;
			}

			adapter.Initialize(isInitialized =>
			{
				if (isInitialized)
				{
					if (isMain)
					{
						Ad.Banner?.Load();
						Ad.Interstitial?.Load();
						Ad.InterstitialImage?.Load();
						Ad.RewardVideo?.Load();
						Ad.AppOpen?.Load();
						Ad.AppOpenResume?.Load();
						Ad.MRec?.Load();
						Ad.Native?.Load();
						Ad.NativeInter?.Load();
						Ad.NativeCollapsible?.Load();
					}
					else
					{
						AdBackFill.Banner?.Load();
						AdBackFill.Interstitial?.Load();
						AdBackFill.InterstitialImage?.Load();
						AdBackFill.RewardVideo?.Load();
						AdBackFill.AppOpen?.Load();
						AdBackFill.AppOpenResume?.Load();
						AdBackFill.MRec?.Load();
						AdBackFill.Native?.Load();
						AdBackFill.NativeInter?.Load();
						AdBackFill.NativeCollapsible?.Load();
					}
				}
			});
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
