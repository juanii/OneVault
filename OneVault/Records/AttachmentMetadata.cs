using Newtonsoft.Json;
using OneVault.Converters;
using System;

namespace OneVault.Records
{
    public class AttachmentMetadata
    {
        [JsonConverter(typeof(UUIDConverter))]
        public Guid itemUUID;

        [JsonConverter(typeof(UUIDConverter))]
        public Guid uuid;

        public long contentsSize;

        [JsonConverter(typeof(FlexibleBooleanConverter))]
        public bool external;

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime updatedAt;

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime txTimestamp;

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime createdAt;

        public AttachmentOverview overview;
    }
}
