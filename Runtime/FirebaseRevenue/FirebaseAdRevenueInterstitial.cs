using System.Collections.Generic;
using com.ktgame.ads.core;

namespace com.ktgame.services.ads.firebase_ad_revenue
{
	public class FirebaseAdRevenueInterstitial : InterstitialDecorator
	{
		private RevenueAdSetting _settings;

		public FirebaseAdRevenueInterstitial(IInterstitialAdapter adapter) : base(adapter)
		{
			_settings = RevenueAdSetting.Instance;
		}

		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);
			FirebaseMeasureAdRevenue.LogAdRevenueEvent(impressionData);
		}

		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseInterstitialTracking.EventLoadFailed,
				new Dictionary<string, string>
				{
					{ "errormsg", adError.Message }
				});
		}

		protected override void LoadSucceededHandler()
		{
			base.LoadSucceededHandler();
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseInterstitialTracking.EventLoadSucceeded, null);
		}

		protected override void ClickHandler(AdPlacement adPlacement)
		{
			base.ClickHandler(adPlacement);
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseInterstitialTracking.EventClicked, null);
		}

		protected override void ShowSucceededHandler(AdPlacement adPlacement)
		{
			base.ShowSucceededHandler(adPlacement);
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseInterstitialTracking.EventShowSucceeded, null);
		}

		protected override void ShowFailedHandler(AdError adError)
		{
			base.ShowFailedHandler(adError);
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseInterstitialTracking.EventShowFailed,
				new Dictionary<string, string>
				{
					{ "errormsg", adError.Message }
				});
		}
	}
}