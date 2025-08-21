using com.ktgame.ads.core;
#if FIREBASE_ANALYTICS
using Firebase.Analytics;
#endif

namespace com.ktgame.services.ads.firebase_ad_revenue
{
    internal static class FirebaseMeasureAdRevenue
    {
        private const string AdImpression = "ad_impression";
        private const string AdPlatform = "ad_platform";
        private const string AdSource = "ad_source";
        private const string AdUnitName = "ad_unit_name";
        private const string AdFormat = "ad_format";
        private const string AdPlacement = "ad_placement";
        private const string Currency = "currency";
        private const string Value = "value";

        internal static void LogAdRevenueEvent(ImpressionData impressionData)
        {
#if FIREBASE_ANALYTICS
            var adParameters = new Parameter[]
            {
                new Parameter(AdPlatform, ToAdRevenueSource(impressionData.AdPlatform)),
                new Parameter(AdSource, impressionData.AdNetwork),
                new Parameter(AdUnitName, impressionData.AdUnit),
                new Parameter(AdFormat, impressionData.AdFormat.ToString()),
                new Parameter(AdPlacement, impressionData.AdPlacement),
                new Parameter(Currency, impressionData.Currency),
                new Parameter(Value, impressionData.Revenue)
            };
            FirebaseAnalytics.LogEvent(AdImpression, adParameters);
#endif
        }

#if FIREBASE_ANALYTICS
        private static string ToAdRevenueSource(AdPlatform adPlatform)
        {
            switch (adPlatform)
            {
                case com.ktgame.ads.core.AdPlatform.Max: return "AppLovin";
                case com.ktgame.ads.core.AdPlatform.IronSource: return "ironSource";
                case com.ktgame.ads.core.AdPlatform.Admob: return "AdMob";
                default: return string.Empty;
            }
        }
#endif
    }
}