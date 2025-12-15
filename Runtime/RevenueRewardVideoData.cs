using System;
using UnityEngine;

namespace com.ktgame.services.ads
{
	[Serializable]
	public struct RevenueRewardVideoData
	{
		[SerializeField] private string _eventLoadFailed;
		[SerializeField] private string _eventLoadSucceeded;
		[SerializeField] private string _eventShowFailed;
		[SerializeField] private string _eventVideoOpened;
		[SerializeField] private string _eventVideoClicked;
		[SerializeField] private string _eventVideoClosed;
		[SerializeField] private string _eventRewarded;
		
		public string EventLoadFailed => _eventLoadFailed;
		public string EventLoadSucceeded => _eventLoadSucceeded;
		public string EventShowFailed => _eventShowFailed;
		public string EventVideoOpened => _eventVideoOpened;
		public string EventClicked => _eventVideoClicked;
		public string EventClosed => _eventVideoClosed;
		public string EventRewarded => _eventRewarded;
	}
}
