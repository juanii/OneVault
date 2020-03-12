using KeePassLib;
using KeePassLib.Interfaces;
using System;
using System.Collections.Generic;

namespace OneVault
{
    public class OneVaultImporter
    {
        private static readonly Dictionary<string, ItemCategory> categoriesByRecordType = new Dictionary<string, ItemCategory>()
        {
            { "001", ItemCategory.Logins },
            { "002", ItemCategory.CreditCards },
            { "003", ItemCategory.SecureNotes },
            { "004", ItemCategory.Identities },
            { "005", ItemCategory.Passwords },
            { "100", ItemCategory.SoftwareLicenses },
            { "101", ItemCategory.BankAccounts },
            { "102", ItemCategory.Databases },
            { "103", ItemCategory.DriverLicenses },
            { "104", ItemCategory.OutdoorLicenses },
            { "105", ItemCategory.Memberships },
            { "106", ItemCategory.Passports },
            { "107", ItemCategory.RewardPrograms },
            { "108", ItemCategory.SocialSecurityNumbers },
            { "109", ItemCategory.WirelessRouters },
            { "110", ItemCategory.Servers },
            { "111", ItemCategory.EmailAccounts },
        };

        public void Import(List<Records.BaseRecord> records, PwDatabase pwDatabase, IStatusLogger statusLogger, UserPrefs userPrefs)
        {
            List<Records.ItemRecord> trashedRecords = null;

            records.RemoveAll(record => record is Records.SavedSearchRecord);

            // Copy trashed items to another collection
            if (userPrefs.KeepTrashedItems)
                trashedRecords = records.FindAll(record => (record is Records.ItemRecord) && (record as Records.ItemRecord).trashed).ConvertAll<Records.ItemRecord>(record => record as Records.ItemRecord);

            // Filter out trashed items from main set
            records.RemoveAll(record => (record is Records.ItemRecord) && (record as Records.ItemRecord).trashed);

            if (userPrefs.FolderLayout == FolderLayout.Category)
            {
                // Since they're not being used, filter out user-defined folders from main set
                records = records.FindAll(record => record is Records.ItemRecord);
                
                // Assign parent folder to items according to their category
                foreach (Records.ItemRecord itemRecord in records.ConvertAll(record => record as Records.ItemRecord))
                {
                    ItemCategory itemCategory = ItemCategory.Unknown;

                    if (!categoriesByRecordType.TryGetValue(itemRecord.category, out itemCategory))
                        itemCategory = ItemCategory.Unknown;

                    Records.RegularFolderRecord categoryFolder = Records.RegularFolderRecord.FolderRecordForCategory(itemCategory);

                    if (!records.Contains(categoryFolder))
                        records.Add(categoryFolder);

                    itemRecord.folder = categoryFolder.uuid;
                }
            }
            else if (userPrefs.FolderLayout == FolderLayout.UserDefined)
            {
                Records.RegularFolderRecord unassignedFolder = null;

                // Assign orphan items the "Unassigned" folder as parent
                foreach (Records.ItemRecord itemRecord in records.FindAll(record => Guid.Empty.Equals((record as Records.ItemRecord).folder)))
                {
                    if (unassignedFolder == null)
                    {
                        unassignedFolder = Records.RegularFolderRecord.UnassignedFolderRecord;
                        records.Add(unassignedFolder);
                    }

                    itemRecord.folder = Records.RegularFolderRecord.UnassignedFolderRecord.uuid;
                }
            }

            IEnumerable<TreeNode<Records.BaseRecord>> tree = buildTree(records);

            PwGroup rootGroup = null;

            if (userPrefs.CreateParentFolder)
                rootGroup = new PwGroup(true, true) { Name = "1Password Import on " + DateTimeFormatter.FormatDateTime(DateTime.Now, userPrefs.DateFormat) };
            else
                rootGroup = pwDatabase.RootGroup;

            foreach (TreeNode<Records.BaseRecord> node in tree)
                importRecord(node, rootGroup, pwDatabase, userPrefs);

            if (trashedRecords != null && trashedRecords.Count > 0)
            {
                PwGroup trashGroup = Records.RegularFolderRecord.TrashFolderRecord.CreatePwGroup();

                foreach (Records.ItemRecord trashedRecord in trashedRecords)
                {
                    PwEntry entry = trashedRecord.CreatePwEntry(pwDatabase, userPrefs);
                    trashGroup.AddEntry(entry, true);
                }

                rootGroup.AddGroup(trashGroup, true);
            }

            if (userPrefs.CreateParentFolder)
                pwDatabase.RootGroup.AddGroup(rootGroup, true);
        }

        private void importRecord(TreeNode<Records.BaseRecord> currentNode, PwGroup pwGroupAddTo, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            Records.BaseRecord record = currentNode.AssociatedObject;

            if (record is Records.RegularFolderRecord)
            {
                PwGroup folder = (record as Records.RegularFolderRecord).CreatePwGroup();

                if (folder != null)
                {
                    pwGroupAddTo.AddGroup(folder, true);

                    foreach (TreeNode<Records.BaseRecord> child in currentNode.Children)
                        importRecord(child, folder, pwDatabase, userPrefs);
                }
            }
            else
            {
                Records.ItemRecord itemRecord = record as Records.ItemRecord;

                PwEntry entry = itemRecord.CreatePwEntry(pwDatabase, userPrefs);

                if (entry != null)
                {
                    PwCustomIcon customIcon = itemRecord.GetPwCustomIcon();

                    if (customIcon != null)
                    {
                        if (!pwDatabase.CustomIcons.Exists(icon => icon.Uuid.Equals(customIcon.Uuid)))
                            pwDatabase.CustomIcons.Add(customIcon);

                        pwDatabase.UINeedsIconUpdate = true;
                    }

                    pwGroupAddTo.AddEntry(entry, true);
                }
            }
        }

        private IEnumerable<TreeNode<Records.BaseRecord>> buildTree(List<Records.BaseRecord> records)
        {
            Dictionary<Guid, TreeNode<Records.BaseRecord>> lookUp = new Dictionary<Guid, TreeNode<Records.BaseRecord>>();
            records.ForEach(x => lookUp.Add(x.uuid, new TreeNode<Records.BaseRecord>() { AssociatedObject = x }));

            foreach (TreeNode<Records.BaseRecord> node in lookUp.Values)
            {
                TreeNode<Records.BaseRecord> parent;
                Guid parentUuid = Guid.Empty;

                if (node.AssociatedObject is Records.ItemRecord)
                    parentUuid = (node.AssociatedObject as Records.ItemRecord).folder;
                else if (node.AssociatedObject is Records.FolderRecord)
                    parentUuid = (node.AssociatedObject as Records.FolderRecord).parent;

                if (!Guid.Empty.Equals(parentUuid))
                {
                    if (lookUp.TryGetValue(parentUuid, out parent))
                    {
                        node.Parent = parent;
                        parent.Children.Add(node);
                    }
                }
            }

            List<TreeNode<Records.BaseRecord>> rootNodes = new List<TreeNode<Records.BaseRecord>>();

            foreach (var value in lookUp.Values)
            {
                if (value.Parent == null)
                    rootNodes.Add(value);
            }

            return rootNodes;
        }
    }
}
