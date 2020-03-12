using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneVault.OPVault;
using OneVault.Records;
using System;
using System.Collections.Generic;

namespace OneVault.Converters
{
    public class ItemRecordConverter : JsonConverter
    {
        private static Dictionary<string, Type> classTypesByItemType = new Dictionary<string, Type>()
        {
            //{ "system.folder.SavedSearch", typeof(SavedSearchRecord) },
            //{ "system.folder.Regular", typeof(RegularFolderRecord) },
            { "001", typeof(WebFormRecord) },
            { "002", typeof(CreditCardRecord) },
            { "003", typeof(SecureNoteRecord) },
            { "004", typeof(IdentityRecord) },
            { "005", typeof(PasswordRecord) },
            { "099", typeof(UnknownRecord) }, //Tombstone
            { "100", typeof(LicenseRecord) },
            { "101", typeof(BankAccountUsRecord) },
            { "102", typeof(DatabaseRecord) },
            { "103", typeof(DriversLicenseRecord) },
            { "104", typeof(HuntingLicenseRecord) },
            { "105", typeof(MembershipRecord) },
            { "106", typeof(PassportRecord) },
            { "107", typeof(RewardProgramRecord) },
            { "108", typeof(SsnUsRecord) },
            { "109", typeof(RouterRecord) },
            { "110", typeof(UnixServerRecord) },
            { "111", typeof(EmailV2Record) },
        };

        private readonly KeyPair keyPair;

        public ItemRecordConverter(KeyPair keyPair)
        {
            this.keyPair = keyPair;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { throw new NotImplementedException(); }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Records.ItemRecord).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonRecord = JObject.Load(reader);
            string recordType = (string)jsonRecord.GetValue("category");

            ItemRecord record = null;

            if (!string.IsNullOrEmpty(recordType) && classTypesByItemType.ContainsKey(recordType))
                record = Activator.CreateInstance(classTypesByItemType[recordType]) as ItemRecord;
            else
                record = new UnknownRecord();

            KeyPair itemKeyPair = jsonRecord.GetValue("k").ToObject<KeyPair>(serializer);

            if (itemKeyPair == null)
                return null;

            foreach (JsonConverter converter in serializer.Converters)
            {
                if (converter is DetailsOPDataConverter)
                    (converter as DetailsOPDataConverter).KeyPair = itemKeyPair;
            }

            serializer.Populate(jsonRecord.CreateReader(), record);

            return record;
        }
    }
}
