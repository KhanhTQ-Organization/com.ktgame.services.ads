using com.ktgame.ads.core;

namespace com.ktgame.ads.admob
{
	public static class AdMobExtensions
	{
#if ADMOB
		public static AdError ToAdError(GoogleMobileAds.Api.AdError admobError, AdPlacement placement)
		{
			return admobError.GetCode() switch
			{
				0 => new AdError(placement, AdErrorCode.NoFill, "No ads are currently eligible for your device."),
				1 => new AdError(placement, AdErrorCode.InvalidRequest, "The ad request was invalid. Please check your ad unit ID or configuration."),
				2 => new AdError(placement, AdErrorCode.Unexpected, "An unexpected error occurred while loading the ad."),
				3 => new AdError(placement, AdErrorCode.Timeout, "Ad loading timed out. Please try again."),
				8 => new AdError(placement, AdErrorCode.ShowError, "An error occurred while trying to show the ad."),
				_ => new AdError(placement, AdErrorCode.Unexpected, $"Unhandled error code: {admobError.GetCode()}")
			};
		}
		
		public static ImpressionData ToImpressionData(this GoogleMobileAds.Api.AdValue adInfo, string unitId, AdFormat adFormat,
			GoogleMobileAds.Api.ResponseInfo responseInfo = null)
		{
			double revenue = adInfo.Value / 1_000_000.0;
			string currency = string.IsNullOrEmpty(adInfo.CurrencyCode) ? "USD" : adInfo.CurrencyCode;
			return new ImpressionData(AdPlatform.Admob, GetAdSourceName(responseInfo), unitId, adFormat, adFormat.ToString(), currency, revenue);
		}

		private static string GetAdSourceName(GoogleMobileAds.Api.ResponseInfo responseInfo)
		{
			try
			{
				var adapterInfo = responseInfo?.GetLoadedAdapterResponseInfo();
				var name = adapterInfo?.AdSourceName;
				return string.IsNullOrEmpty(name) ? "AdMob" : name;
			}
			catch (System.Exception)
			{
				return "AdMob";
			}
		}
#endif
	}
}