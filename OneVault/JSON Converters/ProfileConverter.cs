using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace OneVault.Converters
{
    public class ProfileConverter : JsonConverter
    {
        private readonly string masterPassword;

        public ProfileConverter(string masterPassword)
        {
            this.masterPassword = masterPassword;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { throw new NotImplementedException(); }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Records.Profile).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty(masterPassword))
                return null;

            JObject profileObject = JObject.Load(reader);

            byte[] masterPasswordBytes = Encoding.UTF8.GetBytes(this.masterPassword);
            byte[] salt = Convert.FromBase64String((string)profileObject["salt"]);
            int iterations = (int)profileObject["iterations"];

            if (salt == null || salt.Length == 0)
                return null;

            OPVault.KeyPair derivedKeys = OPVault.Crypto.DeriveKey(masterPasswordBytes, salt, iterations);

            foreach (JsonConverter converter in serializer.Converters)
            {
                if (converter is HashedKeyPairConverter)
                    (converter as HashedKeyPairConverter).KeyPair = derivedKeys;
            }

            Records.Profile profile = new Records.Profile();
            serializer.Populate(profileObject.CreateReader(), profile);

            return profile;
        }
    }
}
