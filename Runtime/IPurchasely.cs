using System;

namespace Purchasely
{
	public interface IPurchasely
	{
		void Init(string apiKey, string userId, bool readyToPurchase, int storeFlags, int logLevel, int runningMode,
			Action<bool, string> onStartCompleted, Action<PurchaselyEvent> onEventReceived);
	}
}