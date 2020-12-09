using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Xsolla.Core
{
	public static class Crypto
	{
		public static string Encrypt(byte[] key, string dataToEncrypt)
		{
			try
			{
				// Initialize
				AesManaged encryptor = new AesManaged();
				// Set the key
				encryptor.Key = key;
				encryptor.IV = key;
				// create a memory stream
				using (MemoryStream encryptionStream = new MemoryStream())
				{
					// Create the crypto stream
					using (CryptoStream encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(),
						CryptoStreamMode.Write))
					{
						// Encrypt
						byte[] utfD1 = UTF8Encoding.UTF8.GetBytes(dataToEncrypt);
						encrypt.Write(utfD1, 0, utfD1.Length);
						encrypt.FlushFinalBlock();
						encrypt.Close();
						// Return the encrypted data
						return Convert.ToBase64String(encryptionStream.ToArray());
					}
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public static string Decrypt(byte[] key, string encryptedString)
		{
			try
			{
				// Initialize
				AesManaged decryptor = new AesManaged();
				byte[] encryptedData = Convert.FromBase64String(encryptedString);
				// Set the key
				decryptor.Key = key;
				decryptor.IV = key;
				// create a memory stream
				using (MemoryStream decryptionStream = new MemoryStream())
				{
					// Create the crypto stream
					using (CryptoStream decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(),
						CryptoStreamMode.Write))
					{
						// Encrypt
						decrypt.Write(encryptedData, 0, encryptedData.Length);
						decrypt.Flush();
						decrypt.Close();
						// Return the unencrypted data
						byte[] decryptedData = decryptionStream.ToArray();
						return UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
					}
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}
}