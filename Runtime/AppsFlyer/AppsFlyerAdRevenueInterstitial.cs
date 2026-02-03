using System.Collections.Generic;
using com.ktgame.ads.core;
using com.ktgame.core;

namespace com.ktgame.services.ads.appsflyer_ad_revenue
{
	public class AppsFlyerAdRevenueInterstitial : InterstitialDecorator
	{
		private readonly RevenueInterstitialData? _interstitialConfig;

		public AppsFlyerAdRevenueInterstitial(IInterstitialAdapter adapter) : base(adapter)
		{
			_interstitialConfig = RevenueAdSetting.Instance.GetInterstitial(AnalyticsProvider.AppsFlyer);
		}

		protected override void AdRevenuePaidHandler(ImpressionData impressionData)
		{
			base.AdRevenuePaidHandler(impressionData);

			if (_interstitialConfig != null)
			{
				AppsFlyerMeasureAdRevenue.LogAdRevenueEvent(impressionData);
			}
		}

		protected override void LoadSucceededHandler()
		{
			base.LoadSucceededHandler();

			if (_interstitialConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventLoadSucceeded, null);
			}
		}

		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);

			if (_interstitialConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventLoadFailed,
					new Dictionary<string, string>
					{
						{ "errormsg", adError.Message }
					});
			}
		}

		protected override void ClickHandler(AdPlacement adPlacement)
		{
			base.ClickHandler(adPlacement);

			if (_interstitialConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventClicked, null);
			}
		}

		protected override void ShowSucceededHandler(AdPlacement adPlacement)
		{
			base.ShowSucceededHandler(adPlacement);

			if (_interstitialConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventShowSucceeded, null);
			}
		}

		protected override void ShowFailedHandler(AdError adError)
		{
			base.ShowFailedHandler(adError);

			if (_interstitialConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventShowFailed,
					new Dictionary<string, string>
					{
						{ "errormsg", adError.Message }
					});
			}
		}
	}
}