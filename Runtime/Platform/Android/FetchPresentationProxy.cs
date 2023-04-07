using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	public class FetchPresentationProxy : AndroidJavaProxy
	{
		private readonly Action<Presentation> _onSuccess;
		private readonly Action<string> _onError;

		internal FetchPresentationProxy(Action<Presentation> onSuccess, Action<string> onError) : base(
			"com.purchasely.unity.proxy.FetchPresentationProxy")
		{
			_onSuccess = onSuccess;
			_onError = onError;
		}

		public void onPresentationFetched(string json, AndroidJavaObject presentationAjo)
		{
			AsyncCallbackHelper.Instance.Queue(() =>
			{
				if (Debug.isDebugBuild)
					Debug.Log(json);

				var presentation = SerializationUtils.Deserialize<Presentation>(json);
				if (presentation == null)
				{
					Debug.LogError("Presentation object could not be deserialized.");
					return;
				}
#if UNITY_ANDROID
				presentation.presentationAjo = presentationAjo;
#endif
				_onSuccess(presentation);
			});
		}

		public void onError(string error)
		{
			AsyncCallbackHelper.Instance.Queue(() => _onError(error));
		}
	}
}