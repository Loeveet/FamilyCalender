using System.Security.Cryptography;
using System.Text;

namespace FamilyCalender.Infrastructure.Services
{
    public class EncryptionService
    {
        public const string Magic = "MagicIsWhatItIs"; // DANGER: never change this

        private readonly string _autoDetectSequence = new string(new[] { (char)0x025B, (char)0x0377, (char)0x0297, (char)0x25CF });
        private readonly string _vectorSalt = "hfk8HJ76fhgfdt78jhdYUTS787AHxc";
        private readonly byte[] _salt = Encoding.UTF8.GetBytes("K86bFd--88LxdtyuHSDY_dhf!fold3");

        public EncryptionService(string magic)
        {
            Key = GenerateKey(magic);
        }

        protected byte[] GenerateKey(string passwordPart)
        {
            var iterations = 4;
            var keySize = 256;
            var hash = "SHA256";
            var passwordBytes = new PasswordDeriveBytes(passwordPart + "?ML4tinfur-RLqra#4Bjbg76bnJ?qH", _salt, hash, iterations);
            var keyBytes = passwordBytes.GetBytes(keySize / 8);
            return keyBytes;
        }

        private byte[] Key { get; }

        public byte[] Encrypt(byte[] dataToEncrypt, string vector)
        {
            var vectorBytes = GetHash(string.Concat(vector, _vectorSalt));

            using (var encryptionHelper = new EncryptionHelper(Key, vectorBytes))
            {
                var encrypt = encryptionHelper.Encrypt(dataToEncrypt);
                return encrypt;
            }
        }

        public byte[] EncryptString(string valueToEncrypt, string vector)
        {
            return Encrypt(Encoding.UTF8.GetBytes(valueToEncrypt), vector);
        }

        public string EncryptStringToString(string valueToEncrypt, string vector)
        {
            return LocalHttpUtitlity.UrlTokenEncode(EncryptString(valueToEncrypt, vector));
        }

        public string AutoDetectEncryptStringToString(string valueToEncrypt, string vector)
        {
            if (string.IsNullOrEmpty(valueToEncrypt))
            {
                return valueToEncrypt;
            }

            if (valueToEncrypt.StartsWith(_autoDetectSequence))
            {
                return valueToEncrypt;
            }

            return string.Concat(_autoDetectSequence, LocalHttpUtitlity.UrlTokenEncode(EncryptString(valueToEncrypt, vector)));
        }

        public byte[] Decrypt(byte[] encryptedData, string vector)
        {
            var vectorBytes = GetHash(string.Concat(vector, _vectorSalt));

            using (var encryptionHelper = new EncryptionHelper(Key, vectorBytes))
            {
                var decrypt = encryptionHelper.Decrypt(encryptedData);
                return decrypt;
            }
        }

        public string DecryptString(byte[] encryptedValue, string vector)
        {
            return Encoding.UTF8.GetString(Decrypt(encryptedValue, vector));
        }

        public string DecryptStringToString(string encryptedValue, string vector)
        {
            return DecryptString(LocalHttpUtitlity.UrlTokenDecode(encryptedValue), vector);
        }

        public string AutoDetectDecryptStringToString(string encryptedValue, string vector)
        {
            if (string.IsNullOrWhiteSpace(encryptedValue))
            {
                return encryptedValue;
            }

            if (encryptedValue.StartsWith(_autoDetectSequence))
            {
                return DecryptString(LocalHttpUtitlity.UrlTokenDecode(encryptedValue.Substring(4)), vector);
            }

            return encryptedValue;
        }

        private byte[] GetHash(string value)
        {
            using (MD5 hash = MD5.Create())
            {
                return hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }

        public class EncryptionHelper : IDisposable
        {
            AesManaged aesManaged;
            UTF8Encoding encoding;

            public EncryptionHelper(byte[] key, byte[] vector)
            {
                encoding = new UTF8Encoding();
                aesManaged = new AesManaged();
                aesManaged.Padding = PaddingMode.PKCS7;
                aesManaged.Key = key;
                aesManaged.IV = vector;
            }

            public byte[] Encrypt(byte[] dataToEncrypt)
            {
                using (var encryptor = aesManaged.CreateEncryptor())
                using (var stream = new MemoryStream())
                using (var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                {
                    crypto.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                    crypto.FlushFinalBlock();
                    stream.Position = 0;
                    var encrypted = new byte[stream.Length];
                    stream.Read(encrypted, 0, encrypted.Length);
                    return encrypted;
                }
            }

            public byte[] Decrypt(byte[] encryptedData)
            {
                try
                {
                    using (var decryptor = aesManaged.CreateDecryptor())
                    using (var stream = new MemoryStream())
                    using (var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
                    {
                        crypto.Write(encryptedData, 0, encryptedData.Length);
                        crypto.FlushFinalBlock();
                        stream.Position = 0;
                        var decryptedBytes = new System.Byte[stream.Length];
                        stream.Read(decryptedBytes, 0, decryptedBytes.Length);

                        return decryptedBytes;
                    }
                }
                catch (Exception)
                {
                    return encryptedData;
                }
            }

            public void Dispose()
            {
                if (aesManaged != null)
                {
                    aesManaged.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// This class is a self-coded version of .net standard HttpServerUtility
    /// https://stackoverflow.com/questions/50731397/httpserverutility-urltokenencode-replacement-for-netstandard
    /// </summary>
    public class LocalHttpUtitlity
    {
        public static string UrlTokenEncode(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (input.Length < 1)
                return String.Empty;
            char[] base64Chars = null;

            // Step 1: Do a Base64 encoding
            string base64Str = Convert.ToBase64String(input);
            if (base64Str == null)
                return null;

            int endPos;
            // Step 2: Find how many padding chars are present in the end
            for (endPos = base64Str.Length; endPos > 0; endPos--)
            {
                if (base64Str[endPos - 1] != '=') // Found a non-padding char!
                {
                    break; // Stop here
                }
            }

            // Step 3: Create char array to store all non-padding chars,
            //      plus a char to indicate how many padding chars are needed
            base64Chars = new char[endPos + 1];
            base64Chars[endPos] = (char)((int)'0' + base64Str.Length - endPos); // Store a char at the end, to indicate how many padding chars are needed

            // Step 3: Copy in the other chars. Transform the "+" to "-", and "/" to "_"
            for (int iter = 0; iter < endPos; iter++)
            {
                char c = base64Str[iter];

                switch (c)
                {
                    case '+':
                        base64Chars[iter] = '-';
                        break;

                    case '/':
                        base64Chars[iter] = '_';
                        break;

                    case '=':
                        base64Chars[iter] = c;
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }
            return new string(base64Chars);
        }

        public static byte[] UrlTokenDecode(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            int len = input.Length;
            if (len < 1)
                return new byte[0];

            // Step 1: Calculate the number of padding chars to append to this string.
            //         The number of padding chars to append is stored in the last char of the string.
            int numPadChars = (int)input[len - 1] - (int)'0';
            if (numPadChars < 0 || numPadChars > 10)
                return null;


            // Step 2: Create array to store the chars (not including the last char)
            //          and the padding chars
            char[] base64Chars = new char[len - 1 + numPadChars];


            // Step 3: Copy in the chars. Transform the "-" to "+", and "*" to "/"
            for (int iter = 0; iter < len - 1; iter++)
            {
                char c = input[iter];

                switch (c)
                {
                    case '-':
                        base64Chars[iter] = '+';
                        break;

                    case '_':
                        base64Chars[iter] = '/';
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }

            // Step 4: Add padding chars
            for (int iter = len - 1; iter < base64Chars.Length; iter++)
            {
                base64Chars[iter] = '=';
            }

            // Do the actual conversion
            return Convert.FromBase64CharArray(base64Chars, 0, base64Chars.Length);
        }
    }
}
