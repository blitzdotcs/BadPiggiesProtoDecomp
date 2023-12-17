public struct GameStateChanged : EventManager.Event
{
	public LevelManager.GameState state;

	public GameStateChanged(LevelManager.GameState state)
	{
		this.state = state;
	}
}
