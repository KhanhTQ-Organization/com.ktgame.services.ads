using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if ADMOB
using System.Collections.Generic;
using GoogleMobileAds.Ump.Api;
#endif

namespace com.ktgame.services.ads
{
	public sealed class AdConsentManager
	{
		private const string LogTag = "[AdConsent]";

		private readonly AdServiceSettings _settings;
		
		public event Action OnConsentChanged;

		public bool InProgress { get; private set; }

		public AdConsentManager(AdServiceSettings settings)
		{
			_settings = settings;
		}

		public bool CanRequestAds
		{
			get
			{
#if ADMOB
				return ConsentInformation.CanRequestAds();
#else
				return true;
#endif
			}
		}

		public bool IsPrivacyOptionsRequired
		{
			get
			{
#if ADMOB
				return ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required;
#else
				return false;
#endif
			}
		}

		public AttStatus TrackingStatus => AttBridge.Status;

		public async UniTask GatherAsync()
		{
			if (InProgress)
			{
				return;
			}

			InProgress = true;
			try
			{
#if ADMOB
				await GatherUmpAsync();
#endif
				await RequestTrackingAsync();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			finally
			{
				InProgress = false;
			}
		}

#if ADMOB
		private async UniTask GatherUmpAsync()
		{
			var updateTcs = new UniTaskCompletionSource<bool>();
			ConsentInformation.Update(BuildRequest(), updateError =>
			{
				if (updateError != null)
				{
					Debug.LogWarning($"{LogTag} UMP update failed: {updateError.Message}");
					updateTcs.TrySetResult(false);
					return;
				}

				updateTcs.TrySetResult(true);
			});

			var timeout = TimeSpan.FromSeconds(Mathf.Max(1f, _settings.ConsentUpdateTimeoutSeconds));
			var (isTimeout, updated) = await updateTcs.Task.TimeoutWithoutException(timeout, DelayType.Realtime);
			await UniTask.SwitchToMainThread();

			if (isTimeout)
			{
				Debug.LogWarning($"{LogTag} UMP update timed out after {timeout.TotalSeconds}s.");
				return;
			}

			if (!updated)
			{
				return;
			}
			
			var formTcs = new UniTaskCompletionSource();
			ConsentForm.LoadAndShowConsentFormIfRequired(formError =>
			{
				if (formError != null)
				{
					Debug.LogWarning($"{LogTag} UMP form error: {formError.Message}");
				}

				formTcs.TrySetResult();
			});

			await formTcs.Task;
			await UniTask.SwitchToMainThread();

			Debug.Log($"{LogTag} UMP done. status={ConsentInformation.ConsentStatus}, canRequestAds={ConsentInformation.CanRequestAds()}, privacyOptions={ConsentInformation.PrivacyOptionsRequirementStatus}");
		}
#endif

		private async UniTask RequestTrackingAsync()
		{
			if (AttBridge.Status != AttStatus.NotDetermined)
			{
				return;
			}

			if (!CanRequestAds)
			{
				Debug.Log($"{LogTag} ATT skipped: consent does not allow ad requests.");
				return;
			}

			await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.Realtime);
			var status = await AttBridge.RequestAsync();
			await UniTask.SwitchToMainThread();
			Debug.Log($"{LogTag} ATT done. status={status}");
		}

		public void ShowPrivacyOptionsForm(Action<string> onComplete = null)
		{
#if ADMOB
			ConsentForm.ShowPrivacyOptionsForm(formError =>
			{
				var message = formError?.Message;
				if (message != null)
				{
					Debug.LogWarning($"{LogTag} Privacy options form error: {message}");
				}

				OnConsentChanged?.Invoke();
				onComplete?.Invoke(message);
			});
#else
			onComplete?.Invoke(null);
#endif
		}
		
		public void ResetForDebug()
		{
#if ADMOB
			if (!Debug.isDebugBuild)
			{
				Debug.LogWarning($"{LogTag} ResetForDebug ignored in release build.");
				return;
			}

			ConsentInformation.Reset();
			Debug.Log($"{LogTag} UMP consent reset. Restart the app to see the form again.");
#endif
		}

#if ADMOB
		private ConsentRequestParameters BuildRequest()
		{
			var request = new ConsentRequestParameters
			{
				TagForUnderAgeOfConsent = _settings.TagForUnderAgeOfConsent,
			};
			
			if (Debug.isDebugBuild && _settings.UmpDebugGeography != UmpDebugGeography.Disabled)
			{
				request.ConsentDebugSettings = new ConsentDebugSettings
				{
					DebugGeography = DebugGeography.EEA,
					TestDeviceHashedIds = new List<string>(_settings.UmpTestDeviceHashedIds)
				};
				Debug.Log($"{LogTag} UMP debug geography = EEA, test devices = {_settings.UmpTestDeviceHashedIds.Count}");
			}

			return request;
		}
#endif
	}
}
