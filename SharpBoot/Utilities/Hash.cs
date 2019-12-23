﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SharpBoot.Utilities
{
    public class Hash
    {
        public static string FileHash<T>(string fileName)
            where T : HashAlgorithm, new()
        {
            StringBuilder formatted;
            using (var fs = File.OpenRead(fileName))
            using (var bs = new BufferedStream(fs))
            {
                using (var ha = new T())
                {
                    var hash = ha.ComputeHash(bs);
                    formatted = new StringBuilder(2 * hash.Length);
                    foreach (var b in hash) formatted.AppendFormat("{0:x2}", b);
                }
            }

            return formatted.ToString();
        }

        public static string FileMD5(string fileName)
        {
            return FileHash<MD5CryptoServiceProvider>(fileName);
        }

        public static string FileSHA1(string fileName)
        {
            return FileHash<SHA1CryptoServiceProvider>(fileName);
        }

        public static string FileSHA256(string fileName)
        {
            return FileHash<SHA256CryptoServiceProvider>(fileName);
        }

        public static string FileSHA384(string fileName)
        {
            return FileHash<SHA384CryptoServiceProvider>(fileName);
        }

        public static string FileSHA512(string fileName)
        {
            return FileHash<SHA512CryptoServiceProvider>(fileName);
        }

        public static string CRC32(string ct)
        {
            return CRC32(Program.GetEnc().GetBytes(ct));
        }

        public static string CRC32(byte[] ct)
        {
            var table = new uint[256];
            for (var i = 0; i < 256; i++)
            {
                var cur = (uint) i;
                for (var j = 0; j < 8; j++)
                    if ((cur & 1) == 1)
                        cur = (cur >> 1) ^ 0xedb88320;
                    else
                        cur = cur >> 1;
                table[i] = cur;
            }

            return (~ct.Aggregate(0xffffffff, (current, t) => (current >> 8) ^ table[t])).ToString("x8");
        }

        public static string FileHash(string fileName, string hash)
        {
            switch (hash)
            {
                case "md5":
                    return FileMD5(fileName);
                case "sha1":
                    return FileSHA1(fileName);
                case "sha256":
                    return FileSHA256(fileName);
            }

            return "";
        }
    }
}
