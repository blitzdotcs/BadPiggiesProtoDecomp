public struct UIEvent : EventManager.Event
{
	public enum Type
	{
		None = 0,
		Building = 1,
		Play = 2,
		LevelSelection = 3,
		NextLevel = 4,
		Home = 5,
		Preview = 6,
		Clear = 7,
		Pause = 8,
		Blueprint = 9,
		ReplayLevel = 10,
		ActivateRockets = 11,
		ActivateEngines = 12,
		BackFromPreview = 13,
		ContinueFromPause = 14,
		ReplayFlight = 15,
		IapPurchaseCurrency = 16,
		IapPurchaseRocket = 17,
		IapPurchaseEngine = 18,
		OpenIapMenu = 19,
		CloseIapMenu = 20,
		QuestModeBuild = 21,
		MoveContraptionLeft = 22,
		MoveContraptionRight = 23,
		MoveContraptionUp = 24,
		MoveContraptionDown = 25
	}

	public Type type;

	public UIEvent(Type type)
	{
		this.type = type;
	}
}
