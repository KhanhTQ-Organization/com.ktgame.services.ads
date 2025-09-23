using System;
using UnityEngine;

namespace com.ktgame.services.ads
{
	[Serializable]
	public struct RevenueInterstitialData
	{
		[SerializeField] private string _eventLoadFailed;
		[SerializeField] private string _eventLoadSucceeded;
		[SerializeField] private string _eventShowFailed;
		[SerializeField] private string _eventShowSucceeded;
		[SerializeField] private string _eventClicked;
		[SerializeField] private string _eventClosed;

		public string EventLoadFailed => _eventLoadFailed;
		public string EventLoadSucceeded => _eventLoadSucceeded;
		public string EventShowFailed => _eventShowFailed;
		public string EventShowSucceeded => _eventShowSucceeded;
		public string EventClicked => _eventClicked;
		public string EventClosed => _eventClosed;
	}
}