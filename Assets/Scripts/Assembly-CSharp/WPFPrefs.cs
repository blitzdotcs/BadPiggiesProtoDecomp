using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public class WPFPrefs : UnityEngine.Object
{
	private static CryptoUtility m_crypto = new CryptoUtility("3b91A049Ca7HvSjhxT35");

	public static void WriteGhostPlayerData(string filename, GhostPlayer gp)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(GhostPlayer));
		FileStream fileStream = new FileStream(LevelManager.kDataPath + "/" + filename, FileMode.Create);
		xmlSerializer.Serialize(fileStream, gp);
		fileStream.Close();
	}

	public static GhostPlayer ReadGhostPlayerData(string filename)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(GhostPlayer));
		GhostPlayer result = new GhostPlayer();
		try
		{
			FileStream fileStream = new FileStream(LevelManager.kDataPath + "/" + filename, FileMode.Open);
			result = xmlSerializer.Deserialize(fileStream) as GhostPlayer;
			fileStream.Close();
		}
		catch
		{
		}
		return result;
	}

	public static string ContraptionFileName(string levelName)
	{
		byte[] array = CryptoUtility.ComputeHash(Encoding.UTF8.GetBytes(levelName));
		return BitConverter.ToString(array).Substring(0, 30).Replace("-", string.Empty) + ".contraption";
	}

	public static void SaveContraptionDataset(string levelName, ContraptionDataset cds)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContraptionDataset));
		MemoryStream memoryStream = new MemoryStream();
		StreamWriter textWriter = new StreamWriter(memoryStream, Encoding.UTF8);
		xmlSerializer.Serialize(textWriter, cds);
		byte[] clearTextBytes = memoryStream.ToArray();
		memoryStream.Close();
		byte[] array = m_crypto.Encrypt(clearTextBytes);
		string text = ContraptionFileName(levelName);
		string text2 = LevelManager.kDataPath + "/contraptions";
		Directory.CreateDirectory(text2);
		FileStream fileStream = new FileStream(text2 + "/" + text, FileMode.Create);
		fileStream.Write(array, 0, array.Length);
		fileStream.Close();
	}

	public static ContraptionDataset LoadContraptionDataset(string levelName)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContraptionDataset));
		ContraptionDataset result = new ContraptionDataset();
		string text = ContraptionFileName(levelName);
		string text2 = LevelManager.kDataPath + "/contraptions";
		if (!File.Exists(text2 + "/" + text))
		{
			return new ContraptionDataset();
		}
		try
		{
			FileStream fileStream = new FileStream(text2 + "/" + text, FileMode.Open);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			byte[] buffer = m_crypto.Decrypt(array, 0);
			MemoryStream stream = new MemoryStream(buffer);
			result = xmlSerializer.Deserialize(stream) as ContraptionDataset;
			fileStream.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
		return result;
	}

	public static ContraptionDataset LoadContraptionDataset(TextAsset textAsset)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContraptionDataset));
		ContraptionDataset result = new ContraptionDataset();
		try
		{
			MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
			result = xmlSerializer.Deserialize(memoryStream) as ContraptionDataset;
			memoryStream.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
		return result;
	}
}
