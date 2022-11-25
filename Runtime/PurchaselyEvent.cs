namespace PurchaselyRuntime
{
	public class PurchaselyEvent
	{
		public string Name { get; }
		public string PropertiesJson { get; }

		internal PurchaselyEvent(string name, string propertiesJson)
		{
			Name = name;
			PropertiesJson = propertiesJson;
		}
	}
}