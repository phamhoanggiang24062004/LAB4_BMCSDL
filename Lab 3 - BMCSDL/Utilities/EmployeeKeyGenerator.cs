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
        if (!Directory.Exists(_keyStoragePath))
        {
            Directory.CreateDirectory(_keyStoragePath);
        }
    }

    public void CreateKeyPairForEmployee(string manv, string mk)
    {
        if (string.IsNullOrWhiteSpace(manv) || string.IsNullOrWhiteSpace(mk))
        {
            MessageBox.Show("Mã nhân viên và mật khẩu không được để trống.", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            string publicKeyPath = Path.Combine(_keyStoragePath, $"{manv}_publickey.xml");
            string privateKeyPath = Path.Combine(_keyStoragePath, $"{manv}_privatekey.bin");

            if (File.Exists(publicKeyPath) && File.Exists(privateKeyPath))
            {
                Console.WriteLine($"Key pair for employee {manv} already exists.");
                return;
            }

            using (RSA rsa = RSA.Create(2048))
            {
                string publicKey = rsa.ToXmlString(false);
                File.WriteAllText(publicKeyPath, publicKey);
                Console.WriteLine($"Public key for {manv} saved to {publicKeyPath}");

                string privateKeyXml = rsa.ToXmlString(true);
                byte[] privateKeyBytes = Encoding.UTF8.GetBytes(privateKeyXml);
                byte[] encryptedPrivateKey = EncryptPrivateKey(privateKeyBytes, mk);
                File.WriteAllBytes(privateKeyPath, encryptedPrivateKey);
                Console.WriteLine($"Private key for {manv} saved to {privateKeyPath}. Ensure it is encrypted or stored securely.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi tạo cặp khóa cho {manv}: {ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
    }

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

    public RSA LoadPrivateKey(string manv, string mk)
    {
        string privateKeyPath = Path.Combine(_keyStoragePath, $"{manv}_privatekey.bin");
        if (!File.Exists(privateKeyPath))
            throw new FileNotFoundException($"Private key for {manv} not found.");

        try
        {
            RSA rsa = RSA.Create();
            byte[] encryptedPrivateKey = File.ReadAllBytes(privateKeyPath);
            byte[] privateKeyBytes = DecryptPrivateKey(encryptedPrivateKey, mk);
            string privateKeyXml = Encoding.UTF8.GetString(privateKeyBytes);
            rsa.FromXmlString(privateKeyXml);
            return rsa;
        }
        catch (Exception ex)
        {
            throw new CryptographicException($"Lỗi khi tải private key cho {manv}: {ex.Message}");
        }
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
                // Ghi IV vào đầu stream
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
                {
                    cs.Write(privateKey, 0, privateKey.Length);
                    cs.FlushFinalBlock(); // Đảm bảo hoàn thành việc ghi dữ liệu và padding
                }

                return ms.ToArray();
            }
        }
    }

    private byte[] DecryptPrivateKey(byte[] encryptedData, string password)
    {
        if (encryptedData == null || encryptedData.Length < 16)
            throw new ArgumentException("Dữ liệu mã hóa không hợp lệ hoặc quá ngắn.");

        using (Aes aes = Aes.Create())
        {
            byte[] key = DeriveKeyFromPassword(password);
            aes.Key = key;

            // Tách IV (16 byte đầu)
            byte[] iv = new byte[16];
            Array.Copy(encryptedData, 0, iv, 0, 16);
            aes.IV = iv;

            // Tách phần dữ liệu mã hóa
            byte[] cipherText = new byte[encryptedData.Length - 16];
            Array.Copy(encryptedData, 16, cipherText, 0, cipherText.Length);

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherText, 0, cipherText.Length);
                        cs.FlushFinalBlock(); // Đảm bảo giải mã hoàn tất
                    }
                    return ms.ToArray();
                }
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException("Lỗi giải mã: Mật khẩu không đúng hoặc dữ liệu bị hỏng.", ex);
            }
        }
    }

    private byte[] DeriveKeyFromPassword(string password)
    {
        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("somesalt"), 10000, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(32); // 32 bytes cho AES-256
        }
    }
}