using KeePassLib;

namespace OneVault.Records
{
    public class DatabaseDetails : PasswordHistoryDetails
    {
        protected override SectionFieldLocator GetUsernameFieldLocator() { return new SectionFieldLocator(string.Empty, "username"); }

        protected override SectionFieldLocator GetPasswordFieldLocator() { return new SectionFieldLocator(string.Empty, "password"); }

        protected override SectionFieldLocator GetURLFieldLocator() { return new SectionFieldLocator(string.Empty, "hostname"); }
    }

    public class DatabaseRecord : ItemRecord
    {
#pragma warning disable IDE1006
        public DatabaseDetails d { get; set; }

        public ItemOverview o { get; set; }
#pragma warning restore IDE1006

        protected override Details GetSecureContents() { return this.d; }

        protected override Overview GetOpenContents() { return this.o; }

        public override void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            base.PopulateEntry(pwEntry, pwDatabase, userPrefs);
            pwEntry.IconId = PwIcon.Drive;
        }
    }
}
