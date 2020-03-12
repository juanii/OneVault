using KeePassLib;

namespace OneVault.Records
{
    public class EmailV2Details : PasswordHistoryDetails
    {
        protected override SectionFieldLocator GetUsernameFieldLocator() { return new SectionFieldLocator(string.Empty, "pop_username"); }

        protected override SectionFieldLocator GetPasswordFieldLocator() { return new SectionFieldLocator(string.Empty, "pop_password"); }

        protected override SectionFieldLocator GetURLFieldLocator() { return new SectionFieldLocator(string.Empty, "pop_server"); }
    }

    public class EmailV2Record : ItemRecord
    {
#pragma warning disable IDE1006
        public EmailV2Details d { get; set; }

        public ItemOverview o { get; set; }
#pragma warning restore IDE1006

        protected override Details GetSecureContents() { return this.d; }

        protected override Overview GetOpenContents() { return this.o; }

        public override void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            base.PopulateEntry(pwEntry, pwDatabase, userPrefs);
            pwEntry.IconId = PwIcon.EMail;
        }
    }
}
