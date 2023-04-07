using System;
using PurchaselyRuntime;
using UnityEngine;
using UnityEngine.UI;

public class PurchaselyDemoPlanButton : MonoBehaviour
{
	[SerializeField] private Text text;
	[SerializeField] private Button button;

	private Plan _plan;
	private Action<Plan> _onClicked;

	public void Init(Plan plan, Action<Plan> onClicked)
	{
		_plan = plan;
		_onClicked = onClicked;

		text.text = $"{plan.name} - {plan.price}{plan.currencySymbol}";
	}

	private void Start()
	{
		button.onClick.AddListener(OnClicked);
	}

	private void OnClicked()
	{
		if (_plan == null)
			return;

		_onClicked(_plan);
	}
}