using System.Collections.Generic;
using com.ktgame.ads.core;
using com.ktgame.core;

namespace com.ktgame.services.ads.firebase_ad_revenue
{
	public class FirebaseAdRevenueInterstitial : InterstitialDecorator
	{
		private readonly RevenueInterstitialData? _interstitialConfig;

		public FirebaseAdRevenueInterstitial(IInterstitialAdapter adapter) : base(adapter)
		{
			_interstitialConfig = RevenueAdSetting.Instance.GetInterstitial(AnalyticsProvider.Firebase);
		}

		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);

			if (_interstitialConfig != null)
			{
				FirebaseMeasureAdRevenue.LogAdRevenueEvent(impressionData);
			}
		}

		protected override void LoadSucceededHandler()
		{
			base.LoadSucceededHandler();

			if (_interstitialConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventLoadSucceeded, null);
			}
		}

		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);

			if (_interstitialConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventLoadFailed,
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
				FirebaseMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventClicked, null);
			}
		}

		protected override void ShowSucceededHandler(AdPlacement adPlacement)
		{
			base.ShowSucceededHandler(adPlacement);

			if (_interstitialConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventShowSucceeded,
					null);
			}
		}

		protected override void ShowFailedHandler(AdError adError)
		{
			base.ShowFailedHandler(adError);

			if (_interstitialConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_interstitialConfig?.EventShowFailed,
					new Dictionary<string, string>
					{
						{ "errormsg", adError.Message }
					});
			}
		}
	}
}
