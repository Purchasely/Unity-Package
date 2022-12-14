using System;

namespace PurchaselyRuntime
{
	[Flags]
	public enum Store
	{
		Google = 2,
		Amazon = 4,
		Huawei = 8
	}

	public enum LogLevel
	{
		Debug = 0,
		Info = 1,
		Warn = 2,
		Error = 3
	}

	public enum RunningMode
	{
		Observer = 0,
		PaywallObserver = 1,
		PaywallOnly = 2,
		TransactionOnly = 3,
		Full = 4
	}

	public enum ProductViewResult
	{
		Purchased = 0,
		Restored = 1,
		Cancelled = 2
	}
}