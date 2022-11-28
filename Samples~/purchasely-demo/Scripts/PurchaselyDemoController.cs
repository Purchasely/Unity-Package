using System;
using PurchaselyRuntime;
using UnityEngine;
using UnityEngine.UI;

public class PurchaselyDemoController : MonoBehaviour
{
	[SerializeField] private Button readyToPurchaseButton;
	[SerializeField] private Button userLoginButton;
	[SerializeField] private Button presentContentButton;
	[SerializeField] private InputField placementIdInput;
	[SerializeField] private InputField contentIdInput;
	[SerializeField] private Toggle showCloseButtonToggle;
	[SerializeField] private Text logText;

	private PurchaselyRuntime.Purchasely _purchasely;

	private void Start()
	{
		var userId = Guid.NewGuid().ToString();
		_purchasely = new PurchaselyRuntime.Purchasely(userId,
			false,
			LogLevel.Debug,
			RunningMode.Full,
			OnPurchaselyStart,
			OnPurchaselyEvent);

		readyToPurchaseButton.onClick.AddListener(OnSetReadyToPurchaseClicked);
		userLoginButton.onClick.AddListener(OnSetUserIdClicked);
		presentContentButton.onClick.AddListener(OnPresentContentClicked);
	}

	private void OnSetReadyToPurchaseClicked()
	{
		_purchasely.SetReadyToPurchase(true);
	}

	private void OnSetUserIdClicked()
	{
		var userId = Guid.NewGuid().ToString();
		_purchasely.UserLogin(userId, OnUserLoginCompleted);
	}

	private void OnPresentContentClicked()
	{
		_purchasely.PresentContentForPlacement(placementIdInput.text,
			showCloseButtonToggle.isOn,
			OnPresentationResult,
			OnPresentationContentLoaded,
			OnPresentationContentClosed,
			contentIdInput.text);
	}

	private void OnPurchaselyStart(bool success, string error)
	{
		Log($"Purchasely Start Result. Success: {success}. Error: {error}.");
	}

	private void OnPurchaselyEvent(PurchaselyEvent purchaselyEvent)
	{
		Log($"Purchasely Event Received. Name: {purchaselyEvent.Name}. Properties: {purchaselyEvent.PropertiesJson}.");
	}

	private void OnUserLoginCompleted(bool needRefresh)
	{
		Log($"Purchasely User Login Completed. Need refresh: {needRefresh}.");
	}

	private void OnPresentationResult(ProductViewResult result, PurchaselyPlan plan)
	{
		Log($"Presentation Result: {result}.");
	}

	private void OnPresentationContentLoaded(bool loaded)
	{
		Log($"Presentation Content Loaded: {loaded}.");
	}

	private void OnPresentationContentClosed()
	{
		Log("Presentation Content Closed.");
	}

	private void Log(string log)
	{
		Debug.Log(log);
		logText.text += $"\n{DateTime.Now:T}{log}";
	}
}