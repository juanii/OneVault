using System;
using System.IO;
using System.Text;

namespace OneVault.OPVault
{
    public class OPData
    {
        private class AttachmentHeaderData
        {
            public ushort Version { get; set; }

            public ushort MetadataLength { get; set; }

            public uint IconLength { get; set; }
        }

        private static byte[] OPDATA_MAGIC_NUMBER = Encoding.ASCII.GetBytes("opdata01");

        private static int PAYLOAD_LENGTH_SIZE = 8;

        private static byte[] OPCLDAT_MAGIC_NUMBER = Encoding.ASCII.GetBytes("OPCLDAT");

        public static byte[] ReadOPData(byte[] opdata, KeyPair keyPair)
        {
            if (!MAC.CheckMAC(new ArraySegment<byte>(opdata, 0, opdata.Length - MAC.MAC_SIZE), new ArraySegment<byte>(opdata, opdata.Length - MAC.MAC_SIZE, MAC.MAC_SIZE), keyPair.macKey))
                return null;

            ArraySegment<byte> magic = new ArraySegment<byte>(opdata, 0, OPDATA_MAGIC_NUMBER.Length);

            for (int i = 0; i < OPDATA_MAGIC_NUMBER.Length; i++)
            {
                if (magic.Array[magic.Offset + i] != OPDATA_MAGIC_NUMBER[i])
                    return null;
            }

            ArraySegment<byte> length = new ArraySegment<byte>(opdata, OPDATA_MAGIC_NUMBER.Length, PAYLOAD_LENGTH_SIZE);

            ulong payloadLength = 0;

            for (int i = 0; i < PAYLOAD_LENGTH_SIZE; i++)
                payloadLength |= (ulong)length.Array[length.Offset + i] << (i * 8);

            byte[] decryptedBlob = Crypto.Decrypt(new ArraySegment<byte>(opdata, OPDATA_MAGIC_NUMBER.Length + PAYLOAD_LENGTH_SIZE + Crypto.IV_SIZE, opdata.Length - OPDATA_MAGIC_NUMBER.Length - PAYLOAD_LENGTH_SIZE - Crypto.IV_SIZE - MAC.MAC_SIZE), new ArraySegment<byte>(opdata, OPDATA_MAGIC_NUMBER.Length + PAYLOAD_LENGTH_SIZE, Crypto.IV_SIZE), keyPair.encryptionKey);

            byte[] plainText = new byte[payloadLength];

            Array.Copy(decryptedBlob, decryptedBlob.Length - (int)payloadLength, plainText, 0, plainText.Length);

            return plainText;
        }

        public static KeyPair DecryptItemKey(byte[] data, KeyPair keyPair)
        {
            if (!MAC.CheckMAC(new ArraySegment<byte>(data, 0, data.Length - MAC.MAC_SIZE), new ArraySegment<byte>(data, data.Length - MAC.MAC_SIZE, MAC.MAC_SIZE), keyPair.macKey))
                return null;

            byte[] itemKeyDecryptedBlob = Crypto.Decrypt(new ArraySegment<byte>(data, Crypto.IV_SIZE, data.Length - Crypto.IV_SIZE - MAC.MAC_SIZE), new ArraySegment<byte>(data, 0, Crypto.IV_SIZE), keyPair.encryptionKey);
            if (itemKeyDecryptedBlob == null || itemKeyDecryptedBlob.Length == 0)
                return null;

            byte[] itemEncryptionKey = new byte[itemKeyDecryptedBlob.Length / 2];
            byte[] itemMACKey = new byte[itemKeyDecryptedBlob.Length / 2];

            Array.Copy(itemKeyDecryptedBlob, 0, itemEncryptionKey, 0, itemEncryptionKey.Length);
            Array.Copy(itemKeyDecryptedBlob, itemEncryptionKey.Length, itemMACKey, 0, itemMACKey.Length);

            return new KeyPair() { encryptionKey = itemEncryptionKey, macKey = itemMACKey };
        }

        private static AttachmentHeaderData readAttachmentHeader(FileStream fs)
        {
            for (int i = 0; i < OPCLDAT_MAGIC_NUMBER.Length; i++)
            {
                if ((byte)fs.ReadByte() != OPCLDAT_MAGIC_NUMBER[i])
                    return null;
            }

            byte version = (byte)fs.ReadByte();

            if (version > 0x02)
                return null;

            ushort metadataLength = 0;
            for (int i = 0; i < 2; i++)
                metadataLength |= (ushort)(fs.ReadByte() << (i * 8));

            fs.Seek(2, SeekOrigin.Current);

            uint iconLength = 0;
            for (int i = 0; i < 4; i++)
                iconLength |= (uint)(fs.ReadByte() << (i * 8));

            return new AttachmentHeaderData()
            {
                Version = version,
                MetadataLength = metadataLength,
                IconLength = iconLength
            };
        }

        public static string ReadAttachmentMetadata(string attachmentFile)
        {
            using (FileStream fs = File.OpenRead(attachmentFile))
            {
                AttachmentHeaderData attachmentHeaderData = readAttachmentHeader(fs);

                if (attachmentHeaderData == null)
                    return null;

                byte[] metadataBytes = new byte[attachmentHeaderData.MetadataLength];
                fs.Read(metadataBytes, 0, metadataBytes.Length);

                return Encoding.UTF8.GetString(metadataBytes);
            }
        }

        public static byte[] ReadAttachmentData(string attachmentFile, KeyPair itemKey)
        {
            using (FileStream fs = File.OpenRead(attachmentFile))
            {
                AttachmentHeaderData attachmentHeaderData = readAttachmentHeader(fs);

                if (attachmentHeaderData == null)
                    return null;

                fs.Seek(attachmentHeaderData.MetadataLength + attachmentHeaderData.IconLength, SeekOrigin.Current);

                byte[] dataOPData = new byte[fs.Length - fs.Position];
                fs.Read(dataOPData, 0, dataOPData.Length);
                return ReadOPData(dataOPData, itemKey);
            }
        }
    }
}
