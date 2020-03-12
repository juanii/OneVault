using Newtonsoft.Json;
using OneVault.Converters;
using OneVault.OPVault;
using System;

namespace OneVault.Records
{
    public class Profile
    {
        [JsonConverter(typeof(UUIDConverter))]
        public Guid uuid { get; set; }

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime updatedAt { get; set; }

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime createdAt { get; set; }

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime tx { get; set; }

        public string profileName { get; set; }

        public byte[] salt { get; set; }

        public int iterations { get; set; }

        public KeyPair masterKey { get; set; }

        public KeyPair overviewKey { get; set; }

        public byte[] iconData { get; set; }

        public double cA { get; set; }

        public double cB { get; set; }

        public double cS { get; set; }

        public double cH { get; set; }

        public string passwordHint { get; set; }

        public bool hasColor { get; set; }
    }
}
