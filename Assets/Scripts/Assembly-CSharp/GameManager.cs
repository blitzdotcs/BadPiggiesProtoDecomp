using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private GameData m_gameData;

	private int m_currentLevel;

	private string m_currentEpisode = string.Empty;

	private List<string> m_levels = new List<string>();

	private string m_openingCutscene;

	private string m_endingCutscene;

	private static GameManager instance;

	public GameData gameData
	{
		get
		{
			return m_gameData;
		}
	}

	public string CurrentEpisode
	{
		get
		{
			return m_currentEpisode;
		}
	}

	public int CurrentLevel
	{
		get
		{
			return m_currentLevel;
		}
	}

	public string OpeningCutscene
	{
		get
		{
			return m_openingCutscene;
		}
	}

	public string EndingCutscene
	{
		get
		{
			return m_endingCutscene;
		}
	}

	public int LevelCount
	{
		get
		{
			return m_levels.Count;
		}
	}

	public static GameManager Instance
	{
		get
		{
			return instance;
		}
	}

	public void OpenEpisode(LevelSelector episodeLevels)
	{
		m_currentEpisode = Application.loadedLevelName;
		m_openingCutscene = episodeLevels.OpeningCutscene;
		m_endingCutscene = episodeLevels.EndingCutscene;
		m_levels = new List<string>(episodeLevels.Levels);
	}

	public void OpenSandboxEpisode(SandboxSelector sandboxLevels)
	{
		m_currentEpisode = Application.loadedLevelName;
		m_levels = new List<string>(sandboxLevels.Levels);
	}

	public void CloseEpisode()
	{
		m_currentEpisode = null;
		m_levels.Clear();
	}

	public void LoadNextLevel()
	{
		Debug.Log("LoadNextLevel: " + m_currentLevel + " " + m_levels.Count);
		if (m_levels[m_currentLevel + 1] == GetCurrentRowJokerLevel())
		{
			m_currentLevel++;
		}
		if (m_currentLevel < m_levels.Count - 1)
		{
			m_currentLevel++;
			LoadLevel(m_currentLevel);
		}
		else
		{
			Loader.Instance.LoadLevel("LevelMenu", true);
		}
	}

	public void LoadLevel(int index)
	{
		Assert.Check(index >= 0 && index < m_levels.Count, "Invalid level index: " + index);
		m_currentLevel = index;
		string levelName = m_levels[index];
		Loader.Instance.LoadLevel(levelName, true);
	}

	public void LoadOpeningCutscene()
	{
		Loader.Instance.LoadLevel(OpeningCutscene, true);
	}

	public void LoadEndingCutscene()
	{
		Loader.Instance.LoadLevel(EndingCutscene, true);
	}

	public bool CurrentLevelRowThreeStarred()
	{
		bool flag = true;
		int num = m_currentLevel / 5 * 5;
		if (m_levels.Count < 5)
		{
			return false;
		}
		for (int i = num; i < num + 4; i++)
		{
			if (!flag)
			{
				break;
			}
			flag &= GameProgress.GetInt(m_levels[i] + "_stars") == 3;
		}
		return flag;
	}

	public bool CurrentEpisodeThreeStarredNormalLevels()
	{
		bool flag = true;
		for (int i = 0; i < m_levels.Count; i++)
		{
			if (!flag)
			{
				break;
			}
			flag &= GameProgress.GetInt(m_levels[i] + "_stars") == 3;
			if ((i + 1) % 5 == 0)
			{
				i++;
			}
		}
		return flag;
	}

	public bool CurrentEpisodeThreeStarredSpecialLevels()
	{
		bool flag = true;
		for (int i = 4; i < m_levels.Count; i += 5)
		{
			if (!flag)
			{
				break;
			}
			flag &= GameProgress.GetInt(m_levels[i] + "_stars") == 3;
		}
		return flag;
	}

	public bool IsLastLevelInEpisode()
	{
		return m_currentLevel == m_levels.Count - 2;
	}

	public string GetCurrentRowJokerLevel()
	{
		int num = m_currentLevel / 5 * 5;
		return (m_levels.Count <= 5) ? string.Empty : m_levels[num + 4];
	}

	public string GetCurrentRowJokerLevelNumber()
	{
		int num = m_currentLevel / 5 * 5;
		return (num + 4 + 1).ToString();
	}

	public static bool IsInstantiated()
	{
		return instance;
	}

	private void Awake()
	{
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		Object.DontDestroyOnLoad(this);
	}

	public void CreateMenuBackground()
	{
		string @string = GameProgress.GetString("MenuBackground", string.Empty);
		GameObject gameObject = null;
		gameObject = ((!(@string != string.Empty)) ? ((GameObject)Resources.Load("Environment/Background/Background_Jungle_01_SET", typeof(GameObject))) : ((GameObject)Resources.Load("Environment/Background/" + @string, typeof(GameObject))));
		Object.Instantiate(gameObject, Vector3.forward * 10f, Quaternion.identity);
	}
}
