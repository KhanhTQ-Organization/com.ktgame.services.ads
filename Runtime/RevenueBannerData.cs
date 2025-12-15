using System;
using UnityEngine;

namespace com.ktgame.services.ads
{
	[Serializable]
	public struct RevenueBannerData
	{
		[SerializeField] private string _eventLoadFailed;
		[SerializeField] private string _eventLoadSucceeded;
		[SerializeField] private string _eventImpressionSuccess;
		
		public string EventLoadFailed => _eventLoadFailed;
		public string EventLoadSucceeded => _eventLoadSucceeded;
		public string EventImpressionSuccess => _eventImpressionSuccess;
	}
}
