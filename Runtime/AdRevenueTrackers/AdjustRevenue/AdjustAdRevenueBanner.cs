using com.ktgame.ads.core;

namespace com.ktgame.services.ads.adjust_ad_revenue
{
    public class AdjustAdRevenueBanner : BannerDecorator
    {
        public AdjustAdRevenueBanner(IBannerAdapter adapter) : base(adapter) { }

        protected override void ImpressionSuccessHandler(ImpressionData impressionData)
        {
            base.ImpressionSuccessHandler(impressionData);
            AdjustMeasureAdRevenue.LogAdRevenueEvent(impressionData);
        }
    }
}
