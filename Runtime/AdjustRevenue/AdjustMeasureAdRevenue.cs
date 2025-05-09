using UnityEngine;
using com.ktgame.ads.core;

#if ADJUST_ANALYTICS
using com.adjust.sdk;
#endif

namespace com.ktgame.services.ads.adjust_ad_revenue
{
    internal static class AdjustMeasureAdRevenue
    {
        internal static void LogAdRevenueEvent(ImpressionData impressionData)
        {
#if ADJUST_ANALYTICS
            var adjustAdRevenue = new AdjustAdRevenue(ToAdRevenueSource(impressionData.AdPlatform));
            adjustAdRevenue.setRevenue(impressionData.Revenue, impressionData.Currency);
            adjustAdRevenue.setAdRevenueNetwork(impressionData.AdNetwork);
            adjustAdRevenue.setAdRevenueUnit(impressionData.AdUnit);
            adjustAdRevenue.setAdRevenuePlacement(impressionData.AdPlacement);
            Adjust.trackAdRevenue(adjustAdRevenue);
            Debug.Log($"[AdjustMeasureAdRevenue]: {impressionData.ToString()}");
#endif
        }

#if ADJUST_ANALYTICS
        private static string ToAdRevenueSource(AdPlatform adPlatform)
        {
            switch (adPlatform)
            {
                case AdPlatform.Max: return AdjustConfig.AdjustAdRevenueSourceAppLovinMAX;
                case AdPlatform.Admob: return AdjustConfig.AdjustAdRevenueSourceAdMob;
                case AdPlatform.IronSource: return AdjustConfig.AdjustAdRevenueSourceIronSource;
                default: return string.Empty;
            }
        }
#endif
    }
}