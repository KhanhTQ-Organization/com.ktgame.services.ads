using com.ktgame.ads.core;
using UnityEngine;

namespace com.ktgame.services.ads.appsflyer_ad_revenue
{
	public class AppsFlyerAdRevenueInterstitial : InterstitialDecorator
	{
		private RevenueAdSetting _settings;

		public AppsFlyerAdRevenueInterstitial(IInterstitialAdapter adapter) : base(adapter)
		{
			_settings = RevenueAdSetting.Instance;
		}
		
		protected override void AdRevenuePaidHandler(ImpressionData impressionData)
		{
			base.AdRevenuePaidHandler(impressionData);
			AppsFlyerMeasureAdRevenue.LogAdRevenueEvent(impressionData);
		}
		
		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerInterstitialTracking.EventLoadFailed, null);
		}

		protected override void LoadSucceededHandler()
		{
			base.LoadSucceededHandler();
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerInterstitialTracking.EventLoadSucceeded, null);
		}
		
		protected override void ClickHandler(AdPlacement adPlacement)
		{
			base.ClickHandler(adPlacement);
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerInterstitialTracking.EventClicked, null);
		}

		protected override void ShowSucceededHandler(AdPlacement adPlacement)
		{
			base.ShowSucceededHandler(adPlacement);
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerInterstitialTracking.EventShowSucceeded, null);
		}

		protected override void ShowFailedHandler(AdError adError)
		{
			base.ShowFailedHandler(adError);
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerInterstitialTracking.EventShowFailed, null);
		}
	}
}
