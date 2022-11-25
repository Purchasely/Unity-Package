using System;
using UnityEngine;

namespace PurchaselyRuntime
{
	internal class PlacementContentProxy : AndroidJavaProxy
	{
		private readonly Action<bool> _onContentLoaded;
		private readonly Action _onContentClosed;
		private readonly Action<ProductViewResult, PurchaselyPlan> _onResult;

		internal PlacementContentProxy(Action<bool> onContentLoaded, Action onContentClosed,
			Action<ProductViewResult, PurchaselyPlan> onResult) : base(
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

		public void onPresentationResult(int result, AndroidJavaObject plan)
		{
			if (_onResult == null)
				return;

			AsyncCallbackHelper.Instance.Queue(() => _onResult((ProductViewResult) result, new PurchaselyPlan(plan)));
		}
	}
}