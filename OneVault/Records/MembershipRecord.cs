using KeePassLib;

namespace OneVault.Records
{
    public class MembershipDetails : PasswordHistoryDetails
    {
        protected override SectionFieldLocator GetPasswordFieldLocator() { return new SectionFieldLocator(string.Empty, "pin"); }

        protected override SectionFieldLocator GetURLFieldLocator() { return new SectionFieldLocator(string.Empty, "website"); }
    }

    public class MembershipRecord : ItemRecord
    {
#pragma warning disable IDE1006
        public MembershipDetails d { get; set; }

        public ItemOverview o { get; set; }
#pragma warning restore IDE1006

        protected override Details GetSecureContents() { return this.d; }

        protected override Overview GetOpenContents() { return this.o; }

        public override void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            base.PopulateEntry(pwEntry, pwDatabase, userPrefs);
            pwEntry.IconId = PwIcon.Certificate;
        }
    }
}
