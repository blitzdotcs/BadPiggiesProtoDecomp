using System.Xml;
using UnityEngine;

public class BuildCustomizationLoader : MonoBehaviour
{
	public GameObject m_fpsCounter;

	private static BuildCustomizationLoader instance;

	private bool m_rovionews;

	private bool m_ads;

	private bool m_flurry;

	private bool m_iapEnabled;

	private bool m_isHDBuild;

	private bool m_isDebugBuild;

	private string m_svnRevisionNumber;

	private string m_applicationVersion;

	private string m_customerId;

	public bool RovioNews
	{
		get
		{
			return m_rovionews;
		}
	}

	public bool AdsEnabled
	{
		get
		{
			return m_ads;
		}
		set
		{
			m_ads = value;
		}
	}

	public bool IAPEnabled
	{
		get
		{
			return m_iapEnabled;
		}
	}

	public bool Flurry
	{
		get
		{
			return m_flurry;
		}
	}

	public string CustomerID
	{
		get
		{
			return m_customerId;
		}
	}

	public string SVNRevisionNumber
	{
		get
		{
			return m_svnRevisionNumber;
		}
	}

	public string ApplicationVersion
	{
		get
		{
			return m_applicationVersion;
		}
	}

	public bool IsFreeVersion
	{
		get
		{
			return DeviceInfo.Instance.ActiveDeviceFamily == DeviceInfo.DeviceFamily.Android && !GameProgress.GetBool("FullVersionUnlocked");
		}
	}

	public bool IsHDVersion
	{
		get
		{
			return m_isHDBuild;
		}
	}

	public bool IsDebugBuild
	{
		get
		{
			return m_isDebugBuild;
		}
		set
		{
			m_isDebugBuild = value;
		}
	}

	public static BuildCustomizationLoader Instance
	{
		get
		{
			return instance;
		}
	}

	private void PopulateCustomizationData(XmlDocument buildXml)
	{
		XmlNode xmlNode = buildXml.SelectSingleNode("/build/parameters/rovio_news");
		XmlNode xmlNode2 = buildXml.SelectSingleNode("/build/parameters/ads");
		XmlNode xmlNode3 = buildXml.SelectSingleNode("/build/parameters/flurry");
		XmlNode xmlNode4 = buildXml.SelectSingleNode("/build/parameters/customer_id");
		XmlNode xmlNode5 = buildXml.SelectSingleNode("/build/parameters/iap_enabled");
		XmlNode xmlNode6 = buildXml.SelectSingleNode("/build/parameters/hd_build");
		XmlNode xmlNode7 = buildXml.SelectSingleNode("/build/parameters/svn_revision");
		XmlNode xmlNode8 = buildXml.SelectSingleNode("/build/parameters/application_version");
		XmlNode xmlNode9 = buildXml.SelectSingleNode("/build/parameters/debug_build");
		if (xmlNode != null)
		{
			m_rovionews = ((xmlNode["enabled"].InnerText == "true") ? true : false);
		}
		else
		{
			m_rovionews = false;
		}
		if (xmlNode2 != null)
		{
			m_ads = ((xmlNode2["enabled"].InnerText == "true") ? true : false);
		}
		else
		{
			m_ads = false;
		}
		if (xmlNode3 != null)
		{
			m_flurry = ((xmlNode3["enabled"].InnerText == "true") ? true : false);
		}
		else
		{
			m_flurry = false;
		}
		if (xmlNode5 != null)
		{
			m_iapEnabled = ((xmlNode5["enabled"].InnerText == "true") ? true : false);
		}
		else
		{
			m_iapEnabled = false;
		}
		if (xmlNode6 != null)
		{
			m_isHDBuild = ((xmlNode6.InnerText == "true") ? true : false);
		}
		else
		{
			m_isHDBuild = false;
		}
		if (xmlNode9 != null)
		{
			m_isDebugBuild = ((xmlNode9.InnerText == "true") ? true : false);
		}
		else
		{
			m_isDebugBuild = false;
		}
		if (xmlNode8 != null)
		{
			m_applicationVersion = xmlNode8.InnerText;
		}
		else
		{
			m_applicationVersion = "1.0.0";
		}
		if (xmlNode7 != null)
		{
			m_svnRevisionNumber = xmlNode7.InnerText;
		}
		else
		{
			m_svnRevisionNumber = "1";
		}
		if (xmlNode4 != null)
		{
			m_customerId = xmlNode4.InnerText;
		}
		else
		{
			m_customerId = "Rovio";
		}
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
		XmlDocument xmlDocument = new XmlDocument();
		TextAsset textAsset = (TextAsset)Resources.Load("Build/customization_data", typeof(TextAsset));
		xmlDocument.LoadXml(textAsset.text);
		PopulateCustomizationData(xmlDocument);
		if (IsDebugBuild)
		{
			Object.Instantiate(m_fpsCounter);
		}
	}
}
