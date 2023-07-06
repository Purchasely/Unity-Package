using System;
using System.Collections.Generic;
using System.Text;
using PurchaselyRuntime;
using UnityEngine;
using UnityEngine.UI;
using Event = PurchaselyRuntime.Event;

public class PurchaselyDemoController : MonoBehaviour
{
	[SerializeField] private Button userActionsButton;
	[SerializeField] private Button userAttributesButton;

	[SerializeField] private Button setPaywallInterceptorButton;
	[SerializeField] private Button processActionButton;

	[SerializeField] private Button presentPlacementButton;
	[SerializeField] private InputField placementIdInput;
	[SerializeField] private InputField contentIdInputPlacement;

	[SerializeField] private Button presentPresentationButton;
	[SerializeField] private InputField presentationIdInputPresentation;
	[SerializeField] private InputField contentIdInputPresentation;

	[SerializeField] private Button presentProductButton;
	[SerializeField] private InputField productIdInput;
	[SerializeField] private InputField presentationIdInputProduct;
	[SerializeField] private InputField contentIdInputProduct;

	[SerializeField] private Button presentPlanButton;
	[SerializeField] private InputField planIdInput;
	[SerializeField] private InputField presentationIdInputPlan;
	[SerializeField] private InputField contentIdInputPlan;

	[SerializeField] private Button listSubscriptionsButton;
	[SerializeField] private Button showSubscriptionsButton;

	[SerializeField] private Button restorePurchasesButton;
	[SerializeField] private Button getAllProductsButton;

	[SerializeField] private Button getProductButton;
	[SerializeField] private InputField productIdInputGet;

	[SerializeField] private Button getPlanButton;
	[SerializeField] private InputField planIdInputGet;

	[SerializeField] private Button purchaseButton;
	[SerializeField] private InputField planIdInputPurchase;
	[SerializeField] private InputField contentIdInputPurchase;

	[SerializeField] private Button fetchPresentationButton;
	[SerializeField] private InputField presentationIdInputFetch;
	[SerializeField] private InputField contentIdInputFetchPresentation;

	[SerializeField] private Button fetchPresentationForPlacementButton;
	[SerializeField] private InputField placementIdInputFetch;
	[SerializeField] private InputField contentIdInputFetchPlacement;

	[SerializeField] private Text logText;
	[SerializeField] private PurchaselyDemoPaywall paywall;

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

		paywall.Init(_purchasely);

		userActionsButton.onClick.AddListener(OnUserActionsClicked);
		userAttributesButton.onClick.AddListener(OnUserAttributesClicked);

		setPaywallInterceptorButton.onClick.AddListener(OnSetPaywallInterceptorClicked);
		processActionButton.onClick.AddListener(OnProcessActionButtonClicked);

		presentPlacementButton.onClick.AddListener(OnPresentPlacementClicked);
		presentPresentationButton.onClick.AddListener(OnPresentPresentationClicked);
		presentProductButton.onClick.AddListener(OnPresentProductClicked);
		presentPlanButton.onClick.AddListener(OnPresentPlanClicked);

		listSubscriptionsButton.onClick.AddListener(OnListSubscriptionsClicked);
		showSubscriptionsButton.onClick.AddListener(OnShowSubscriptionsClicked);

		restorePurchasesButton.onClick.AddListener(OnRestorePurchasesClicked);
		getAllProductsButton.onClick.AddListener(OnGetAllProductsClicked);

		getProductButton.onClick.AddListener(OnGetProductClicked);
		getPlanButton.onClick.AddListener(OnGetPlanClicked);

		purchaseButton.onClick.AddListener(OnPurchaseClicked);

		fetchPresentationButton.onClick.AddListener(OnFetchPresentationClicked);
		fetchPresentationForPlacementButton.onClick.AddListener(OnFetchPresentationForPlacementClicked);
	}

	private void OnUserActionsClicked()
	{
		_purchasely.UserDidConsumeSubscriptionContent();
		_purchasely.UserLogout();

		var userId = _purchasely.GetAnonymousUserId();
		_purchasely.UserLogin(userId, OnUserLoginCompleted);
	}


	private void OnUserAttributesClicked()
	{
		_purchasely.SetUserAttribute("StringAttribute", "String message");
		_purchasely.SetUserAttribute("IntAttribute", -100);
		_purchasely.SetUserAttribute("FloatAttribute", 147.5f);
		_purchasely.SetUserAttribute("BoolAttribute", true);
		_purchasely.SetUserAttribute("DateAttribute", DateTime.Now);

		Log($"String Attribute: {_purchasely.GetUserAttribute("StringAttribute")}");
		Log($"Int Attribute: {_purchasely.GetUserAttribute("IntAttribute")}");
		Log($"Float Attribute: {_purchasely.GetUserAttribute("FloatAttribute")}");
		Log($"Bool Attribute: {_purchasely.GetUserAttribute("BoolAttribute")}");
		Log($"Date Attribute: {_purchasely.GetUserAttribute("DateAttribute")}");

		_purchasely.ClearUserAttribute("StringAttribute");
		Log($"String Attribute after clear: {_purchasely.GetUserAttribute("StringAttribute")}");

		_purchasely.ClearUserAttributes();
		Log($"Int Attribute after clear: {_purchasely.GetUserAttribute("IntAttribute")}");
	}

	private void OnSetPaywallInterceptorClicked()
	{
		_purchasely.SetPaywallActionInterceptor(OnPaywallActionIntercepted);
	}

	private void OnProcessActionButtonClicked()
	{
		_purchasely.ProcessPaywallAction(true);
	}

	private void OnPresentPlacementClicked()
	{
		_purchasely.PresentPresentationForPlacement(placementIdInput.text,
			OnPresentationResult,
			OnPresentationContentLoaded,
			OnPresentationContentClosed,
			contentIdInputPlacement.text);
	}

	private void OnPresentPresentationClicked()
	{
		_purchasely.PresentPresentationWithId(presentationIdInputPresentation.text,
			OnPresentationResult,
			OnPresentationContentLoaded,
			OnPresentationContentClosed,
			contentIdInputPresentation.text);
	}

	private void OnPresentProductClicked()
	{
		_purchasely.PresentPresentationForProduct(productIdInput.text,
			OnPresentationResult,
			OnPresentationContentLoaded,
			OnPresentationContentClosed,
			contentIdInputProduct.text,
			presentationIdInputProduct.text);
	}

	private void OnPresentPlanClicked()
	{
		_purchasely.PresentPresentationForPlan(planIdInput.text,
			OnPresentationResult,
			OnPresentationContentLoaded,
			OnPresentationContentClosed,
			contentIdInputPlan.text,
			presentationIdInputPlan.text);
	}

	private void OnListSubscriptionsClicked()
	{
		_purchasely.GetUserSubscriptions(OnGetSubscriptionsSuccess, Log);
	}

	private void OnShowSubscriptionsClicked()
	{
		_purchasely.PresentSubscriptions();
	}

	private void OnRestorePurchasesClicked()
	{
		_purchasely.RestoreAllProducts(false, LogPlan, Log);
	}

	private void OnGetAllProductsClicked()
	{
		_purchasely.GetAllProducts(OnGetAllProductsSuccess, Log);
	}

	private void OnGetProductClicked()
	{
		_purchasely.GetProduct(productIdInputGet.text, LogProduct, Log);
	}

	private void OnGetPlanClicked()
	{
		_purchasely.GetPlan(planIdInputGet.text, LogPlan, Log);
	}

	private void OnPurchaseClicked()
	{
		_purchasely.Purchase(planIdInputPurchase.text, LogPlan, Log, contentIdInputPurchase.text);
	}

	private void OnFetchPresentationClicked()
	{
		_purchasely.FetchPresentation(presentationIdInputFetch.text, OnFetchPresentationSuccess, Log,
			contentIdInputFetchPresentation.text);
	}

	private void OnFetchPresentationForPlacementClicked()
	{
		_purchasely.FetchPresentationForPlacement(placementIdInputFetch.text, OnFetchPresentationSuccess, Log,
			contentIdInputFetchPlacement.text);
	}

	private void OnPurchaselyStart(bool success, string error)
	{
		Log($"Purchasely Start Result. Success: {success}. Error: {error}.");

		_purchasely.SetReadyToPurchase(true);
		_purchasely.SetLanguage("en");
		_purchasely.SetDefaultPresentationResultHandler(OnDefaultPresentationResult);
		_purchasely.HandleDeepLinkUrl("https://purchasely.com");
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

	private void OnDefaultPresentationResult(ProductViewResult result, Plan plan)
	{
		Log($"Default Presentation Result: {result}.");
	}

	private void OnGetSubscriptionsSuccess(List<SubscriptionData> subscriptionData)
	{
		Log("Get Subscription Data Success.");

		foreach (var subscription in subscriptionData)
		{
			Log($"Subscription ID: {subscription.id}");

			var plan = subscription.plan;
			if (plan != null)
				LogPlan(plan);

			var product = subscription.product;
			if (product != null)
				LogProduct(product);
		}
	}

	private void LogPlan(Plan plan)
	{
		Log($"Plan name: {plan.name}. ID: {plan.vendorId}");
	}

	private void LogProduct(Product product)
	{
		Log($"Product name: {product.name}. ID: {product.vendorId}");
	}

	private void OnGetAllProductsSuccess(List<Product> products)
	{
		Log($"Get All Products Success. Products fetched: {products.Count}.");
		foreach (var product in products)
		{
			LogProduct(product);
		}
	}

	private void OnFetchPresentationSuccess(Presentation presentation)
	{
		Log("Fetch Presentation Success.");
		LogPresentation(presentation);

		switch (presentation.presentationType)
		{
			case PresentationType.Normal:
			case PresentationType.Fallback:
				_purchasely.PresentPresentationWithId(presentation.id,
					OnPresentationResult,
					OnPresentationContentLoaded,
					OnPresentationContentClosed);
				break;
			case PresentationType.Unknown:
			case PresentationType.Deactivated:
				Log($"Fetched presentation with type: {presentation.presentationType}. Will not show content.");
				break;
			case PresentationType.Client:
				paywall.Show(presentation);
				break;
		}
	}

	private void LogPresentation(Presentation presentation)
	{
		Log($"Presentation id: {presentation.id}. Type: {presentation.presentationType}. ");
	}

	private void Log(string log)
	{
		Debug.Log(log);

		if (_logs.Count >= LAST_LOGS_COUNT)
			_logs.Dequeue();

		_logs.Enqueue($"{DateTime.Now:T}: {log}");

		var stringBuilder = new StringBuilder();
		foreach (var logString in _logs)
		{
			stringBuilder.AppendLine(logString);
		}

		logText.text = stringBuilder.ToString();
	}
}