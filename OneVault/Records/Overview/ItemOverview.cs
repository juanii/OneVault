using KeePassLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;

namespace OneVault.Records
{
    //wallet.financial.CreditCard
    //identities.Identity
    //wallet.financial.BankAccountUS
    //wallet.computer.Database
    //wallet.government.DriversLicense
    //wallet.membership.Membership
    //wallet.onlineservices.Email.v2
    //wallet.government.HuntingLicense
    //wallet.membership.RewardProgram
    //wallet.government.Passport
    //wallet.computer.UnixServer
    //wallet.government.SsnUS
    //wallet.computer.Router
    //wallet.computer.License
    public class ItemOverview : Overview
    {
#pragma warning disable IDE1006
        public List<string> tags { get; set; }

        public ulong faveIndex { get; set; }

        public int ps { get; set; }

        public string ainfo { get; set; }

        public byte[] icon { get; set; }
#pragma warning restore IDE1006

        public PwCustomIcon PwCustomIcon { get; private set; }

        public override void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            base.PopulateEntry(pwEntry, pwDatabase, userPrefs);

            if (this.tags != null)
                pwEntry.Tags.AddRange(this.tags);

            if (this.faveIndex > 0 && !pwEntry.Tags.Contains(Properties.Strings.Tag_Favorite))
                pwEntry.Tags.Add(Properties.Strings.Tag_Favorite);

            if (this.icon != null)
            {
                byte[] customIconData = null;

                try
                {
                    using (MemoryStream originalIconStream = new MemoryStream(this.icon))
                    using (Bitmap originalIcon = new Bitmap(originalIconStream))
                    using (MemoryStream convertedIconStream = new MemoryStream())
                    {
                        originalIcon.Save(convertedIconStream, ImageFormat.Png);
                        customIconData = convertedIconStream.ToArray();

                        using (MD5 md5 = MD5.Create())
                        {
                            PwUuid customIconUuid = new PwUuid(md5.ComputeHash(customIconData));
                            this.PwCustomIcon = new PwCustomIcon(customIconUuid, customIconData);
                            pwEntry.CustomIconUuid = customIconUuid;
                        }
                    }
                }
                catch (ArgumentException)
                {
                    // Image format is not supported or one of its dimensions is bigger than 65,535
                }
            }
        }
    }
}
