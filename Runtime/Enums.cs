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
        FIREBASE_APP_INSTANCE_ID = 0,
        AIRSHIP_CHANNEL_ID = 1,
        AIRSHIP_USER_ID = 2,
        BATCH_INSTALLATION_ID = 3,
        ADJUST_ID = 4,
        APPSFLYER_ID = 5,
        ONESIGNAL_PLAYER_ID = 6,
        MIXPANEL_DISTINCT_ID = 7,
        CLEVER_TAP_ID = 8,
        SENDINBLUE_USER_EMAIL = 9,
        ITERABLE_USER_EMAIL = 10,
        ITERABLE_USER_ID = 11,
        AT_INTERNET_ID_CLIENT = 12,
        MPARTICLE_USER_ID = 13,
        CUSTOMERIO_USER_ID = 14,
        CUSTOMERIO_USER_EMAIL = 15,
        BRANCH_USER_DEVELOPER_IDENTITY = 16,
        AMPLITUDE_USER_ID = 17,
        AMPLITUDE_DEVICE_ID = 18,
        MOENGAGE_UNIQUE_ID = 19,
        ONESIGNAL_EXTERNAL_ID = 20,
    }
}