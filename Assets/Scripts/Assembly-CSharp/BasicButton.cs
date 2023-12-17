using UnityEngine;

public class BasicButton : UIButton
{
	public enum Type
	{
		Building = 0,
		Play = 1,
		LevelSelection = 2,
		NextLevel = 3,
		Home = 4,
		Preview = 5,
		Clear = 6,
		Pause = 7,
		Blueprint = 8,
		Retry = 9,
		Rockets = 10,
		Engines = 11
	}

	public Type m_type;

	protected override void OnTouchRelease()
	{
		base.OnTouchRelease();
		Execute();
	}

	protected void Execute()
	{
		switch (m_type)
		{
		case Type.Building:
			WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Building);
			break;
		case Type.Play:
		{
			LevelManager.GameState gameState2 = ((WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Building) ? LevelManager.GameState.Continue : LevelManager.GameState.Running);
			WPFMonoBehaviour.levelManager.SetGameState(gameState2);
			break;
		}
		case Type.Home:
			Loader.Instance.LoadLevel("MainMenu", false);
			break;
		case Type.Retry:
			Loader.Instance.LoadLevel(Application.loadedLevelName, true);
			break;
		case Type.NextLevel:
			GameManager.Instance.LoadNextLevel();
			break;
		case Type.Clear:
			WPFMonoBehaviour.levelManager.constructionUI.ClearContraption();
			break;
		case Type.Preview:
		{
			LevelManager.GameState gameState = ((WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Running) ? LevelManager.GameState.PreviewWhileBuilding : LevelManager.GameState.PreviewWhileRunning);
			WPFMonoBehaviour.levelManager.SetGameState(gameState);
			break;
		}
		case Type.Rockets:
		{
			Rocket[] componentsInChildren2 = WPFMonoBehaviour.levelManager.contraptionRunning.GetComponentsInChildren<Rocket>();
			Rocket[] array = componentsInChildren2;
			foreach (Rocket rocket in array)
			{
				rocket.ProcessTouch();
			}
			break;
		}
		case Type.Engines:
		{
			Engine[] componentsInChildren = WPFMonoBehaviour.levelManager.contraptionRunning.GetComponentsInChildren<Engine>();
			componentsInChildren[0].ProcessTouch();
			break;
		}
		case Type.LevelSelection:
		case Type.Pause:
		case Type.Blueprint:
			break;
		}
	}
}
