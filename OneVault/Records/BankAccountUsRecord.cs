using KeePassLib;
using Newtonsoft.Json;

namespace OneVault.Records
{
    public class BankAccountUsDetails : PasswordHistoryDetails
    {
        protected override SectionFieldLocator GetPasswordFieldLocator() { return new SectionFieldLocator(string.Empty, "telephonePin"); }
    }

    public class BankAccountUsRecord : ItemRecord
    {
#pragma warning disable IDE1006
        public BankAccountUsDetails d { get; set; }

        public ItemOverview o { get; set; }
#pragma warning restore IDE1006

        protected override Details GetSecureContents() { return this.d; }

        protected override Overview GetOpenContents() { return this.o; }

        public override void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            base.PopulateEntry(pwEntry, pwDatabase, userPrefs);
            pwEntry.IconId = PwIcon.Homebanking;
        }
    }
}
