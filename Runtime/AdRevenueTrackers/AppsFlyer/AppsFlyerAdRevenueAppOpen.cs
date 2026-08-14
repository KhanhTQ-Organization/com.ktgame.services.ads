using com.ktgame.ads.core;

namespace com.ktgame.services.ads.appsflyer_ad_revenue
{
	public class AppsFlyerAdRevenueAppOpen : AppOpenDecorator
	{
		public AppsFlyerAdRevenueAppOpen(IAppOpenAdapter adapter) : base(adapter) { }

		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);
			AppsFlyerMeasureAdRevenue.LogAdRevenueEvent(impressionData);
		}
	}
}