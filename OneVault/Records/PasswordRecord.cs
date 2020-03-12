using KeePassLib;
using KeePassLib.Security;

namespace OneVault.Records
{
    public class PasswordDetails : PasswordHistoryDetails
    {
#pragma warning disable IDE1006
        public string password { get; set; }
#pragma warning restore IDE1006
    }

    public class PasswordRecord : ItemRecord
    {
#pragma warning disable IDE1006
        public PasswordDetails d { get; set; }

        public SiteOverview o { get; set; }
#pragma warning restore IDE1006

        protected override Details GetSecureContents() { return this.d; }

        protected override Overview GetOpenContents() { return this.o; }

        public override void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            if (!string.IsNullOrEmpty(this.d.password))
                pwEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(pwDatabase.MemoryProtection.ProtectPassword, this.d.password));

            base.PopulateEntry(pwEntry, pwDatabase, userPrefs);

            pwEntry.IconId = PwIcon.Key;
        }
    }
}
