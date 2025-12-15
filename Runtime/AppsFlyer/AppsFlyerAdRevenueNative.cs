using com.ktgame.ads.core;

namespace com.ktgame.services.ads.appsflyer_ad_revenue
{
	public class AppsFlyerAdRevenueNative : NativeDecorator
	{
		private RevenueAdSetting _settings;

		public AppsFlyerAdRevenueNative(INativeAdapter adapter) : base(adapter)
		{
			_settings = RevenueAdSetting.Instance;
		}
        
		protected override void PaidHandler(ImpressionData impressionData)
		{
			base.PaidHandler(impressionData);
			AppsFlyerMeasureAdRevenue.LogAdRevenueEvent(impressionData);
		}
		
		protected override void LoadSucceededHandler(AdPlacement AdPlacement)
		{
			base.LoadSucceededHandler(AdPlacement);
		}
	}
}
