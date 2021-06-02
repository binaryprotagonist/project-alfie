namespace DynamicBox.EventManagement.GameEvents.VoiceOver
{
	public class NewLetterSelectedEvent : GameEvent
	{
		public readonly bool Value;

		public NewLetterSelectedEvent (bool value)
		{
			Value = value;
		}
	}
}