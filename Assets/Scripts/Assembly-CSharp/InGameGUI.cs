using UnityEngine;

public class InGameGUI : MonoBehaviour
{
	public GameObject buildMenuPrefab;

	public GameObject flightMenuPrefab;

	public GameObject previewMenuPrefab;

	public GameObject pauseMenuPrefab;

	public GameObject levelCompleteMenuPrefab;

	private GameObject buildMenu;

	private GameObject flightMenu;

	private GameObject previewMenu;

	private GameObject pauseMenu;

	private GameObject levelCompleteMenu;

	private GameObject currentMenu;

	private void Awake()
	{
		buildMenu = InstantiateMenu(buildMenuPrefab);
		flightMenu = InstantiateMenu(flightMenuPrefab);
		previewMenu = InstantiateMenu(previewMenuPrefab);
		pauseMenu = InstantiateMenu(pauseMenuPrefab);
		levelCompleteMenu = InstantiateMenu(levelCompleteMenuPrefab);
	}

	private GameObject InstantiateMenu(GameObject prefab)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(prefab);
		gameObject.name = prefab.name;
		gameObject.transform.position = base.transform.position;
		gameObject.transform.parent = base.transform;
		gameObject.SetActiveRecursively(false);
		return gameObject;
	}

	private void OnEnable()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChangedEvent);
	}

	private void OnDisable()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChangedEvent);
	}

	private void ReceiveGameStateChangedEvent(GameStateChanged data)
	{
		switch (data.state)
		{
		case LevelManager.GameState.Building:
			SetMenu(buildMenu);
			break;
		case LevelManager.GameState.Running:
			SetMenu(flightMenu);
			break;
		case LevelManager.GameState.PreviewWhileBuilding:
			SetMenu(previewMenu);
			break;
		case LevelManager.GameState.PreviewWhileRunning:
			SetMenu(previewMenu);
			break;
		case LevelManager.GameState.PausedWhileBuilding:
			SetMenu(pauseMenu);
			break;
		case LevelManager.GameState.PausedWhileRunning:
			SetMenu(pauseMenu);
			break;
		case LevelManager.GameState.Completed:
			SetMenu(levelCompleteMenu);
			break;
		case LevelManager.GameState.Preview:
		case LevelManager.GameState.PreviewMoving:
		case LevelManager.GameState.Continue:
			break;
		}
	}

	private void SetMenu(GameObject menu)
	{
		HideCurrentMenu();
		currentMenu = menu;
		if ((bool)currentMenu)
		{
			currentMenu.SetActiveRecursively(true);
		}
	}

	public void HideCurrentMenu()
	{
		if ((bool)currentMenu)
		{
			currentMenu.SetActiveRecursively(false);
		}
	}
}
