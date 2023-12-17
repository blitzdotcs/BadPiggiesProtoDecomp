using System.IO;
using System.Security.Cryptography;

public class CryptoUtility
{
	private byte[] m_keyBytes;

	private byte[] m_ivBytes;

	private static SHA1CryptoServiceProvider m_sha1Service = new SHA1CryptoServiceProvider();

	private AesManaged m_aes = new AesManaged();

	public CryptoUtility(string key)
	{
		byte[] salt = new byte[13]
		{
			82, 166, 66, 87, 146, 51, 179, 108, 242, 110,
			98, 237, 124
		};
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, salt);
		m_keyBytes = rfc2898DeriveBytes.GetBytes(32);
		m_ivBytes = rfc2898DeriveBytes.GetBytes(16);
	}

	public byte[] Encrypt(byte[] clearTextBytes)
	{
		ICryptoTransform transform = m_aes.CreateEncryptor(m_keyBytes, m_ivBytes);
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		cryptoStream.Write(clearTextBytes, 0, clearTextBytes.Length);
		cryptoStream.FlushFinalBlock();
		memoryStream.Position = 0L;
		byte[] array = new byte[memoryStream.Length];
		memoryStream.Read(array, 0, array.Length);
		cryptoStream.Close();
		memoryStream.Close();
		return array;
	}

	public byte[] Decrypt(byte[] encryptedBytes, int offset)
	{
		ICryptoTransform transform = m_aes.CreateDecryptor(m_keyBytes, m_ivBytes);
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		cryptoStream.Write(encryptedBytes, offset, encryptedBytes.Length - offset);
		cryptoStream.FlushFinalBlock();
		memoryStream.Position = 0L;
		byte[] array = new byte[memoryStream.Length];
		memoryStream.Read(array, 0, array.Length);
		cryptoStream.Close();
		memoryStream.Close();
		return array;
	}

	public static byte[] ComputeHash(byte[] data)
	{
		return m_sha1Service.ComputeHash(data);
	}

	public byte[] ComputeHash(byte[] data, int offset, int count)
	{
		return m_sha1Service.ComputeHash(data, offset, count);
	}
}
