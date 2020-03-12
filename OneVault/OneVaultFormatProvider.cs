using KeePass.DataExchange;
using KeePassLib;
using KeePassLib.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OneVault
{
    public enum ItemCategory
    {
        Unknown,
        Logins,
        SecureNotes,
        CreditCards,
        Passwords,
        Identities,
        BankAccounts,
        Databases,
        DriverLicenses,
        Memberships,
        EmailAccounts,
        OutdoorLicenses,
        RewardPrograms,
        Passports,
        Servers,
        SocialSecurityNumbers,
        WirelessRouters,
        SoftwareLicenses,
    }

    public class OneVaultFormatProvider : FileFormatProvider
    {
        private readonly OneVaultParser oneVaultParser = new OneVaultParser();
        private readonly OneVaultImporter oneVaultImporter = new OneVaultImporter();

        public override bool SupportsImport
        {
            get { return true; }
        }

        public override bool SupportsExport
        {
            get { return false; }
        }

        public override string FormatName
        {
            get { return "1Password OPVault"; }
        }

        public override string ApplicationGroup
        {
            get { return KeePass.Resources.KPRes.PasswordManagers; }
        }

        public override string DefaultExtension
        {
            get { return "opvault"; }
        }

        public override bool ImportAppendsToRootGroupOnly
        {
            get { return false; }
        }

        public override bool RequiresFile
        {
            get { return false; }
        }

        public override Image SmallIcon
        {
            get { return Properties.Icons.OneVault_icon_16; }
        }

        public override void Import(PwDatabase pwDatabase, Stream input, IStatusLogger statusLogger)
        {
            ConfigurationForm configurationForm = new ConfigurationForm();

            statusLogger.StartLogging("OPVault file import", true);

            if (configurationForm.ShowDialog() == DialogResult.OK)
            {
                statusLogger.SetText("Parsing OPVault file...", LogStatusType.Info);

                string defaultDir = Path.Combine(configurationForm.ImportDirPath, "default");
                Records.Profile profile = oneVaultParser.ParseProfile(Path.Combine(defaultDir, "profile.js"), configurationForm.MasterPassword);

                if (profile == null)
                {
                    MessageBox.Show("Could not read profile file.", "Invalid OPVault", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (profile.masterKey == null || profile.overviewKey == null)
                {
                    MessageBox.Show("Please check the master password is correct.", "Error processing OPVault", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    List<Records.BaseRecord> records = oneVaultParser.ParseFolders(Path.Combine(defaultDir, "folders.js"), profile);
                    Dictionary<string, Records.AttachmentMetadata> attachments = new Dictionary<string, Records.AttachmentMetadata>();

                    for (int i = 0; i < 16; i++)
                    {
                        records.AddRange(oneVaultParser.ParseItems(Path.Combine(defaultDir, string.Format("band_{0:X}.js", i)), profile));
                    }

                    foreach (string attachmentFilePath in Directory.GetFiles(defaultDir, "*.attachment"))
                    {
                        Records.AttachmentMetadata attachmentMetadata = oneVaultParser.ParseAttachments(attachmentFilePath, profile);
                        Records.BaseRecord item = records.Find(r => r.uuid.Equals(attachmentMetadata.itemUUID));
                        if (item is Records.ItemRecord)
                            (item as Records.ItemRecord).AddAttachment(attachmentFilePath, attachmentMetadata);
                    }

                    statusLogger.SetText(string.Format("Importing {0} parsed records...", records.Count), LogStatusType.Info);
                    oneVaultImporter.Import(records, pwDatabase, statusLogger, configurationForm.GetUserPrefs());
                    statusLogger.SetText("Finished import.", LogStatusType.Info);
                }
            }
            else
            {
                statusLogger.SetText("Import cancelled.", LogStatusType.Info);
            }

            statusLogger.EndLogging();
        }
    }
}
