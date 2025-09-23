using com.ktgame.ads.core;

namespace com.ktgame.services.ads.appsflyer_ad_revenue
{
	public class AppsFlyerAdRevenueBanner : BannerDecorator
	{
		private RevenueAdSetting _settings;

		public AppsFlyerAdRevenueBanner(IBannerAdapter adapter) : base(adapter)
		{
			_settings = RevenueAdSetting.Instance;
		}
        
		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);
			AppsFlyerMeasureAdRevenue.LogAdRevenueEvent(impressionData);
		}
        
		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);
			AppsFlyerMeasureAdRevenue.SendAdEvent(string.Empty, null);
		}
	}
}