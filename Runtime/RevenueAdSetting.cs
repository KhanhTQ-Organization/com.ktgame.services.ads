using System.Collections.Generic;
using System.Linq;
using com.ktgame.core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.ktgame.services.ads
{
	public class RevenueAdSetting : ServiceSettingsSingleton<RevenueAdSetting>
	{
		public override string PackageName => GetType().Namespace;
		public IReadOnlyList<AdRevenueProviderSetting> Providers => _providers;
		
		[Title("Revenue Providers")]
		[ListDrawerSettings(Expanded = true)]
		[SerializeField]
		private List<AdRevenueProviderSetting> _providers;

		// ================== API ==================

		public AdRevenueProviderSetting GetProvider(AnalyticsProvider provider)
		{
			return _providers.FirstOrDefault(p => p.Provider == provider);
		}

		public RevenueBannerData? GetBanner(AnalyticsProvider provider) => GetProvider(provider)?.Banner;

		public RevenueInterstitialData? GetInterstitial(AnalyticsProvider provider) => GetProvider(provider)?.Interstitial;

		public RevenueRewardVideoData? GetRewardVideo(AnalyticsProvider provider) => GetProvider(provider)?.RewardVideo;

		public RevenueAppOpenData GetAppOpen(AnalyticsProvider provider) => GetProvider(provider)?.AppOpen;

		public RevenueNativeData GetNative(AnalyticsProvider provider) => GetProvider(provider)?.Native;
	}
}
