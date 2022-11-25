using System;
using UnityEngine;

namespace Purchasely
{
	internal class StartProxy : AndroidJavaProxy
	{
		private readonly Action<bool, string> _onStartCompleted;

		internal StartProxy(Action<bool, string> onStartCompleted) : base("com.purchasely.unity.proxy.StartProxy")
		{
			_onStartCompleted = onStartCompleted;
		}

		public void onStartCompleted(bool success, string error)
		{
			AsyncCallbackHelper.Instance.Queue(() => _onStartCompleted(success, error));
		}
	}
}