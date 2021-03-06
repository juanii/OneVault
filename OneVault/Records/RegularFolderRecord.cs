﻿using KeePassLib;
using System;
using System.Collections.Generic;

namespace OneVault.Records
{
    public class RegularFolderRecord : FolderRecord
    {
#pragma warning disable IDE1006
        public Overview overview { get; set; }
#pragma warning restore IDE1006

        //protected override Overview GetOpenContents() { return this.overview; }

        private static readonly Dictionary<ItemCategory, PwIcon> iconsByCategory = new Dictionary<ItemCategory, PwIcon>()
        {
            { ItemCategory.Unknown, PwIcon.Warning },
            { ItemCategory.Logins, PwIcon.World },
            { ItemCategory.SecureNotes, PwIcon.Note },
            { ItemCategory.CreditCards, PwIcon.Money },
            { ItemCategory.Passwords, PwIcon.Key },
            { ItemCategory.Identities, PwIcon.Identity },
            { ItemCategory.BankAccounts, PwIcon.Homebanking },
            { ItemCategory.Databases, PwIcon.Drive },
            { ItemCategory.DriverLicenses, PwIcon.Certificate },
            { ItemCategory.Memberships, PwIcon.Certificate },
            { ItemCategory.EmailAccounts, PwIcon.EMail },
            { ItemCategory.OutdoorLicenses, PwIcon.Certificate },
            { ItemCategory.RewardPrograms, PwIcon.Homebanking },
            { ItemCategory.Passports, PwIcon.Identity },
            { ItemCategory.Servers, PwIcon.NetworkServer },
            { ItemCategory.SocialSecurityNumbers, PwIcon.Identity },
            { ItemCategory.WirelessRouters, PwIcon.IRCommunication },
            { ItemCategory.SoftwareLicenses, PwIcon.Certificate }
        };

        private static Dictionary<Guid, PwIcon> iconsByUuid = new Dictionary<Guid, PwIcon>();

        private static Dictionary<ItemCategory, RegularFolderRecord> foldersByCategory = new Dictionary<ItemCategory, RegularFolderRecord>();

        private static RegularFolderRecord unassignedFolder = null;

        private static RegularFolderRecord trashFolder = null;

        public static RegularFolderRecord UnassignedFolderRecord
        {
            get
            {
                if (unassignedFolder == null)
                {
                    DateTime now = DateTime.Now;
                    unassignedFolder = new RegularFolderRecord()
                    {
                        overview = new Overview()
                        {
                            title = Properties.Strings.FolderName_Unassigned
                        },
                        created = now,
                        updated = now,
                        uuid = Guid.NewGuid()
                    };
                }

                return unassignedFolder;
            }
        }

        public static RegularFolderRecord TrashFolderRecord
        {
            get
            {
                if (trashFolder == null)
                {
                    DateTime now = DateTime.Now;
                    trashFolder = new RegularFolderRecord()
                    {
                        overview = new Overview()
                        {
                            title = Properties.Strings.FolderName_Trash
                        },
                        created = now,
                        updated = now,
                        uuid = Guid.NewGuid()
                    };
                    iconsByUuid.Add(trashFolder.uuid, PwIcon.TrashBin);
                }

                return trashFolder;
            }
        }

        public static RegularFolderRecord FolderRecordForCategory(ItemCategory itemCategory)
        {
            RegularFolderRecord folderRecord;

            if (!foldersByCategory.TryGetValue(itemCategory, out folderRecord))
            {
                DateTime now = DateTime.Now;
                folderRecord = new RegularFolderRecord()
                {
                    overview = new Overview()
                    {
                        title = Properties.Strings.ResourceManager.GetString(string.Concat("FolderName_", Enum.GetName(typeof(ItemCategory), itemCategory))),
                    },
                    created = now,
                    updated = now,
                    uuid = Guid.NewGuid()
                };
                foldersByCategory.Add(itemCategory, folderRecord);
                iconsByUuid.Add(folderRecord.uuid, iconsByCategory[itemCategory]);
            }

            return folderRecord;
        }

        public PwGroup CreatePwGroup()
        {
            PwIcon folderIcon;

            if (!iconsByUuid.TryGetValue(this.uuid, out folderIcon))
                folderIcon = PwIcon.Folder;

            PwGroup folder = new PwGroup(true, true)
            {
                Name = this.overview.title,
                CreationTime = this.created,
                LastModificationTime = this.updated,
                IconId = folderIcon,
            };

            return folder;
        }
    }
}
