using UnityEngine;

public class URLManager : MonoBehaviour
{
	public enum LinkType
	{
		Youtube = 0,
		Facebook = 1,
		Twitter = 2
	}

	private static URLManager instance;

	private string m_baseURLString;

	public static URLManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		instance = this;
	}

	private void Start()
	{
		GenerateURLBaseString();
	}

	private void GenerateURLBaseString()
	{
		m_baseURLString = "http://cloud.rovio.com/link/redirect/?p=bpc&r=game";
		switch (DeviceInfo.Instance.ActiveDeviceFamily)
		{
		case DeviceInfo.DeviceFamily.Ios:
			if (BuildCustomizationLoader.Instance.IsHDVersion)
			{
				if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad3Gen)
				{
					m_baseURLString += "&d=ipad3";
				}
				else
				{
					m_baseURLString += "&d=ipad";
				}
			}
			else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4 || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4S)
			{
				m_baseURLString += "&d=iphone4";
			}
			else
			{
				m_baseURLString += "&d=iphone";
			}
			break;
		case DeviceInfo.DeviceFamily.Android:
			m_baseURLString += "&d=android";
			break;
		case DeviceInfo.DeviceFamily.Osx:
			m_baseURLString += "&d=osx";
			break;
		case DeviceInfo.DeviceFamily.Pc:
			m_baseURLString += "&d=windows";
			break;
		}
		if (BuildCustomizationLoader.Instance.IsFreeVersion)
		{
			m_baseURLString += "&a=free";
		}
		else
		{
			m_baseURLString += "&a=full";
		}
		m_baseURLString = m_baseURLString + "&v=" + BuildCustomizationLoader.Instance.ApplicationVersion;
		m_baseURLString = m_baseURLString + "&c=" + BuildCustomizationLoader.Instance.CustomerID;
		m_baseURLString = m_baseURLString + "&i=" + SystemInfo.deviceUniqueIdentifier;
	}

	public void OpenURL(LinkType type)
	{
		string text = "&t=";
		switch (type)
		{
		case LinkType.Youtube:
			text += "youtube";
			break;
		case LinkType.Facebook:
			text += "facebook";
			break;
		case LinkType.Twitter:
			text += "twitter";
			break;
		}
		Application.OpenURL(m_baseURLString + text);
		Debug.Log(m_baseURLString + text);
	}
}
