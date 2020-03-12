using Newtonsoft.Json;
using System;

namespace OneVault.Converters
{
    public class HashedKeyPairConverter : JsonConverter
    {
        public OPVault.KeyPair KeyPair { get; set; }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { throw new NotImplementedException(); }

        public override bool CanConvert(Type objectType)
        {
            return typeof(OPVault.KeyPair).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (this.KeyPair == null)
                return null;

            byte[] keyBytes = Convert.FromBase64String(reader.Value as string);
            if (keyBytes != null && keyBytes.Length > 0)
            {
                byte[] masterKeyPlainText = OPVault.OPData.ReadOPData(keyBytes, this.KeyPair);
                if (masterKeyPlainText != null && masterKeyPlainText.Length > 0)
                    return OPVault.Crypto.HashKey(masterKeyPlainText, this.KeyPair);
            }

            return null;
        }
    }
}
