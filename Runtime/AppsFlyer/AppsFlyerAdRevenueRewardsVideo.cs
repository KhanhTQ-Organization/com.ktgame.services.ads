using com.ktgame.ads.core;

namespace com.ktgame.services.ads.appsflyer_ad_revenue
{
	public class AppsFlyerAdRevenueRewardsVideo : RewardVideoDecorator
	{
		private RevenueAdSetting _settings;

		public AppsFlyerAdRevenueRewardsVideo(IRewardVideoAdapter adapter) : base(adapter)
		{
			_settings = RevenueAdSetting.Instance;
		}
		
		protected override void ImpressionSuccessHandler(ImpressionData impressionData)
		{
			base.ImpressionSuccessHandler(impressionData);
			AppsFlyerMeasureAdRevenue.LogAdRevenueEvent(impressionData);
		}

		protected override void LoadSucceededHandler()
		{
			base.LoadSucceededHandler();
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerRewardVideoTracking.EventLoadSucceeded, null);
		}

		protected override void LoadFailedHandler(AdError adError)
		{
			base.LoadFailedHandler(adError);
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerRewardVideoTracking.EventLoadFailed, null);
		}
		
		protected override void RewardHandler(AdPlacement rewardData)
		{
			base.RewardHandler(rewardData);
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerRewardVideoTracking.EventRewarded, null);
		}

		protected override void VideoClosedHandler()
		{
			base.VideoClosedHandler();
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerRewardVideoTracking.EventClosed, null);
		}

		protected override void VideoOpenedHandler()
		{
			base.VideoOpenedHandler();
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerRewardVideoTracking.EventVideoOpened, null);
		}

		protected override void ShowFailedHandler(AdError error)
		{
			base.ShowFailedHandler(error);
			AppsFlyerMeasureAdRevenue.SendAdEvent(_settings.AppsFlyerRewardVideoTracking.EventShowFailed, null);
		}
	}
}