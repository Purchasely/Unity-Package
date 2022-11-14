using System;
using UnityEngine;

namespace Purchasely
{
	public class Purchasely
	{
		public const string SettingsPath = "Purchasely/Settings";
		public const string SettingsFullPath = "Assets/Resources/" + SettingsPath + ".asset";

		private IPurchasely _implementation;

		public Purchasely(string userId, bool readyToPurchase, Store stores, LogLevel logLevel, RunningMode runningMode)
		{
#if UNITY_ANDROID
			_implementation = new PurchaselyAndroid();
#endif
			var settings = Resources.Load<PurchaselySettings>(SettingsFullPath);
			if (settings == null)
				throw new ArgumentException(
					$"Purchasely settings were not found. Please, make sure that asset \'{SettingsFullPath}\' exists. In the Unity Editor go to Window->Purchasely, then provide your API key and other required data.");

			_implementation?.Init(userId, readyToPurchase, (int) stores, (int) logLevel, (int) runningMode);
		}
	}
}