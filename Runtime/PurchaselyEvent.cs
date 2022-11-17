namespace Purchasely
{
	public class PurchaselyEvent
	{
		public string Id { get; }
		public string Name { get; }
		public string PropertiesJson { get; }

		internal PurchaselyEvent(string id, string name, string propertiesJson)
		{
			Id = id;
			Name = name;
			PropertiesJson = propertiesJson;
		}
	}
}