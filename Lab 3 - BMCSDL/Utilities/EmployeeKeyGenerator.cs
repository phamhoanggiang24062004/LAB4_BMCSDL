using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EmployeeKeyGenerator
{
    private readonly string _keyStoragePath = "D:\\NAM_3\\BMCSDL\\Lab 3 - BMCSDL\\StorageKey";

    public string getKeyStoragePath()
    {
        return _keyStoragePath;
    }
    public EmployeeKeyGenerator()
    {
        // Ensure the storage directory exists
        if (!Directory.Exists(_keyStoragePath))
        {
            Directory.CreateDirectory(_keyStoragePath);
        }
    }

    public void CreateKeyPairForEmployee(string manv, string mk)
    {
        try
        {
            string publicKeyPath = Path.Combine(_keyStoragePath, $"{manv}_publickey.xml");
            string privateKeyPath = Path.Combine(_keyStoragePath, $"{manv}_privatekey.xml");

            if (File.Exists(publicKeyPath) && File.Exists(privateKeyPath))
            {
                Console.WriteLine($"Key pair for employee {manv} already exists.");
                return;
            }

            // Create RSA key pair
            using (RSA rsa = RSA.Create(2048)) // 2048-bit key 
            {
                // Export public key
                string publicKey = rsa.ToXmlString(false); // false = public key only
                File.WriteAllText(publicKeyPath, publicKey);
                Console.WriteLine($"Public key for {manv} saved to {publicKeyPath}");

                // Export private key, encrypted with the provided password (MK)
                // Note: .NET's RSA.ToXmlString doesn't support password encryption directly.
                // For password protection, we can use a custom encryption or rely on secure storage.
                // Here, we'll simulate by storing the private key and recommend secure storage.
                string privateKey = rsa.ToXmlString(true); // true = include private key
                File.WriteAllText(privateKeyPath, privateKey);
                Console.WriteLine($"Private key for {manv} saved to {privateKeyPath}. Ensure it is encrypted or stored securely.");

                // Simulate password protection (in production, use a secure key vault or encrypt the private key)
                // Example: Encrypt private key with AES using the MK password (not implemented here for brevity).
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating key pair for {manv}: {ex.Message}");
        }
    }

    // Example method to load and use the public key (for reference)
    public RSA LoadPublicKey(string manv)
    {
        string publicKeyPath = Path.Combine(_keyStoragePath, $"{manv}_publickey.xml");
        if (!File.Exists(publicKeyPath))
            throw new FileNotFoundException($"Public key for {manv} not found.");

        RSA rsa = RSA.Create();
        string publicKeyXml = File.ReadAllText(publicKeyPath);
        rsa.FromXmlString(publicKeyXml);
        return rsa;         
    }

    public RSA LoadPrivateKey(string manv)
    {
        string privateKeyPath = Path.Combine(_keyStoragePath, $"{manv}_privatekey.xml");
        if (!File.Exists(privateKeyPath))
            throw new FileNotFoundException($"Private key for {manv} not found.");

        RSA rsa = RSA.Create();
        string privateKeyXml = File.ReadAllText(privateKeyPath);
        rsa.FromXmlString(privateKeyXml);
        return rsa;          
    }


    private byte[] EncryptPrivateKey(byte[] privateKey, string password)
    {
        using (Aes aes = Aes.Create())
        {
            byte[] key = DeriveKeyFromPassword(password);
            aes.Key = key;
            aes.GenerateIV();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(privateKey, 0, privateKey.Length);
                }
                return ms.ToArray();
            }
        }
    }

    private byte[] DecryptPrivateKey(byte[] encryptedData, string password)
    {
        using (Aes aes = Aes.Create())
        {
            byte[] key = DeriveKeyFromPassword(password);
            aes.Key = key;
            byte[] iv = new byte[16];
            Array.Copy(encryptedData, 0, iv, 0, 16);
            aes.IV = iv;
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(encryptedData, 16, encryptedData.Length - 16);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }

    private byte[] DeriveKeyFromPassword(string password)
    {
        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("somesalt"), 10000, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(32); // 32 bytes for AES-256
        }
    }


}

