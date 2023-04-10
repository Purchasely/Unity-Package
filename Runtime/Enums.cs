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

	public enum PresentationType
	{
		Unknown = 0,
		Normal = 1,
		Fallback = 2,
		Deactivated = 3,
		Client = 4
	}
}