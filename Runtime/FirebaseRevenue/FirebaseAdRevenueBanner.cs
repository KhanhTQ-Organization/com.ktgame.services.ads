using com.ktgame.ads.core;

namespace com.ktgame.services.ads.firebase_ad_revenue
{
    public class FirebaseAdRevenueBanner : BannerDecorator
    {
        public FirebaseAdRevenueBanner(IBannerAdapter adapter) : base(adapter) { }

        protected override void ImpressionSuccessHandler(ImpressionData impressionData)
        {
            base.ImpressionSuccessHandler(impressionData);
            FirebaseMeasureAdRevenue.LogAdRevenueEvent(impressionData);
        }
    }
}
