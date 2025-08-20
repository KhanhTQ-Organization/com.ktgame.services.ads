using System;

namespace com.ktgame.ads.core
{
	public class AdMobCollapsibleBanner : IBannerAdapter
	{
		public event Action<AdError> OnLoadFailed;
		public event Action<AdPlacement> OnLoadSucceeded;
		public event Action<ImpressionData> OnImpressionSuccess;

		protected string UnitId { private set; get; }
		protected AdPlacement AdPlacement { private set; get; }
		protected BannerSize AdSize { private set; get; }
		protected BannerPosition AdPosition { private set; get; }
		
		public AdMobCollapsibleBanner(string unitId, BannerSize bannerSize, BannerPosition bannerPosition)
		{
			UnitId = unitId;
			AdSize = bannerSize;
			AdPosition = bannerPosition;
			AdPlacement = new AdPlacement("Banner");
		}

		public void Load()
		{
		}

		public void Show()
		{
		}

		public void Hide()
		{
		}

		public void Destroy()
		{
		}
	}
}