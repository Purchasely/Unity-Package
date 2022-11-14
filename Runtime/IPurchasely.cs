namespace Purchasely
{
	public interface IPurchasely
	{
		void Init(string userId, bool readyToPurchase, int storeFlags, int logLevel, int runningMode);
	}
}