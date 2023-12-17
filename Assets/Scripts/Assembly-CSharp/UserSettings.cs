using UnityEngine;

public class UserSettings : MonoBehaviour
{
	private static SettingsData m_data;

	private void Awake()
	{
		Assert.Check(m_data == null, "Created two instances of UserSettings");
		m_data = new SettingsData(DeviceInfo.Instance.PersistentDataPath() + "/Settings.xml", false, string.Empty);
		m_data.Load();
		Object.DontDestroyOnLoad(this);
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
