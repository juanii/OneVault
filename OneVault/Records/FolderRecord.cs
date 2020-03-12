using Newtonsoft.Json;
using OneVault.Converters;
using System;

namespace OneVault.Records
{
    [JsonConverter(typeof(FolderRecordConverter))]
    public class FolderRecord : BaseRecord
    {
#pragma warning disable IDE1006
        [JsonConverter(typeof(UUIDConverter))]
        public Guid parent { get; set; }

        public bool smart { get; set; }
#pragma warning restore IDE1006
    }
}
