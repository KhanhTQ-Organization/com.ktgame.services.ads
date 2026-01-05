using UnityEngine;
using com.ktgame.ads.core;

#if ADJUST_ANALYTICS
using AdjustSdk;
#endif

namespace com.ktgame.services.ads.adjust_ad_revenue
{
	internal static class AdjustMeasureAdRevenue
	{
		internal static void LogAdRevenueEvent(ImpressionData impressionData)
		{
#if ADJUST_ANALYTICS
            var adRevenue = new AdjustAdRevenue(ToAdRevenueSource(impressionData.AdPlatform));
            adRevenue.SetRevenue(impressionData.Revenue, "USD");
            adRevenue.AdRevenueNetwork = impressionData.AdNetwork;
            adRevenue.AdRevenueUnit = impressionData.AdUnit;
            adRevenue.AdRevenuePlacement = impressionData.AdPlacement;
            //Debug.Log($"[AdjustAdTrackingEvent] LogRevenueEvent : {impression.AdFlatform} | {impression.AdSource} | {impression.AdUnitId} | {impression.AdPlacement} | {impression.AdValue}");
            Adjust.TrackAdRevenue(adRevenue);
            Debug.Log($"[AdjustMeasureAdRevenue]: {impressionData.ToString()}");
#endif
		}

#if ADJUST_ANALYTICS
        private static string ToAdRevenueSource(AdPlatform adPlatform)
        {
            switch (adPlatform)
            {
                case AdPlatform.Admob: return "admob_sdk";
                case AdPlatform.IronSource: return "ironsource_sdk";
                case AdPlatform.Max: return "applovin_max_sdk";
                default: return "other";
            }
        }
#endif
	}
}