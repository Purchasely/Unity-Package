using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	public class FetchPresentationProxy : AndroidJavaProxy
	{
		private readonly Action<Presentation> _onSuccess;
		private readonly Action<string> _onError;
		private readonly Action<ProductViewResult, Plan> _onResult;
		private readonly Action _onCloseButtonClicked;

		internal FetchPresentationProxy(Action<Presentation> onSuccess, Action<string> onError,
			Action<ProductViewResult, Plan> onResult, Action onCloseButtonClicked) : base(
			"com.purchasely.unity.proxy.FetchPresentationProxy")
		{
			_onSuccess = onSuccess;
			_onError = onError;
			_onResult = onResult;
			_onCloseButtonClicked = onCloseButtonClicked;
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

				presentation.presentationAjo = presentationAjo;

				_onSuccess(presentation);
			});
		}

		public void onError(string error)
		{
			AsyncCallbackHelper.Instance.Queue(() => _onError(error));
		}

		public void onPresentationResult(int result, string planJson)
		{
			if (_onResult == null)
				return;

			AsyncCallbackHelper.Instance.Queue(() =>
			{
				if (Debug.isDebugBuild)
					Debug.Log(planJson);

				_onResult((ProductViewResult) result, SerializationUtils.Deserialize<Plan>(planJson));
			});
		}

		void onContentLoaded(bool loaded)
		{
			Debug.Log("Stub.");
		}

		void onContentClosed()
		{
			AsyncCallbackHelper.Instance.Queue(() =>
			{
				_onCloseButtonClicked();
			});
		}
	}
}