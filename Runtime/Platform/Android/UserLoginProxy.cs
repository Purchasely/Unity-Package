using System;
using UnityEngine;

namespace Purchasely
{
	internal class UserLoginProxy : AndroidJavaProxy
	{
		private readonly Action<bool> _onUserLoginCompleted;

		internal UserLoginProxy(Action<bool> onUserLoginCompleted) : base("com.purchasely.unity.proxy.UserLoginProxy")
		{
			_onUserLoginCompleted = onUserLoginCompleted;
		}

		public void onUserLogin(bool refreshRequired)
		{
			AsyncCallbackHelper.Instance.Queue(() => _onUserLoginCompleted(refreshRequired));
		}
	}
}