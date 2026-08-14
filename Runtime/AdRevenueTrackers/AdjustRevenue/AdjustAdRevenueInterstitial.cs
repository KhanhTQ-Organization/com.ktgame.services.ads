using com.ktgame.ads.core;

namespace com.ktgame.services.ads.adjust_ad_revenue
{
    public class AdjustAdRevenueInterstitial : InterstitialDecorator
    {
        public AdjustAdRevenueInterstitial(IInterstitialAdapter adapter) : base(adapter) { }

        protected override void ImpressionSuccessHandler(ImpressionData impressionData)
        {
            base.ImpressionSuccessHandler(impressionData);
            AdjustMeasureAdRevenue.LogAdRevenueEvent(impressionData);
        }
    }
}
