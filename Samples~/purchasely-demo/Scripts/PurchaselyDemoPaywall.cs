using System.Collections.Generic;
using PurchaselyRuntime;
using UnityEngine;
using UnityEngine.UI;

public class PurchaselyDemoPaywall : MonoBehaviour
{
	[SerializeField] private Button closeButton;
	[SerializeField] private Text nameText;
	[SerializeField] private PurchaselyDemoPlanButton buttonPrefab;
	[SerializeField] private Transform contentHolder;

	private PurchaselyRuntime.Purchasely _purchasely;
	private Presentation _presentation;

	private readonly Dictionary<string, PurchaselyDemoPlanButton> _planButtons =
		new Dictionary<string, PurchaselyDemoPlanButton>();

	public void Init(PurchaselyRuntime.Purchasely purchasely)
	{
		_purchasely = purchasely;
	}

	public void Show(Presentation presentation)
	{
		_presentation = presentation;
		gameObject.SetActive(true);

		_purchasely.ClientPresentationOpened(_presentation);

		nameText.text = presentation.id;

		ClearButtons();
		foreach (var plan in presentation.plans)
		{
			_purchasely.GetPlan(plan, OnPlanFetched, Debug.LogError);
		}
	}

	private void Start()
	{
		closeButton.onClick.AddListener(OnCloseButtonClicked);
	}

	private void OnCloseButtonClicked()
	{
		gameObject.SetActive(false);
		if (_presentation == null)
			return;

		_purchasely.ClientPresentationClosed(_presentation);
		_presentation = null;
	}

	private void ClearButtons()
	{
		foreach (var button in _planButtons.Values)
		{
			Destroy(button.gameObject);
		}

		_planButtons.Clear();
	}

	private void OnPlanFetched(Plan plan)
	{
		if (_planButtons.ContainsKey(plan.id))
			return;

		var button = Instantiate(buttonPrefab, contentHolder);
		button.Init(plan, PurchasePlan);

		_planButtons.Add(plan.id, button);
	}

	private void PurchasePlan(Plan plan)
	{
		_purchasely.Purchase(plan.id, OnPlanPurchaseSuccess, Debug.LogError);
	}

	private void OnPlanPurchaseSuccess(Plan plan)
	{
		Debug.Log("Plan purchased successfully - " + plan.id);
	}
}