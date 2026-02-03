using System;
using com.ktgame.core;
using Sirenix.OdinInspector;

namespace com.ktgame.services.ads
{
	[Serializable]
	public class AdRevenueProviderSetting
	{
		[EnumPaging] public AnalyticsProvider Provider;

		[EnumPaging] public IAAFormatType Formats;

		[ShowIf("@Formats.HasFlag(IAAFormatType.Banner)")] public RevenueBannerData Banner;

		[ShowIf("@Formats.HasFlag(IAAFormatType.Interstitial)")] public RevenueInterstitialData Interstitial;

		[ShowIf("@Formats.HasFlag(IAAFormatType.Reward)")] public RevenueRewardVideoData RewardVideo;

		[ShowIf("@Formats.HasFlag(IAAFormatType.Aoa)")] public RevenueAppOpenData AppOpen;

		[ShowIf("@Formats.HasFlag(IAAFormatType.Native)")] public RevenueNativeData Native;
	}
}