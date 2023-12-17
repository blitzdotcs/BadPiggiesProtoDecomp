using System;

public static class EventManager
{
	public interface Event
	{
	}

	private class EventTypeManager<T> where T : Event
	{
		public static OnEvent<T> handler;
	}

	public delegate void OnEvent<T>(T data) where T : Event;

	public static void Send<T>(T data) where T : Event
	{
		OnEvent<T> handler = EventTypeManager<T>.handler;
		if (handler != null)
		{
			handler(data);
		}
	}

	public static void Connect<T>(OnEvent<T> handler) where T : Event
	{
		EventTypeManager<T>.handler = (OnEvent<T>)Delegate.Combine(EventTypeManager<T>.handler, handler);
	}

	public static void Disconnect<T>(OnEvent<T> handler) where T : Event
	{
		EventTypeManager<T>.handler = (OnEvent<T>)Delegate.Remove(EventTypeManager<T>.handler, handler);
	}
}
