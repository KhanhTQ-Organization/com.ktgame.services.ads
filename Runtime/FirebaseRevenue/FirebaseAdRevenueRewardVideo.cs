using System.Collections.Generic;
using com.ktgame.ads.core;

namespace com.ktgame.services.ads.firebase_ad_revenue
{
	public class FirebaseAdRevenueRewardVideo : RewardVideoDecorator
	{
		private RevenueAdSetting _settings;

		public FirebaseAdRevenueRewardVideo(IRewardVideoAdapter adapter) : base(adapter)
		{
			_settings = RevenueAdSetting.Instance;
		}

		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);
			FirebaseMeasureAdRevenue.LogAdRevenueEvent(impressionData);
		}

		protected override void LoadSucceededHandler()
		{
			base.LoadSucceededHandler();
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseRewardVideoTracking.EventLoadSucceeded, null);
		}

		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseRewardVideoTracking.EventLoadFailed,
				new Dictionary<string, string>
				{
					{ "errormsg", adError.Message }
				});
		}

		protected override void RewardHandler(AdPlacement rewardData)
		{
			base.RewardHandler(rewardData);
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseRewardVideoTracking.EventRewarded,
				new Dictionary<string, string>
				{
					{ "placement", rewardData.Location }
				});
		}

		protected override void VideoClosedHandler()
		{
			base.VideoClosedHandler();
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseRewardVideoTracking.EventClosed, null);
		}

		protected override void VideoOpenedHandler()
		{
			base.VideoOpenedHandler();
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseRewardVideoTracking.EventVideoOpened, null);
		}

		protected override void ShowFailedHandler(AdError error)
		{
			base.ShowFailedHandler(error);
			FirebaseMeasureAdRevenue.SendAdEvent(_settings.FirebaseRewardVideoTracking.EventShowFailed,
				new Dictionary<string, string>
				{
					{ "errormsg", error.Message }
				});
		}
	}
}