using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	public class JsonErrorProxy : AndroidJavaProxy
	{
		private readonly Action<string> _onSuccess;
		private readonly Action<string> _onError;

		internal JsonErrorProxy(Action<string> onSuccess, Action<string> onError) : base(
			"com.purchasely.unity.proxy.JsonErrorProxy")
		{
			_onSuccess = onSuccess;
			_onError = onError;
		}

		public void onSuccess(string json)
		{
			if (Debug.isDebugBuild)
				Debug.Log(json);

			AsyncCallbackHelper.Instance.Queue(() => _onSuccess(json));
		}

		public void onError(string error)
		{
			AsyncCallbackHelper.Instance.Queue(() => _onError(error));
		}
	}
}