using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace OneVault.Converters
{
    public class DetailsOPDataConverter : JsonConverter
    {
        public OPVault.KeyPair KeyPair { get; set; }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { throw new NotImplementedException(); }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Records.Details).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String || this.KeyPair == null)
                return null;

            byte[] opData = Convert.FromBase64String(reader.Value as string);
            if (opData != null && opData.Length > 0)
            {
                byte[] plainBytes = OPVault.OPData.ReadOPData(opData, this.KeyPair);
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
