using UnityEngine;

public class GameProgress : MonoBehaviour
{
	private static SettingsData m_data;

	private void Awake()
	{
		Assert.Check(m_data == null, "Created two instances of GameProgress");
		m_data = new SettingsData(DeviceInfo.Instance.PersistentDataPath() + "/Progress.dat", true, "56SA%FG42Dv5#4aG67f2");
		m_data.Load();
		Object.DontDestroyOnLoad(this);
		if (!GetBool("GameProgress_initialized"))
		{
			InitializeGameProgressData();
		}
	}

	public static void InitializeGameProgressData()
	{
		SetInt("Blueprints_Available", 3);
		SetBool("GameProgress_initialized", true);
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			m_data.Save();
		}
	}

	public static void SetLevelCompleted(string levelName)
	{
		m_data.SetInt(levelName + "_completed", 1);
	}

	public static bool IsLevelCompleted(string levelName)
	{
		return m_data.GetInt(levelName + "_completed", 0) == 1;
	}

	public static void AddSandboxStar(string levelName, string starName)
	{
		string key = levelName + "_star_" + starName;
		if (m_data.GetInt(key, 0) == 0)
		{
			m_data.SetInt(key, 1);
			string key2 = levelName + "_stars";
			int @int = m_data.GetInt(key2, 0);
			@int++;
			m_data.SetInt(key2, @int);
		}
	}

	public static bool HasSandboxStar(string levelName, string starName)
	{
		string key = levelName + "_star_" + starName;
		return m_data.GetInt(key, 0) > 0;
	}

	public static int SandboxStarCount(string levelName)
	{
		string key = levelName + "_stars";
		return m_data.GetInt(key, 0);
	}

	public static int AddSandboxParts(BasePart.PartType part, int count)
	{
		return m_data.AddToInt("part_" + part, count);
	}

	public static int GetSandboxPartCount(BasePart.PartType part)
	{
		return m_data.GetInt("part_" + part, 0);
	}

	public static void SetInt(string key, int value)
	{
		m_data.SetInt(key, value);
	}

	public static int GetInt(string key, int defaultValue = 0)
	{
		return m_data.GetInt(key, defaultValue);
	}

	public static void SetFloat(string key, float value)
	{
		m_data.SetFloat(key, value);
	}

	public static float GetFloat(string key, float defaultValue = 0f)
	{
		return m_data.GetFloat(key, defaultValue);
	}

	public static void SetString(string key, string value)
	{
		m_data.SetString(key, value);
	}

	public static string GetString(string key, string defaultValue = "")
	{
		return m_data.GetString(key, defaultValue);
	}

	public static void SetBool(string key, bool value)
	{
		m_data.SetBool(key, value);
	}

	public static bool GetBool(string key, bool defaultValue = false)
	{
		return m_data.GetBool(key, defaultValue);
	}

	public static bool HasKey(string key)
	{
		return m_data.HasKey(key);
	}

	public static void DeleteKey(string key)
	{
		m_data.DeleteKey(key);
	}

	public static void DeleteAll()
	{
		m_data.DeleteAll();
	}

	public static void Save()
	{
		m_data.Save();
	}

	public static void Load()
	{
		m_data.Load();
	}
}
