using com.ktgame.ads.core;
using UnityEngine;

namespace com.ktgame.services.ads.adjust_ad_revenue
{
	public class AdjustAdRevenueAppOpen : AppOpenDecorator
	{
		public AdjustAdRevenueAppOpen(IAppOpenAdapter adapter) : base(adapter) { }
        
		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);
			AdjustMeasureAdRevenue.LogAdRevenueEvent(impressionData);
		}
	}
}