using Cysharp.Threading.Tasks;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
using AOT;
#endif

namespace com.ktgame.services.ads
{
	public enum AttStatus
	{
		NotSupported = -1,
		NotDetermined = 0,
		Restricted = 1,
		Denied = 2,
		Authorized = 3
	}

	public static class AttBridge
	{
#if UNITY_IOS && !UNITY_EDITOR
		private delegate void AttCallback(int status);

		[DllImport("__Internal")]
		private static extern int _KtAttGetStatus();

		[DllImport("__Internal")]
		private static extern void _KtAttRequest(AttCallback callback);

		private static UniTaskCompletionSource<AttStatus> _pending;

		[MonoPInvokeCallback(typeof(AttCallback))]
		private static void OnRequestCompleted(int status)
		{
			var pending = _pending;
			_pending = null;
			pending?.TrySetResult((AttStatus)status);
		}
#endif

		public static AttStatus Status
		{
			get
			{
#if UNITY_IOS && !UNITY_EDITOR
				return (AttStatus)_KtAttGetStatus();
#else
				return AttStatus.NotSupported;
#endif
			}
		}

		public static UniTask<AttStatus> RequestAsync()
		{
#if UNITY_IOS && !UNITY_EDITOR
			var status = Status;
			if (status != AttStatus.NotDetermined)
			{
				return UniTask.FromResult(status);
			}

			if (_pending != null)
			{
				return _pending.Task;
			}

			_pending = new UniTaskCompletionSource<AttStatus>();
			var task = _pending.Task;
			_KtAttRequest(OnRequestCompleted);
			return task;
#else
			return UniTask.FromResult(AttStatus.NotSupported);
#endif
		}
	}
}
