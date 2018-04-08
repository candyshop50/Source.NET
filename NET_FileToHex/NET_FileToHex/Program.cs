using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NET_FileToHex
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
                FileToHex(args[i]);

            //Console.ReadKey();
        }

        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted)
        {
            byte[] encryptedBytes = null;
            byte[] passwordBytes = new byte[240];
            new Random().NextBytes(passwordBytes);

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = new byte[passwordBytes.Length + ms.ToArray().Length];
                    passwordBytes.CopyTo(encryptedBytes, 0);
                    ms.ToArray().CopyTo(encryptedBytes, passwordBytes.Length);
                }
            }

            return encryptedBytes;
        }


        private static void FileToHex(string fileName)
        {
            byte[] encryptedAutorun = AES_Encrypt(File.ReadAllBytes(fileName));
            ByteArrayToString(encryptedAutorun);
        }

        public static void ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            string[] bytes = hex.Split(new char[] { '-' });

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] text = Encoding.UTF8.GetBytes("public static byte[] encryptedAutroun = {\r\n");
                ms.Write(text, 0, text.Length);

                for (int i = 0; i < bytes.Length; i++)
                {
                    string format = (i == 0) || (i % 20 != 0) ? "0x{0}{1}" : "0x{0}{1}\r\n";
                    text = Encoding.UTF8.GetBytes(string.Format(format, bytes[i], (i != bytes.Length - 1) ? "," : ""));
                    ms.Write(text, 0, text.Length);
                }

                text = Encoding.UTF8.GetBytes("};");
                ms.Write(text, 0, text.Length);

                File.WriteAllBytes("hui.cs", ms.ToArray());
            }
        }

        public static string ByteArrayToString2(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
