namespace DynamicBox.EventManagement.GameEvents
{
	public class BuyIAPItemEvent : GameEvent
	{
		public readonly string ProductID;

		public BuyIAPItemEvent (string productID)
		{
			ProductID = productID;
		}
	}
}