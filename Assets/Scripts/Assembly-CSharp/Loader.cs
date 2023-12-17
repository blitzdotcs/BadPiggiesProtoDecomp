using System.Collections;
using UnityEngine;

public class Loader : MonoBehaviour
{
	private static Loader instance;

	private Vector3 originalPosition = Vector3.zero;

	private string m_lastLoadedLevel = string.Empty;

	public static Loader Instance
	{
		get
		{
			return instance;
		}
	}

	public string LastLoadedString
	{
		get
		{
			return m_lastLoadedLevel;
		}
	}

	public void LoadLevel(string levelName, bool showLoadingScreen)
	{
		m_lastLoadedLevel = levelName;
		if (showLoadingScreen)
		{
			Show();
		}
		else
		{
			base.gameObject.active = true;
		}
		GameProgress.Save();
		StartCoroutine(LoadLevelAsync(levelName));
	}

	private IEnumerator LoadLevelAsync(string levelName)
	{
		yield return Application.LoadLevelAsync(levelName);
		Debug.Log("Level loaded: " + levelName);
	}

	private void Awake()
	{
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		Object.DontDestroyOnLoad(this);
		originalPosition = base.transform.position;
	}

	private void Start()
	{
		Hide();
	}

	private void Show()
	{
		RepositionToNearplane();
		base.gameObject.SetActiveRecursively(true);
	}

	private void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	private void RepositionToNearplane()
	{
		GameObject gameObject = GameObject.Find("HUDCamera");
		if (!gameObject)
		{
			gameObject = GameObject.Find("Main Camera");
		}
		if ((bool)gameObject && (bool)gameObject.GetComponent<Camera>())
		{
			float z = gameObject.transform.position.z + gameObject.GetComponent<Camera>().nearClipPlane * 2f;
			base.transform.position = new Vector3(originalPosition.x, originalPosition.y - gameObject.transform.InverseTransformPoint(0f, 0f, 0f).y, z);
		}
	}

	private void OnLevelWasLoaded(int levelIndex)
	{
		Hide();
		RepositionToNearplane();
	}
}
