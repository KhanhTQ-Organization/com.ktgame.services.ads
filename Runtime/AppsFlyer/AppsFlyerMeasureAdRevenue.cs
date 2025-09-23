using System;
using System.Collections.Generic;
using com.ktgame.ads.core;
using UnityEngine;

#if APPSFLYER_ANALYTICS
using AppsFlyerSDK;
#endif

namespace com.ktgame.services.ads.appsflyer_ad_revenue
{
	internal static class AppsFlyerMeasureAdRevenue
	{
		internal static void LogAdRevenueEvent(ImpressionData impressionData)
		{
#if APPSFLYER_ANALYTICS
			try
			{
				var additionalParams = new Dictionary<string, string>
				{
					{ AdRevenueScheme.COUNTRY, Application.systemLanguage.ToString() },
					{ AdRevenueScheme.AD_UNIT, impressionData.AdUnit },
					{ AdRevenueScheme.AD_TYPE, impressionData.AdPlatform.ToString() },
					{ AdRevenueScheme.PLACEMENT, impressionData.AdPlacement }
				};

				var revenueData = new AFAdRevenueData(
					impressionData.AdNetwork,
					ToMediationNetwork(impressionData.AdPlatform),
					impressionData.Currency,
					impressionData.Revenue
				);
				
				AppsFlyer.logAdRevenue(revenueData, additionalParams);
			}
			catch (Exception ex)
			{
				Debug.LogError($"[AppsFlyerMeasureAdRevenue] Error: {ex}");
			}
#endif
		}

		internal static void SendAdEvent(string eventName, Dictionary<string, string> additionalParams)
		{
#if APPSFLYER_ANALYTICS
			if (!string.IsNullOrEmpty(eventName))
			{
				Debug.Log("[Appsflyer] LogEvent: " + eventName);

				AppsFlyer.sendEvent(eventName, additionalParams);
			}
#endif
		}

#if APPSFLYER_ANALYTICS
		private static MediationNetwork ToMediationNetwork(AdPlatform adPlatform)
		{
			switch (adPlatform)
			{
				case AdPlatform.Max:
					return MediationNetwork.ApplovinMax;
				case AdPlatform.Admob:
					return MediationNetwork.GoogleAdMob;
				case AdPlatform.IronSource:
					return MediationNetwork.IronSource;
				default:
					return MediationNetwork.Custom;
			}
		}
#endif
	}
}