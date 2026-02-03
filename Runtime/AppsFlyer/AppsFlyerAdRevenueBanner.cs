using System.Collections.Generic;
using com.ktgame.ads.core;
using com.ktgame.core;

namespace com.ktgame.services.ads.appsflyer_ad_revenue
{
	public class AppsFlyerAdRevenueBanner : BannerDecorator
	{
		private readonly RevenueBannerData? _bannerConfig;

		public AppsFlyerAdRevenueBanner(IBannerAdapter adapter)
			: base(adapter)
		{
			_bannerConfig = RevenueAdSetting.Instance.GetBanner(AnalyticsProvider.AppsFlyer);
		}

		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);

			if (_bannerConfig != null)
			{
				AppsFlyerMeasureAdRevenue.LogAdRevenueEvent(impressionData);
				AppsFlyerMeasureAdRevenue.SendAdEvent(_bannerConfig?.EventImpressionSuccess, null);
			}
		}

		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);

			if (_bannerConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_bannerConfig?.EventLoadFailed,
					new Dictionary<string, string>
					{
						{ "errormsg", adError.Message }
					});
			}
		}
	}
}
