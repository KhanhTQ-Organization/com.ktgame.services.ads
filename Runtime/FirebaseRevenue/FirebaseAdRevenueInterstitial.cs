using com.ktgame.ads.core;

namespace com.ktgame.services.ads.firebase_ad_revenue
{
    public class FirebaseAdRevenueInterstitial : InterstitialDecorator
    {
        public FirebaseAdRevenueInterstitial(IInterstitialAdapter adapter) : base(adapter) { }

        protected override void ImpressionSuccessHandler(ImpressionData impressionData)
        {
            base.ImpressionSuccessHandler(impressionData);
            FirebaseMeasureAdRevenue.LogAdRevenueEvent(impressionData);
        }
    }
}
