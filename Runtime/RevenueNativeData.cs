using System;
using UnityEngine;

namespace com.ktgame.services.ads
{
	[Serializable]
	public class RevenueNativeData
	{
		[SerializeField] private string _eventLoadSucceeded;
        
		public string EventLoadSucceeded => _eventLoadSucceeded;
	}
}