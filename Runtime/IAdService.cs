using System;
using com.ktgame.core;
using com.ktgame.ads.core;

namespace com.ktgame.services.ads
{
	public interface IAdService : IService, IInitializable
	{
		IAdAdapter Ad { get; }
		IAdAdapter AdBackFill { get; }
		bool CanRequestAds { get; }
		bool AdsStarted { get; }
		bool IsPrivacyOptionsRequired { get; }
		void ShowPrivacyOptionsForm(Action<string> onComplete = null);
		void ResetConsentForDebug();
		void SetPause(bool pause);
	}
}
