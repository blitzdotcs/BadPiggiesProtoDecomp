using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

public class AchievementData : MonoBehaviour
{
	public struct AchievementDataHolder
	{
		public double progress;

		public bool completed;
	}

	[Serializable]
	public class AchievementDescriptor
	{
		public string id;

		public Texture2D icon;

		public double limit;

		public double debugLimit;
	}

	private static AchievementData instance;

	private bool m_limitsInitialized;

	private string m_fileName;

	private bool m_useEncryption;

	private CryptoUtility m_crypto;

	private Dictionary<string, AchievementDataHolder> m_achievementData = new Dictionary<string, AchievementDataHolder>();

	[SerializeField]
	private List<AchievementDescriptor> m_achievementList = new List<AchievementDescriptor>();

	private Dictionary<string, AchievementDescriptor> m_achievementLimits = new Dictionary<string, AchievementDescriptor>();

	public string FileName
	{
		get
		{
			return m_fileName;
		}
		set
		{
			m_fileName = value;
		}
	}

	public Dictionary<string, AchievementDescriptor> AchievementsLimits
	{
		get
		{
			return m_achievementLimits;
		}
	}

	public Dictionary<string, double> AchievementsProgress
	{
		get
		{
			Dictionary<string, double> dictionary = new Dictionary<string, double>();
			foreach (KeyValuePair<string, AchievementDataHolder> achievementDatum in m_achievementData)
			{
				dictionary.Add(achievementDatum.Key, achievementDatum.Value.progress);
			}
			return dictionary;
		}
	}

	public static AchievementData Instance
	{
		get
		{
			return instance;
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			Save();
		}
	}

	public void Save()
	{
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.Indent = true;
		xmlWriterSettings.IndentChars = "  ";
		xmlWriterSettings.NewLineChars = "\r\n";
		xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
		xmlWriterSettings.Encoding = Encoding.UTF8;
		MemoryStream memoryStream = new MemoryStream();
		XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
		xmlWriter.WriteStartDocument();
		xmlWriter.WriteStartElement("data");
		foreach (KeyValuePair<string, AchievementDataHolder> achievementDatum in m_achievementData)
		{
			xmlWriter.WriteStartElement("Achievement");
			xmlWriter.WriteAttributeString("id", achievementDatum.Key);
			xmlWriter.WriteAttributeString("progress", achievementDatum.Value.progress.ToString());
			xmlWriter.WriteAttributeString("completed", achievementDatum.Value.completed.ToString());
			xmlWriter.WriteEndElement();
		}
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndDocument();
		xmlWriter.Close();
		byte[] array = memoryStream.ToArray();
		if (!m_useEncryption)
		{
			FileStream fileStream = new FileStream(m_fileName, FileMode.Create);
			fileStream.Write(array, 0, array.Length);
			fileStream.Close();
			return;
		}
		byte[] array2 = m_crypto.Encrypt(array);
		byte[] array3 = CryptoUtility.ComputeHash(array2);
		FileStream fileStream2 = new FileStream(m_fileName, FileMode.Create);
		fileStream2.Write(array3, 0, array3.Length);
		fileStream2.Write(array2, 0, array2.Length);
		fileStream2.Close();
	}

	public bool Load()
	{
		if (!File.Exists(m_fileName))
		{
			return false;
		}
		try
		{
			FileStream fileStream = new FileStream(m_fileName, FileMode.Open);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			byte[] buffer;
			if (m_useEncryption)
			{
				if (array.Length < 20)
				{
					throw new IOException("Corrupted data file: could not read hash");
				}
				byte[] array2 = m_crypto.ComputeHash(array, 20, array.Length - 20);
				for (int i = 0; i < 20; i++)
				{
					if (array2[i] != array[i])
					{
						throw new IOException("Corrupted data file");
					}
				}
				buffer = m_crypto.Decrypt(array, 20);
			}
			else
			{
				buffer = array;
			}
			MemoryStream stream = new MemoryStream(buffer);
			LoadXml(stream);
			return true;
		}
		catch (IOException ex)
		{
			Debug.LogError(ex.ToString());
		}
		return false;
	}

	private void LoadXml(Stream stream)
	{
		XmlTextReader xmlReader = new XmlTextReader(stream);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(xmlReader);
		XmlNodeList childNodes = xmlDocument.DocumentElement.ChildNodes;
		m_achievementData.Clear();
		AchievementDataHolder adh = default(AchievementDataHolder);
		foreach (XmlNode item in childNodes)
		{
			XmlAttributeCollection attributes = item.Attributes;
			XmlAttribute xmlAttribute = attributes["id"];
			XmlAttribute xmlAttribute2 = attributes["progress"];
			XmlAttribute xmlAttribute3 = attributes["completed"];
			double.TryParse(xmlAttribute2.Value, out adh.progress);
			bool.TryParse(xmlAttribute3.Value, out adh.completed);
			SetAchievement(xmlAttribute.Value, adh);
		}
	}

	public void SetAchievement(string id, AchievementDataHolder adh)
	{
		m_achievementData[id] = adh;
		Save();
	}

	public AchievementDataHolder GetAchievement(string id)
	{
		return m_achievementData[id];
	}

	private void InitializeAchievementLimits()
	{
		foreach (AchievementDescriptor achievement in m_achievementList)
		{
			m_achievementLimits.Add(achievement.id, achievement);
		}
		m_limitsInitialized = true;
	}

	public int GetAchievementLimit(string id)
	{
		if (!m_limitsInitialized)
		{
			InitializeAchievementLimits();
		}
		AchievementDescriptor value;
		m_achievementLimits.TryGetValue(id, out value);
		if (BuildCustomizationLoader.Instance.IsDebugBuild)
		{
			return (int)value.debugLimit;
		}
		return (int)value.limit;
	}

	public static bool IsInstantiated()
	{
		return instance;
	}

	private void Awake()
	{
		Assert.Check(instance == null, "Singleton " + base.name + " spawned twice");
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		m_fileName = DeviceInfo.Instance.PersistentDataPath() + "/Achievements.xml";
		m_useEncryption = true;
		m_crypto = new CryptoUtility("fHHg5#%3RRfnJi78&%lP?65");
		InitializeAchievementLimits();
		if (Load())
		{
			return;
		}
		AchievementDataHolder value = default(AchievementDataHolder);
		foreach (KeyValuePair<string, AchievementDescriptor> achievementLimit in m_achievementLimits)
		{
			value.progress = 0.0;
			value.completed = false;
			m_achievementData.Add(achievementLimit.Key, value);
		}
		Save();
	}
}
