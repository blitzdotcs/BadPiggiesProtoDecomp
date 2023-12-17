public struct InputEvent
{
	public enum EventType
	{
		Press = 0,
		Release = 1,
		MouseEnter = 2,
		MouseLeave = 3,
		MouseReturn = 4
	}

	public EventType type;

	public InputEvent(EventType type)
	{
		this.type = type;
	}
}
