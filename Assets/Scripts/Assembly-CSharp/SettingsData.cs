using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

public class SettingsData
{
	private string m_fileName;

	private bool m_useEncryption;

	private Dictionary<string, object> m_data = new Dictionary<string, object>();

	private CryptoUtility m_crypto;

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

	public SettingsData(string fileName, bool useEncryption, string key)
	{
		m_fileName = fileName;
		m_useEncryption = useEncryption;
		m_crypto = new CryptoUtility(key);
	}

	public T Get<T>(string key, T defaultValue)
	{
		object value;
		if (m_data.TryGetValue(key, out value))
		{
			return (T)value;
		}
		return defaultValue;
	}

	public void SetInt(string key, int value)
	{
		m_data[key] = value;
	}

	public int GetInt(string key, int defaultValue)
	{
		return Get(key, defaultValue);
	}

	public int AddToInt(string key, int delta, int minValue = int.MinValue, int maxValue = int.MaxValue)
	{
		object value;
		if (m_data.TryGetValue(key, out value))
		{
			int num = Mathf.Clamp((int)value + delta, minValue, maxValue);
			m_data[key] = num;
			return num;
		}
		int num2 = Mathf.Clamp(delta, minValue, maxValue);
		m_data[key] = num2;
		return num2;
	}

	public void SetFloat(string key, float value)
	{
		m_data[key] = value;
	}

	public float GetFloat(string key, float defaultValue)
	{
		return Get(key, defaultValue);
	}

	public void SetString(string key, string value)
	{
		m_data[key] = value;
	}

	public string GetString(string key, string defaultValue)
	{
		return Get(key, defaultValue);
	}

	public void SetBool(string key, bool value)
	{
		m_data[key] = value;
	}

	public bool GetBool(string key, bool defaultValue)
	{
		return Get(key, defaultValue);
	}

	public bool HasKey(string key)
	{
		return m_data.ContainsKey(key);
	}

	public void DeleteKey(string key)
	{
		m_data.Remove(key);
	}

	public void DeleteAll()
	{
		m_data.Clear();
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
		foreach (KeyValuePair<string, object> datum in m_data)
		{
			xmlWriter.WriteStartElement(datum.Value.GetType().Name);
			xmlWriter.WriteAttributeString("key", datum.Key);
			xmlWriter.WriteAttributeString("value", datum.Value.ToString());
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

	public void Load()
	{
		if (!File.Exists(m_fileName))
		{
			return;
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
		}
		catch (IOException ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	public void LoadXml(Stream stream)
	{
		XmlTextReader xmlReader = new XmlTextReader(stream);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(xmlReader);
		XmlNodeList childNodes = xmlDocument.DocumentElement.ChildNodes;
		m_data.Clear();
		foreach (XmlNode item in childNodes)
		{
			XmlAttributeCollection attributes = item.Attributes;
			XmlAttribute xmlAttribute = attributes["key"];
			XmlAttribute xmlAttribute2 = attributes["value"];
			if (item.Name == "Int32")
			{
				int result;
				if (int.TryParse(xmlAttribute2.Value, out result))
				{
					SetInt(xmlAttribute.Value, result);
				}
			}
			else if (item.Name == "Single")
			{
				float result2;
				if (float.TryParse(xmlAttribute2.Value, out result2))
				{
					SetFloat(xmlAttribute.Value, result2);
				}
			}
			else if (item.Name == "Boolean")
			{
				bool result3;
				if (bool.TryParse(xmlAttribute2.Value, out result3))
				{
					SetBool(xmlAttribute.Value, result3);
				}
			}
			else if (item.Name == "String")
			{
				SetString(xmlAttribute.Value, xmlAttribute2.Value);
			}
		}
	}
}
