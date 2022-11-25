using System;

namespace Purchasely
{
	public class PurchaselyIos : IPurchasely
	{
		public void Init(string apiKey, string userId, bool readyToPurchase, int storeFlags, int logLevel,
			int runningMode, Action<bool, string> onStartCompleted, Action<PurchaselyEvent> onEventReceived)
		{
			throw new NotImplementedException();
		}

		public void UserLogin(string userId, Action<bool> onCompleted)
		{
			throw new NotImplementedException();
		}

		public void SetIsReadyToPurchase(bool ready)
		{
			throw new NotImplementedException();
		}

		public void PresentContentForPlacement(string placementId, bool displayCloseButton,
			Action<ProductViewResult, PurchaselyPlan> onResult, Action<bool> onContentLoaded,
			Action onCloseButtonClicked, string contentId)
		{
			throw new NotImplementedException();
		}
	}
}