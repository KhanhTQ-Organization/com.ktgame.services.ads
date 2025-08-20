using com.ktgame.ads.core;

namespace com.ktgame.ads.max_applovin
{
    public static class MaxApplovinExtensions
    {
#if MAX_APPLOVIN
        public static AdError ToErrorCode(this MaxSdkBase.ErrorCode errorCode, AdPlacement placement)
        {
            return errorCode switch
            {
                MaxSdkBase.ErrorCode.NetworkError => new AdError(placement, AdErrorCode.Unexpected, "The ad request failed due to a generic network error."),
                MaxSdkBase.ErrorCode.NoFill => new AdError(placement, AdErrorCode.NoAdToShow, "No ads are currently eligible for your device."),
                MaxSdkBase.ErrorCode.NetworkTimeout => new AdError(placement, AdErrorCode.ServerResponseFailed, "The ad request timed out (usually due to poor connectivity)."),
                MaxSdkBase.ErrorCode.NoNetwork => new AdError(placement, AdErrorCode.NoInternet, "Indicates that the device is not connected to the internet (e.g. airplane mode)."),
                MaxSdkBase.ErrorCode.AdLoadFailed => new AdError(placement, AdErrorCode.Unexpected, "An internal state error with the AppLovin MAX SDK."),
                _ => new AdError(placement, AdErrorCode.Unexpected, "Unknown reason.")
            };
        }

        public static MaxSdkBase.BannerPosition ToMaxApplovinBannerPosition(this BannerPosition position)
        {
            return position switch
            {
                BannerPosition.Top => MaxSdkBase.BannerPosition.TopCenter,
                BannerPosition.Bottom => MaxSdkBase.BannerPosition.BottomCenter,
                _ => MaxSdkBase.BannerPosition.BottomCenter
            };
        }

        public static ImpressionData ToImpressionData(this MaxSdkBase.AdInfo adInfo, AdFormat adFormat)
        {
            double revenue = adInfo.Revenue;
            string networkName = adInfo.NetworkName;
            string adUnitIdentifier = adInfo.AdUnitIdentifier;
            string adPlacement = adInfo.Placement;
            return new ImpressionData(AdPlatform.Max, networkName, adUnitIdentifier, adFormat, adPlacement,"USD", revenue);
        }
#endif
    }
}