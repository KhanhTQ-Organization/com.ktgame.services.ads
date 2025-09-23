using com.ktgame.core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.ktgame.services.ads
{
	public class RevenueAdSetting : ServiceSettingsSingleton<RevenueAdSetting>
	{
		public override string PackageName => GetType().Namespace;

		[SerializeField] [TabGroup("Adjust")] [Title("Adjust Banner Tracking", bold: true)]
		private RevenueBannerData _adjustBannerTracking;

		[SerializeField] [TabGroup("Adjust")] [Title("Adjust Interstitial Tracking", bold: true)]
		private RevenueInterstitialData _adjustInterstitialTracking;

		[SerializeField] [TabGroup("Firebase")] [Title("Firebase Banner Tracking", bold: true)]
		private RevenueBannerData _firebaseBannerTracking;

		[SerializeField] [TabGroup("Firebase")] [Title("Firebase Interstitial Tracking", bold: true)]
		private RevenueInterstitialData _firebaseInterstitialTracking;

		[SerializeField] [TabGroup("Firebase")] [Title("Firebase RewardVideo Tracking", bold: true)]
		private RevenueRewardVideoData _firebaseRewardVideoTracking;

		[SerializeField] [TabGroup("AppsFlyer")] [Title("AppsFlyer RewardVideo Tracking", bold: true)]
		private RevenueRewardVideoData _appsFlyerRewardVideoTracking;

		[SerializeField] [TabGroup("AppsFlyer")] [Title("AppsFlyer Interstitial Tracking", bold: true)]
		private RevenueInterstitialData _appsFlyerInterstitialTracking;

		public RevenueRewardVideoData AppsFlyerRewardVideoTracking => _appsFlyerRewardVideoTracking;
		public RevenueInterstitialData AppsFlyerInterstitialTracking => _appsFlyerInterstitialTracking;
		public RevenueInterstitialData FirebaseInterstitialTracking => _firebaseInterstitialTracking;
		public RevenueRewardVideoData FirebaseRewardVideoTracking => _firebaseRewardVideoTracking;
	}
}