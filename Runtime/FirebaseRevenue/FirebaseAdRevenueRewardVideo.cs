using System.Collections.Generic;
using com.ktgame.ads.core;
using com.ktgame.core;

namespace com.ktgame.services.ads.firebase_ad_revenue
{
	public class FirebaseAdRevenueRewardVideo : RewardVideoDecorator
	{
		private readonly RevenueRewardVideoData? _rewardVideoConfig;

		public FirebaseAdRevenueRewardVideo(IRewardVideoAdapter adapter) : base(adapter)
		{
			_rewardVideoConfig = RevenueAdSetting.Instance.GetRewardVideo(AnalyticsProvider.Firebase);
		}

		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);
			if (_rewardVideoConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventLoadSucceeded, null);
			}
		}

		protected override void LoadSucceededHandler()
		{
			base.LoadSucceededHandler();
			if (_rewardVideoConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventLoadSucceeded, null);
			}
		}

		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);
			if (_rewardVideoConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventLoadFailed,
					new Dictionary<string, string>
					{
						{ "errormsg", adError.Message }
					});
			}
		}

		protected override void RewardHandler(AdPlacement rewardData)
		{
			base.RewardHandler(rewardData);
			if (_rewardVideoConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventRewarded,
					new Dictionary<string, string>
					{
						{ "placement", rewardData.Location }
					});
			}
		}

		protected override void VideoClosedHandler()
		{
			base.VideoClosedHandler();
			if (_rewardVideoConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventClosed, null);
			}
		}

		protected override void VideoOpenedHandler()
		{
			base.VideoOpenedHandler();
			if (_rewardVideoConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventVideoOpened, null);
			}
		}

		protected override void ShowFailedHandler(AdError error)
		{
			base.ShowFailedHandler(error);
			if (_rewardVideoConfig != null)
			{
				FirebaseMeasureAdRevenue.SendAdEvent(
					_rewardVideoConfig?.EventShowFailed, new Dictionary<string, string>
					{
						{ "errormsg", error.Message }
					});
			}
		}
	}
}