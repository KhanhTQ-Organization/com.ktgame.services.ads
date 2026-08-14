using System.Collections.Generic;
using com.ktgame.ads.core;
using com.ktgame.core;

namespace com.ktgame.services.ads.appsflyer_ad_revenue
{
	public class AppsFlyerAdRevenueRewardsVideo : RewardVideoDecorator
	{
		private readonly RevenueRewardVideoData? _rewardVideoConfig;

		public AppsFlyerAdRevenueRewardsVideo(IRewardVideoAdapter adapter) : base(adapter)
		{
			_rewardVideoConfig = RevenueAdSetting.Instance.GetRewardVideo(AnalyticsProvider.AppsFlyer);
		}

		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);

			if (_rewardVideoConfig == null)
			{
				return;
			}

			AppsFlyerMeasureAdRevenue.LogAdRevenueEvent(impressionData);
		}

		protected override void LoadSucceededHandler()
		{
			base.LoadSucceededHandler();

			if (_rewardVideoConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventLoadSucceeded, null);
			}
		}

		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);

			if (_rewardVideoConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventLoadFailed,
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
				AppsFlyerMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventRewarded,
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
				AppsFlyerMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventClosed, null);
			}
		}

		protected override void VideoOpenedHandler()
		{
			base.VideoOpenedHandler();

			if (_rewardVideoConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventVideoOpened, null);
			}
		}

		protected override void ShowFailedHandler(AdError error)
		{
			base.ShowFailedHandler(error);

			if (_rewardVideoConfig != null)
			{
				AppsFlyerMeasureAdRevenue.SendAdEvent(_rewardVideoConfig?.EventShowFailed,
					new Dictionary<string, string>
					{
						{ "errormsg", error.Message }
					});
			}
		}
	}
}
