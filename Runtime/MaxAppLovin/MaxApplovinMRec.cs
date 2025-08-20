using System;
using com.ktgame.ads.core;
using UnityEngine;

namespace com.ktgame.ads.max_applovin
{
    public class MaxApplovinMRec : IMRecAdapter
    {
        protected string UnitId { private set; get; }
        
        public event Action<AdPlacement> OnLoadSucceeded;
        public event Action<AdError> OnLoadFailed;
        public event Action<AdPlacement> OnClicked;
        public event Action<ImpressionData> OnImpressionSuccess;
        
        protected AdPlacement AdPlacement { private set; get; }
        protected Vector2 AdSize { private set; get; }
        protected MRecPosition AdPosition { private set; get; }
        
        public MaxApplovinMRec(string unitId, Vector2 mRectSize, MRecPosition mRecPosition)
        {
            UnitId = unitId;
            AdSize = mRectSize;
            AdPosition = mRecPosition;
            AdPlacement = new AdPlacement("MRec");
#if MAX_APPLOVIN
		MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
#endif
        }
        
        public void Load()
        {
#if MAX_APPLOVIN     
            MaxSdk.CreateMRec(UnitId, MaxSdkBase.AdViewPosition.Centered);
            MaxSdk.SetMRecPlacement(UnitId, String.Empty);
#endif
        }
		
#if MAX_APPLOVIN    
		public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			OnLoadSucceeded?.Invoke(AdPlacement);
		}
		
		public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
		{
			var adError = errorInfo.Code.ToErrorCode(AdPlacement);
			OnLoadFailed?.Invoke(adError);
		}

		public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			OnClicked?.Invoke(AdPlacement);
		}
		
		private void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			var impressionData = adInfo.ToImpressionData(AdFormat.MRec);
			OnImpressionSuccess?.Invoke(impressionData);
		}
#endif
		
        public void Show()
        {
#if MAX_APPLOVIN
			MaxSdk.ShowMRec(UnitId);
#endif
        }

		public void Hide()
		{
#if MAX_APPLOVIN
			 MaxSdk.HideMRec(UnitId);
#endif
		}

		public void UpdateMRecPosition(MRecPosition position)
		{
#if MAX_APPLOVIN
			var mRecPosition = (MaxSdkBase.AdViewPosition)Enum.Parse(typeof(MaxSdkBase.AdViewPosition), position.ToString());
			MaxSdk.UpdateMRecPosition(UnitId, mRecPosition);
#endif
		}

		public void UpdateMRecPosition(Vector2 position)
		{
#if MAX_APPLOVIN
			MaxSdk.UpdateMRecPosition(UnitId, position.x, position.y);
#endif
		}

		public void Destroy()
        {
#if MAX_APPLOVIN
			MaxSdk.DestroyMRec(UnitId);
#endif
        }
    }
}