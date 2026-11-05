using System;
using com.ktgame.ads.core;
using com.ktgame.ads.core.extensions;
using com.ktgame.core.di;
using com.ktgame.services.remote_config;

#if MAX_APPLOVIN
using com.ktgame.ads.max_applovin;
#endif

#if ADMOB
using com.ktgame.ads.admob;
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
		private const string LogTag = "[AdService]";

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
		public bool AdsStarted => _adsStarted;

		private AdServiceSettings _settings;
		private IAASettingSO _iaaSettingSo;
		private bool _adsStarted;
		private AdConsentManager _consent;

		public async UniTask OnInitialize(IArchitecture architecture)
		{
			_settings = AdServiceSettings.Instance;
			_iaaSettingSo = IAASettingSO.Instance;
#if PRODUCTION && COLLAPSIBLE
			_gmaNativePopup ??= Game.Instance.GetService<IGmaNativeInterstitial>();
			_gmaNativeCollapsible  ??= Game.Instance.GetService<IGmaNativeCollapsible>();
			_mRecCollapsibleAds  ??= Game.Instance.GetService<IMRecCollapsibleAds>();
#endif
			
			_consent = new AdConsentManager(_settings);
			_consent.OnConsentChanged += TryStartAds;
			var consentTask = _consent.GatherAsync();

			await UniTask.DelaySeconds(1f);
			await SetMainAds();
			await SetBackFillAds();

			Initialized = true;

			StartAdsWhenAllowedAsync(consentTask).Forget();
		}

		#region Unit id resolution (P0.3)

		private static string GetUnitId(string newId, string oldId)
		{
			return string.IsNullOrWhiteSpace(newId) ? oldId?.Trim() : newId.Trim();
		}
		
		private string GetRemoteOrLocalUnitId(string remoteKey, string newId, string oldId)
		{
			string remoteId = null;
			try
			{
				remoteId = _remoteConfigService?.GetValue(remoteKey)?.String;
			}
			catch (Exception e)
			{
				Debug.LogWarning($"{LogTag} Remote config '{remoteKey}' unreadable, using local id. {e.Message}");
			}

			if (!string.IsNullOrWhiteSpace(remoteId))
			{
				return remoteId.Trim();
			}

			return GetUnitId(newId, oldId);
		}

		#endregion

		#region Build adapters

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
					await SetAdsAdMob(false);
					break;
			}
		}

		private async UniTask SetNullAds(bool isMain)
		{
			IAdAdapter adapter = new NullAdAdapter();
			ApplyDecorators(adapter, null, null, null, null, null, null, null, null, null, null, isMain);
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

			ApplyDecorators(adapter, bannerAdapter, interstitialAdapter, null, rewardVideoAdapter, appOpenAdapter, null, mRecAdapter, null, null, null, isMain);
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

			var interImageId = GetRemoteOrLocalUnitId(AdRemoteConfigKeys.InterImageAdId,
				_iaaSettingSo.GmaAndroid.InterstitialImageUnitID, _settings.AndroidAdmobInterstitialImageUnitId);
			if (!string.IsNullOrEmpty(interImageId)) interstitialImageAdapter = new AdMobInterstitial(interImageId);

			var rewardId = GetUnitId(_iaaSettingSo.GmaAndroid.RewardUnitID, _settings.AndroidAdmobRewardedVideoUnitId);
			if (!string.IsNullOrEmpty(rewardId)) rewardVideoAdapter = new AdmobRewardVideo(rewardId);

			var aoaId = GetRemoteOrLocalUnitId(AdRemoteConfigKeys.OpenAdId,
				_iaaSettingSo.GmaAndroid.AoaUnitID, _settings.AndroidAdmobAppOpenUnitId);
			if (!string.IsNullOrEmpty(aoaId)) appOpenAdapter = new AdMobAppOpen(aoaId);

			var aoaResumeId = GetRemoteOrLocalUnitId(AdRemoteConfigKeys.OpenAdResumeId,
				_iaaSettingSo.GmaAndroid.AoaResumeUnitID, _settings.AndroidAdmobAppOpenResumeUnitId);
			if (!string.IsNullOrEmpty(aoaResumeId)) appOpenResumeAdapter = new AdMobAppOpen(aoaResumeId);

#if ADMOB_NATIVE
			var nativeId = GetRemoteOrLocalUnitId(AdRemoteConfigKeys.NativeAdId,
				_iaaSettingSo.GmaAndroid.NativeUnitID, _settings.AndroidAdmobNativeUnitId);
			if (!string.IsNullOrEmpty(nativeId)) nativeAdapter = new AdMobNative(nativeId, AdFormat.Native);

			var nativeInterId = GetRemoteOrLocalUnitId(AdRemoteConfigKeys.NativeInterstitialAdId,
				_iaaSettingSo.GmaAndroid.NativeInterstitialUnitID, _settings.AndroidAdmobNativeInterUnitId);
			if (!string.IsNullOrEmpty(nativeInterId))
			{
#if COLLAPSIBLE
				// Game-side native popup implementation owns this unit.
				_gmaNativePopup?.Initialize(nativeInterId);
#else
				nativeInterAdapter = new AdMobNative(nativeInterId, AdFormat.NativeInter);
#endif
			}

#if COLLAPSIBLE
			var nativeCollapId = GetRemoteOrLocalUnitId(AdRemoteConfigKeys.NativeCollapsibleAdId,
				_iaaSettingSo.GmaAndroid.NativeCollapsibleUnitID, _settings.AndroidAdmobNativeCollapsibleUnitId);
			if (!string.IsNullOrEmpty(nativeCollapId))
			{
				_gmaNativeCollapsible?.Initialize(nativeCollapId);
			}
#endif
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

			ApplyDecorators(adapter, bannerAdapter, interstitialAdapter, interstitialImageAdapter, rewardVideoAdapter, appOpenAdapter, appOpenResumeAdapter, mRecAdapter, nativeAdapter, nativeInterAdapter, nativeCollapAdapter, isMain);
			await UniTask.CompletedTask;
		}

		private void ApplyDecorators(
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
			if (RevenueAdSetting.Instance.ActiveProviders.HasFlag(AnalyticsProvider.Firebase))
			{
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
			}
#endif

#if ADJUST_ANALYTICS
			if (RevenueAdSetting.Instance.ActiveProviders.HasFlag(AnalyticsProvider.Adjust))
			{
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
			}
#endif

#if APPSFLYER_ANALYTICS
			if (RevenueAdSetting.Instance.ActiveProviders.HasFlag(AnalyticsProvider.AppsFlyer))
			{
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
			}
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
		}

		#endregion

		#region Start SDKs (only after consent)

		private async UniTaskVoid StartAdsWhenAllowedAsync(UniTask consentTask)
		{
			try
			{
				await consentTask;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			TryStartAds();
		}

		private void TryStartAds()
		{
			if (_adsStarted || !Initialized)
			{
				return;
			}

			if (!CanRequestAds)
			{
				Debug.LogWarning($"{LogTag} Consent does not allow ad requests yet. Ad SDKs NOT started; will retry on resume / after privacy form.");
				return;
			}

			_adsStarted = true;
			Debug.Log($"{LogTag} Consent OK -> starting ad SDKs (main: {_iaaSettingSo.MainMediation}, backfill: {_iaaSettingSo.BackfillMediation}).");

			StartAdapter(Ad);
			if (AdBackFill != null && !ReferenceEquals(AdBackFill, Ad))
			{
				StartAdapter(AdBackFill);
			}
		}

		private static void StartAdapter(IAdAdapter adapter)
		{
			if (adapter == null)
			{
				return;
			}

			adapter.Initialize(isInitialized =>
			{
				if (!isInitialized)
				{
					return;
				}
				
				adapter.Banner?.Load();
				adapter.Interstitial?.Load();
				adapter.InterstitialImage?.Load();
				adapter.RewardVideo?.Load();
				adapter.AppOpen?.Load();
				adapter.AppOpenResume?.Load();
				adapter.MRec?.Load();
				adapter.Native?.Load();
				adapter.NativeInter?.Load();
				adapter.NativeCollapsible?.Load();
			});
		}

		private void OnApplicationPause(bool paused)
		{
			if (paused || _adsStarted || !Initialized || _consent == null || _consent.InProgress)
			{
				return;
			}

			RetryConsentAsync().Forget();
		}

		private async UniTaskVoid RetryConsentAsync()
		{
			await _consent.GatherAsync();
			TryStartAds();
		}

		#endregion

		#region Consent (delegated to AdConsentManager)

		public bool CanRequestAds => _consent != null && _consent.CanRequestAds;

		public bool IsPrivacyOptionsRequired => _consent != null && _consent.IsPrivacyOptionsRequired;

		public void ShowPrivacyOptionsForm(Action<string> onComplete = null)
		{
			if (_consent == null)
			{
				onComplete?.Invoke("AdService not initialized");
				return;
			}

			_consent.ShowPrivacyOptionsForm(onComplete);
		}

		public void ResetConsentForDebug()
		{
			_consent?.ResetForDebug();
		}

		#endregion

		public void SetPause(bool pause)
		{
			Ad?.SetPause(pause);
		}
	}
}
