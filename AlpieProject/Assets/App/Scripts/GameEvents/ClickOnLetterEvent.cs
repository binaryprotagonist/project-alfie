namespace DynamicBox.EventManagement.GameEvents
{
	public class ClickOnLetterEvent : GameEvent
	{
		public readonly bool Value;

		public ClickOnLetterEvent (bool value)
		{
			Value = value;
		}
	}
}