using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	internal class PlacementContentProxy : AndroidJavaProxy
	{
		private readonly Action<bool> _onContentLoaded;
		private readonly Action _onContentClosed;
		private readonly Action<ProductViewResult, Plan> _onResult;

		internal PlacementContentProxy(Action<bool> onContentLoaded, Action onContentClosed,
			Action<ProductViewResult, Plan> onResult) : base(
			"com.purchasely.unity.proxy.PlacementContentProxy")
		{
			_onContentLoaded = onContentLoaded;
			_onContentClosed = onContentClosed;
			_onResult = onResult;
		}

		public void onContentLoaded(bool loaded)
		{
			if (_onContentLoaded == null)
				return;

			AsyncCallbackHelper.Instance.Queue(() => _onContentLoaded(loaded));
		}

		public void onContentClosed()
		{
			if (_onContentClosed == null)
				return;

			AsyncCallbackHelper.Instance.Queue(() => _onContentClosed());
		}

		public void onPresentationResult(int result, string planJson)
		{
			if (_onResult == null)
				return;

			if (Debug.isDebugBuild)
				Debug.Log(planJson);

			AsyncCallbackHelper.Instance.Queue(() => _onResult((ProductViewResult) result, SerializationUtils.Deserialize<Plan>(planJson)));
		}
	}
}