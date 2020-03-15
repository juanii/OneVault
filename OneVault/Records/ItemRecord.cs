using KeePassLib;
using KeePassLib.Security;
using Newtonsoft.Json;
using OneVault.Converters;
using OneVault.OPVault;
using System;
using System.Collections.Generic;
using System.IO;

namespace OneVault.Records
{
    public class ItemRecord : BaseRecord
    {
#pragma warning disable IDE1006
        //[JsonConverter(typeof(ItemCategory))]
        public string category { get; set; }

        [JsonConverter(typeof(UUIDConverter))]
        public Guid folder { get; set; }

        public ulong fave { get; set; }

        [JsonConverter(typeof(FlexibleBooleanConverter))]
        public bool trashed { get; set; }

        public KeyPair k { get; set; }

        public byte[] hmac { get; set; }

        public string scope { get; set; }
#pragma warning restore IDE1006

        protected virtual Details GetSecureContents() { return null; }

        protected virtual Overview GetOpenContents() { return null; }

        private readonly Dictionary<string, AttachmentMetadata> attachmentsMetadata = new Dictionary<string, AttachmentMetadata>();

        public void AddAttachment(string attachmentFilePath, AttachmentMetadata attachmentMetadata)
        {
            attachmentsMetadata.Add(attachmentFilePath, attachmentMetadata);
        }

        public override void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            pwEntry.CreationTime = this.created;
            pwEntry.LastModificationTime = this.updated;

            Details secureContents = this.GetSecureContents();
            if (secureContents != null)
                secureContents.PopulateEntry(pwEntry, pwDatabase, userPrefs);

            Overview openContents = this.GetOpenContents();
            if (openContents != null)
                openContents.PopulateEntry(pwEntry, pwDatabase, userPrefs);

            if (this.fave > 0 && !pwEntry.Tags.Contains(Properties.Strings.Tag_Favorite))
                pwEntry.Tags.Add(Properties.Strings.Tag_Favorite);

            foreach (KeyValuePair<string, AttachmentMetadata> attachment in attachmentsMetadata)
            {
                if (File.Exists(attachment.Key))
                {
                    byte[] attachmentContents = OPData.ReadAttachmentData(attachment.Key, this.k);
                    if (attachmentContents != null)
                        pwEntry.Binaries.Set(attachment.Value.overview.filename, new ProtectedBinary(false, attachmentContents));
                }
            }

            if (secureContents is PasswordHistoryDetails)
                (secureContents as PasswordHistoryDetails).CreateHistoryEntries(pwEntry, pwDatabase, userPrefs);
        }

        public virtual PwCustomIcon GetPwCustomIcon()
        {
            ItemOverview itemSecureContents = this.GetOpenContents() as ItemOverview;

            if (itemSecureContents != null)
                return itemSecureContents.PwCustomIcon;

            return null;
        }
    }
}
