﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Itemdex.Core
{
    public static class Reader
    {
        private static readonly byte[] EncryptionKey = new UnicodeEncoding().GetBytes("h3y_gUyZ");
        private const int PlayerFileType = 3;

        public static void ExtractJourneyModeProgressFromFile(MemoryStream ms, Itemdex itemdex)
        {
            var player = new Player();

            try
            {
                var rijndaelManaged = new RijndaelManaged {Padding = PaddingMode.None};
                
                ms.Position = 0;
                using var cryptoStream = new CryptoStream(ms,
                    rijndaelManaged.CreateDecryptor(EncryptionKey, EncryptionKey),
                    CryptoStreamMode.Read);
                using var binaryReader = new BinaryReader(cryptoStream);

                itemdex.SaveVersion = binaryReader.ReadInt32();

                ReadHeader(binaryReader);

                itemdex.CharacterName = binaryReader.ReadString();
                if (itemdex.SaveVersion < 218)
                {
                    return;
                }

                ReadUselessValues(binaryReader, player);
                
                itemdex.LoadProgress(binaryReader);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fucked: {ex.Message}");
            }
        }

        private static void ReadUselessValues(BinaryReader binaryReader, Player player1)
        {
            binaryReader.ReadByte();
            binaryReader.ReadInt64();
            binaryReader.ReadInt32();
            binaryReader.ReadByte();
            binaryReader.ReadByte();
            binaryReader.ReadByte();
            binaryReader.ReadByte();
            binaryReader.ReadByte();
            binaryReader.ReadInt32();
            binaryReader.ReadInt32();
            binaryReader.ReadInt32();
            binaryReader.ReadInt32();
            binaryReader.ReadBoolean();
            binaryReader.ReadBoolean();
            binaryReader.ReadInt32();
            ReadRgb(binaryReader);
            ReadRgb(binaryReader);
            ReadRgb(binaryReader);
            ReadRgb(binaryReader);
            ReadRgb(binaryReader);
            ReadRgb(binaryReader);
            ReadRgb(binaryReader);

            var equipSlots = 20;
            for (var index = 0; index < equipSlots; ++index)
            {
                binaryReader.ReadInt32();
                binaryReader.ReadByte();
            }

            var dyeSlots = 10;
            for (var index1 = 0; index1 < dyeSlots; ++index1)
            {
                binaryReader.ReadInt32();
                binaryReader.ReadByte();
            }

            for (var index = 0; index < 58; ++index)
            {
                binaryReader.ReadInt32();
                binaryReader.ReadInt32();
                binaryReader.ReadByte();
                binaryReader.ReadBoolean();
            }

            for (var index = 0; index < 5; ++index)
            {
                binaryReader.ReadInt32();
                binaryReader.ReadByte();
                binaryReader.ReadInt32();
                binaryReader.ReadByte();
            }

            for (var index = 0; index < 40; ++index)
            {
                binaryReader.ReadInt32();
                binaryReader.ReadInt32();
                binaryReader.ReadByte();
            }

            for (var index = 0; index < 40; ++index)
            {
                binaryReader.ReadInt32();
                binaryReader.ReadInt32();
                binaryReader.ReadByte();
            }

            for (var index = 0; index < 40; ++index)
            {
                binaryReader.ReadInt32();
                binaryReader.ReadInt32();
                binaryReader.ReadByte();
            }

            for (var index = 0; index < 40; ++index)
            {
                binaryReader.ReadInt32();
                binaryReader.ReadInt32();
                binaryReader.ReadByte();
            }

            binaryReader.ReadByte();

            var buffSlots = 22;
            for (var index = 0; index < buffSlots; ++index)
            {
                player1.buffType[index] = binaryReader.ReadInt32();
                player1.buffTime[index] = binaryReader.ReadInt32();
                if (player1.buffType[index] == 0)
                {
                    --index;
                    --buffSlots;
                }
            }

            for (var index = 0; index < 200; ++index)
            {
                var num2 = binaryReader.ReadInt32();
                if (num2 != -1)
                {
                    binaryReader.ReadInt32();
                    binaryReader.ReadInt32();
                    binaryReader.ReadString();
                }
                else
                    break;
            }

            binaryReader.ReadBoolean();
            {
                var num2 = 13;
                for (var index = 0; index < num2; ++index)
                    binaryReader.ReadBoolean();
            }

            binaryReader.ReadInt32();
            for (var index = 0; index < 4; ++index)
                binaryReader.ReadInt32();

            {
                var num2 = 11;
                for (var index = 0; index < num2; ++index)
                    binaryReader.ReadInt32();
            }

            binaryReader.ReadInt32();
            player1.dead = binaryReader.ReadBoolean();
            if (player1.dead)
                binaryReader.ReadInt32();

            binaryReader.ReadInt64();
            binaryReader.ReadInt32();
        }

        private static void ReadHeader(BinaryReader reader)
        {
            var num1 = (long) reader.ReadUInt64();
            if ((num1 & 72057594037927935L) != 27981915666277746L)
                throw new Exception("Expected Re-Logic file format.");

            var fileType = (byte) ((ulong) num1 >> 56 & byte.MaxValue);

            if (fileType != PlayerFileType)
                throw new Exception("Found invalid file type.");

            reader.ReadUInt32();
            reader.ReadUInt64();
        }

        private static void ReadRgb(BinaryReader bb)
        {
            bb.ReadByte();
            bb.ReadByte();
            bb.ReadByte();
        }
    }
}