#if UNITY_IOS && !UNITY_EDITOR

using System;
using System.Runtime.InteropServices;

namespace PurchaselyRuntime
{
	public class PurchaselyIos : IPurchasely
	{
		public void Init(string apiKey, string userId, bool readyToPurchase, int logLevel,
			int runningMode, Action<bool, string> onStartCompleted, Action<PurchaselyEvent> onEventReceived)
		{
			var startCallback = new Action<bool, string>((success, error) =>
			{
				AsyncCallbackHelper.Instance.Queue(() => { onStartCompleted(success, error); });
			});

			var eventCallback = new Action<string, string>((name, propertiesJson) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onEventReceived(new PurchaselyEvent(name, propertiesJson));
				});
			});

			_purchaselyStart(apiKey, userId, readyToPurchase, logLevel, runningMode,
				IosUtils.StartCallback, startCallback.GetPointer(),
				IosUtils.EventCallback, eventCallback.GetPointer());
		}

		public void UserLogin(string userId, Action<bool> onCompleted)
		{
			var completeCallback = new Action<bool>(needRefresh =>
			{
				AsyncCallbackHelper.Instance.Queue(() => { onCompleted(needRefresh); });
			});

			_purchaselyUserLogin(userId, IosUtils.BoolCallback, completeCallback.GetPointer());
		}

		public void SetIsReadyToPurchase(bool ready)
		{
			_purchaselySetIsReadyToPurchase(ready);
		}

		public void PresentContentForPlacement(string placementId, bool displayCloseButton,
			Action<ProductViewResult, PurchaselyPlan> onResult, Action<bool> onContentLoaded,
			Action onCloseButtonClicked, string contentId)
		{
			var resultCallback = new Action<int, IntPtr>((resultInt, planPointer) =>
			{
				AsyncCallbackHelper.Instance.Queue(() =>
				{
					onResult((ProductViewResult) resultInt, new PurchaselyPlan(planPointer));
				});
			});

			var contentLoadCallback = new Action<bool>(isLoaded =>
			{
				AsyncCallbackHelper.Instance.Queue(() => { onContentLoaded(isLoaded); });
			});

			var closeButtonCallback = new Action(() => { AsyncCallbackHelper.Instance.Queue(onCloseButtonClicked); });

			if (contentId == null)
				contentId = string.Empty;

			_purchaselyShowContentForPlacement(placementId, contentId, displayCloseButton,
				IosUtils.BoolCallback, contentLoadCallback.GetPointer(),
				IosUtils.VoidCallback, closeButtonCallback.GetPointer(),
				IosUtils.PresentationResultCallback, resultCallback.GetPointer());
		}

		[DllImport("__Internal")]
		static extern void _purchaselyStart(string apiKey, string userId, bool readyToPurchase, int logLevel,
			int runningMode, IosUtils.StartCallbackDelegate startCallback, IntPtr startCallbackPtr,
			IosUtils.EventCallbackDelegate eventCallback, IntPtr eventCallbackPtr);

		[DllImport("__Internal")]
		static extern void _purchaselyUserLogin(string userId, IosUtils.BoolCallbackDelegate onUserLogin,
			IntPtr onUserLoginPtr);

		[DllImport("__Internal")]
		static extern void _purchaselySetIsReadyToPurchase(bool ready);

		[DllImport("__Internal")]
		static extern void _purchaselyShowContentForPlacement(string placementId, string contentId,
			bool displayCloseButton,
			IosUtils.BoolCallbackDelegate loadCallback, IntPtr loadCallbackPtr,
			IosUtils.VoidCallbackDelegate closeCallback, IntPtr closeCallbackPtr,
			IosUtils.PresentationResultCallbackDelegate presentationResultCallback,
			IntPtr presentationResultCallbackPtr);
	}
}

#endif