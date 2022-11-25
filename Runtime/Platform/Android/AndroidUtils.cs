using UnityEngine;

namespace Purchasely
{
	internal static class AndroidUtils
	{
		internal static AndroidJavaObject Activity
		{
			get
			{
				const string UnityPlayerClassName = "com.unity3d.player.UnityPlayer";
				var unityPlayer = new AndroidJavaClass(UnityPlayerClassName);

				const string CurrentActivityFieldName = "currentActivity";
				return unityPlayer.GetStatic<AndroidJavaObject>(CurrentActivityFieldName);
			}
		}
	}
}