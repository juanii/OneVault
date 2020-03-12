using Newtonsoft.Json;
using System;

namespace OneVault.Converters
{
    public class EncryptedKeyPairConverter : JsonConverter
    {
        private readonly OPVault.KeyPair keyPair;

        public EncryptedKeyPairConverter(OPVault.KeyPair keyPair)
        {
            this.keyPair = keyPair;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { throw new NotImplementedException(); }

        public override bool CanConvert(Type objectType)
        {
            return typeof(OPVault.KeyPair).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (this.keyPair == null)
                return null;

            byte[] itemKeyEncryptedBlob = Convert.FromBase64String(reader.Value as string);

            if (itemKeyEncryptedBlob == null || itemKeyEncryptedBlob.Length == 0)
                return null;

            return OPVault.OPData.DecryptItemKey(itemKeyEncryptedBlob, this.keyPair);
        }
    }
}
