using UnityEngine;

namespace Purchasely
{
	public class PurchaselyAndroid : IPurchasely
	{
		private AndroidJavaObject _javaBridge;
		public void Init(string userId, bool readyToPurchase, int storeFlags, int logLevel, int runningMode)
		{
			_javaBridge = new AndroidJavaObject("com.purchasely.unity.PurchaselyBridge"); //TODO: implement
		}
	}
}
