using System;
using System.Collections.Generic;
using System.Text;
using PurchaselyRuntime;
using UnityEngine;
using UnityEngine.UI;
using Event = PurchaselyRuntime.Event;

public class PurchaselyDemoController : MonoBehaviour
{
	[SerializeField] private Button readyToPurchaseButton;
	[SerializeField] private Button userLoginButton;

	[SerializeField] private Button setPaywallInterceptorButton;
	[SerializeField] private Button processActionButton;

	[SerializeField] private Button presentContentButton;
	[SerializeField] private InputField placementIdInput;
	[SerializeField] private InputField contentIdInput;
	[SerializeField] private Text logText;

	private PurchaselyRuntime.Purchasely _purchasely;

	private const int LAST_LOGS_COUNT = 10;
	private readonly Queue<string> _logs = new Queue<string>(LAST_LOGS_COUNT);

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

		setPaywallInterceptorButton.onClick.AddListener(OnSetPaywallInterceptorClicked);
		processActionButton.onClick.AddListener(OnProcessActionButtonClicked);

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

	private void OnSetPaywallInterceptorClicked()
	{
		_purchasely.SetPaywallActionInterceptor(OnPaywallActionIntercepted);
	}

	private void OnProcessActionButtonClicked()
	{
		_purchasely.ProcessPaywallAction(true);
	}

	private void OnPresentContentClicked()
	{
		_purchasely.PresentContentForPlacement(placementIdInput.text,
			OnPresentationResult,
			OnPresentationContentLoaded,
			OnPresentationContentClosed,
			contentIdInput.text);
	}

	private void OnPurchaselyStart(bool success, string error)
	{
		Log($"Purchasely Start Result. Success: {success}. Error: {error}.");
	}

	private void OnPurchaselyEvent(Event @event)
	{
		Log($"Purchasely Event Received. Name: {@event.name}. Type: {@event.type}.");
	}

	private void OnUserLoginCompleted(bool needRefresh)
	{
		Log($"Purchasely User Login Completed. Need refresh: {needRefresh}.");
	}

	private void OnPaywallActionIntercepted(PaywallAction action)
	{
		Log($"Purchasely Paywall Action Intercepted. Action: {action.action}.");
	}

	private void OnPresentationResult(ProductViewResult result, Plan plan)
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

		if (_logs.Count >= LAST_LOGS_COUNT)
			_logs.Dequeue();

		_logs.Enqueue($"{DateTime.Now:T:}: {log}");

		var stringBuilder = new StringBuilder();
		foreach (var logString in _logs)
		{
			stringBuilder.AppendLine(logString);
		}

		logText.text = stringBuilder.ToString();
	}
}