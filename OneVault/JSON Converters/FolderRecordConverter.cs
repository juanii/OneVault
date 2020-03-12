using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace OneVault.Converters
{
    public class FolderRecordConverter : JsonConverter
    {
        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { throw new NotImplementedException(); }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Records.FolderRecord).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonRecord = JObject.Load(reader);
            JToken smart;
            bool isSmart = jsonRecord.TryGetValue("smart", out smart) && (bool)smart;
            Records.FolderRecord folder;

            if (isSmart)
                folder = new Records.SavedSearchRecord();
            else
                folder = new Records.RegularFolderRecord();

            serializer.Populate(jsonRecord.CreateReader(), folder);

            return folder;
        }
    }
}
