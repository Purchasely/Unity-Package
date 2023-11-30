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

    public enum PLYAttribute
    {
		AMPLITUDE_USER_ID = 0,
		AMPLITUDE_DEVICE_ID = 1,
		FIREBASE_APP_INSTANCE_ID = 2,
		AIRSHIP_CHANNEL_ID = 3,
		AIRSHIP_USER_ID = 4,
		BATCH_INSTALLATION_ID = 5,
		ADJUST_ID = 6,
		APPSFLYER_ID = 7,
		ONESIGNAL_PLAYER_ID = 8,
		MIXPANEL_DISTINCT_ID = 9,
		CLEVER_TAP_ID = 10,
		SENDINBLUE_USER_EMAIL = 11,
		ITERABLE_USER_ID = 12,
		ITERABLE_USER_EMAIL = 13,
		AT_INTERNET_ID_CLIENT = 14,
		MPARTICLE_USER_ID = 15,
		BRANCH_USER_DEVELOPER_IDENTITY = 16,
		CUSTOMERIO_USER_EMAIL = 17,
		CUSTOMERIO_USER_ID = 18,
		MOENGAGE_UNIQUE_ID = 19,

    }
}