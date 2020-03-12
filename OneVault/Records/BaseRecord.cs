using KeePassLib;
using Newtonsoft.Json;
using OneVault.Converters;
using System;

namespace OneVault.Records
{
    public class BaseRecord
    {
#pragma warning disable IDE1006
        [JsonConverter(typeof(UUIDConverter))]
        public Guid uuid { get; set; }

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime created { get; set; }

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime updated { get; set; }

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime tx { get; set; }
#pragma warning restore IDE1006

        public PwEntry CreatePwEntry(PwDatabase pwStorage, UserPrefs userPrefs)
        {
            PwEntry pwEntry = new PwEntry(true, true);

            this.PopulateEntry(pwEntry, pwStorage, userPrefs);

            return pwEntry;
        }

        public virtual void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            pwEntry.CreationTime = this.created;
            pwEntry.LastModificationTime = this.updated;
        }
    }
}
