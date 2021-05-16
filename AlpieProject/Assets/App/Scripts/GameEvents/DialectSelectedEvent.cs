namespace DynamicBox.EventManagement.GameEvents
{
	public class DialectSelectedEvent : GameEvent
	{
		public readonly int ActiveDialectIndex;

		public DialectSelectedEvent (int activeDialectIndex)
		{
			ActiveDialectIndex = activeDialectIndex;
		}
	}
}