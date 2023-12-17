using UnityEngine;

public class DeviceInfo : MonoBehaviour
{
	public enum DeviceFamily
	{
		Ios = 0,
		Android = 1,
		Pc = 2,
		Osx = 3
	}

	private DeviceFamily deviceFamily = DeviceFamily.Pc;

	private static DeviceInfo instance;

	public bool UsesTouchInput
	{
		get
		{
			if ((deviceFamily == DeviceFamily.Ios || deviceFamily == DeviceFamily.Android) && !Application.isEditor)
			{
				return true;
			}
			return false;
		}
	}

	public DeviceFamily ActiveDeviceFamily
	{
		get
		{
			return deviceFamily;
		}
	}

	public static DeviceInfo Instance
	{
		get
		{
			return instance;
		}
	}

	public string PersistentDataPath()
	{
		if (!Application.isEditor)
		{
			return GetiPhoneDocumentsPath();
		}
		return Application.persistentDataPath;
	}

	private bool RequireHiResTextures()
	{
		if (Screen.currentResolution.height > 768)
		{
			return true;
		}
		return false;
	}

	public static bool IsInstantiated()
	{
		return instance;
	}

	private void Awake()
	{
		deviceFamily = DeviceFamily.Ios;
		Debug.Log("Device info: Active device family: " + ActiveDeviceFamily);
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		Object.DontDestroyOnLoad(this);
	}

	public static string GetiPhoneDocumentsPath()
	{
		string text = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
		text = text.Substring(0, text.LastIndexOf('/'));
		return text + "/Documents";
	}
}
