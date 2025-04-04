using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Services.Security
{
    /// <summary>
    /// واجهة خدمة التشفير وفك التشفير
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// تشفير بيانات نصية
        /// </summary>
        string Encrypt(string plainText);
        
        /// <summary>
        /// فك تشفير البيانات
        /// </summary>
        string Decrypt(string cipherText);
        
        /// <summary>
        /// تشفير كائن بتحويله إلى JSON أولاً
        /// </summary>
        string EncryptObject<T>(T obj);
        
        /// <summary>
        /// فك تشفير كائن من JSON مشفر
        /// </summary>
        T? DecryptObject<T>(string encryptedJson);
    }
    
    /// <summary>
    /// تنفيذ خدمة التشفير وفك التشفير باستخدام AES
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        
        /// <summary>
        /// إنشاء خدمة تشفير جديدة
        /// </summary>
        public EncryptionService(IConfiguration configuration)
        {
            // الحصول على مفاتيح التشفير من التكوين
            var encryptionSettings = configuration.GetSection("EncryptionSettings");
            var keyString = encryptionSettings["Key"] ?? throw new InvalidOperationException("Encryption key is not configured");
            var ivString = encryptionSettings["IV"] ?? throw new InvalidOperationException("Encryption IV is not configured");
            
            // تحويل المفاتيح إلى بايتات
            _key = Convert.FromBase64String(keyString);
            _iv = Convert.FromBase64String(ivString);
        }
        
        /// <summary>
        /// تشفير بيانات نصية
        /// </summary>
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;
                
            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            
            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new(cryptoStream))
            {
                streamWriter.Write(plainText);
            }
            
            byte[] encryptedData = memoryStream.ToArray();
            return Convert.ToBase64String(encryptedData);
        }
        
        /// <summary>
        /// فك تشفير البيانات
        /// </summary>
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;
                
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            
            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            
            using MemoryStream memoryStream = new(cipherBytes);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);
            
            return streamReader.ReadToEnd();
        }
        
        /// <summary>
        /// تشفير كائن بتحويله إلى JSON أولاً
        /// </summary>
        public string EncryptObject<T>(T obj)
        {
            if (obj == null)
                return string.Empty;
                
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            return Encrypt(json);
        }
        
        /// <summary>
        /// فك تشفير كائن من JSON مشفر
        /// </summary>
        public T? DecryptObject<T>(string encryptedJson)
        {
            if (string.IsNullOrEmpty(encryptedJson))
                return default;
                
            var json = Decrypt(encryptedJson);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
} 