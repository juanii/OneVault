using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace OneVault
{
    public class OneVaultParser
    {
        public Records.Profile ParseProfile(string profileFilePath, string masterPassword)
        {
            if (!File.Exists(profileFilePath))
            {
                MessageBox.Show("No profile file", "Invalid OPVault", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings()
            {
                Converters = new JsonConverter[] {
                    new Converters.ProfileConverter(masterPassword),
                    new Converters.HashedKeyPairConverter()
                }
            });

            using (FileStream fs = File.OpenRead(profileFilePath))
            using (TextReader tr = new StreamReader(fs))
            {
                if (this.findJSONObject(tr))
                {
                    using (JsonTextReader jtr = new JsonTextReader(tr))
                    {
                        return serializer.Deserialize<Records.Profile>(jtr);
                    }
                }
            }

            return null;
        }

        public List<Records.BaseRecord> ParseFolders(string foldersFilePath, Records.Profile profile)
        {
            List<Records.BaseRecord> folders = new List<Records.BaseRecord>();

            if (File.Exists(foldersFilePath))
            {
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings()
                {
                    Converters = new JsonConverter[] {
                        new Converters.DetailsOPDataConverter(),
                        new Converters.OverviewOPDataConverter(profile.overviewKey),
                    }
                });

                using (FileStream fs = File.OpenRead(foldersFilePath))
                using (TextReader tr = new StreamReader(fs))
                {
                    if (this.findJSONObject(tr))
                    {
                        using (JsonTextReader jtr = new JsonTextReader(tr))
                        {
                            while (jtr.Read())
                            {
                                if (jtr.TokenType == JsonToken.StartObject && jtr.Depth > 0)
                                {
                                    folders.Add(serializer.Deserialize<Records.FolderRecord>(jtr));
                                }
                                else if (jtr.TokenType == JsonToken.EndObject && jtr.Depth == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return folders;
        }

        public List<Records.BaseRecord> ParseItems(string bandFilePath, Records.Profile profile)
        {
            List<Records.BaseRecord> records = new List<Records.BaseRecord>();

            if (File.Exists(bandFilePath))
            {
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings()
                {
                    Converters = new JsonConverter[] {
                        new Converters.ItemRecordConverter(profile.masterKey),
                        new Converters.DetailsOPDataConverter(),
                        new Converters.EncryptedKeyPairConverter(profile.masterKey),
                        new Converters.OverviewOPDataConverter(profile.overviewKey),
                    }
                });

                using (FileStream fs = File.OpenRead(bandFilePath))
                using (TextReader tr = new StreamReader(fs))
                {
                    if (this.findJSONObject(tr))
                    {
                        using (JsonTextReader jtr = new JsonTextReader(tr))
                        {
                            while (jtr.Read())
                            {
                                if (jtr.TokenType == JsonToken.StartObject && jtr.Depth > 0)
                                {
                                    records.Add(serializer.Deserialize<Records.ItemRecord>(jtr));
                                }
                                else if (jtr.TokenType == JsonToken.EndObject && jtr.Depth == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return records;
        }

        public Records.AttachmentMetadata ParseAttachments(string attachmentFilePath, Records.Profile profile)
        {
            if (File.Exists(attachmentFilePath))
            {
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings()
                {
                    Converters = new JsonConverter[] {
                        new Converters.OverviewOPDataConverter(profile.overviewKey),
                    }
                });

                using (FileStream fs = File.OpenRead(attachmentFilePath))
                {
                    string jsonString = OPVault.OPData.ReadAttachmentMetadata(attachmentFilePath);
                    using (TextReader tr = new StringReader(jsonString))
                    using (JsonTextReader jtr = new JsonTextReader(tr))
                    {
                        return serializer.Deserialize<Records.AttachmentMetadata>(jtr);
                    }
                }
            }

            return null;
        }

        private bool findJSONObject(TextReader reader)
        {
            int num = reader.Peek();

            while (num != '{' && num != -1)
            {
                reader.Read();
                num = reader.Peek();
            }

            return num == '{';
        }
    }
}
