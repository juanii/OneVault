using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace OneVault.Converters
{
    public class OverviewOPDataConverter : JsonConverter
    {
        private readonly OPVault.KeyPair keyPair;

        public OverviewOPDataConverter(OPVault.KeyPair keyPair)
        {
            this.keyPair = keyPair;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { throw new NotImplementedException(); }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Records.Overview).IsAssignableFrom(objectType) || typeof(Records.AttachmentOverview).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String || this.keyPair == null)
                return null;

            byte[] opData = Convert.FromBase64String(reader.Value as string);
            if (opData != null && opData.Length > 0)
            {
                byte[] plainBytes = OPVault.OPData.ReadOPData(opData, keyPair);
                if (plainBytes != null && plainBytes.Length > 0)
                {
                    string plainText = Encoding.UTF8.GetString(plainBytes);

                    object overview = Activator.CreateInstance(objectType);
                    using (TextReader tr = new StringReader(plainText))
                        serializer.Populate(tr, overview);

                    return overview;
                }
            }

            return null;
        }
    }
}
