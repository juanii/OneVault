using KeePassLib;

namespace OneVault.Records
{
    public class WebFormRecord : ItemRecord
    {
#pragma warning disable IDE1006
        public WebFormDetails d { get; set; }

        public SubmittableOverview o { get; set; }
#pragma warning restore IDE1006

        protected override Details GetSecureContents() { return this.d; }

        protected override Overview GetOpenContents() { return this.o; }

        public override void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            base.PopulateEntry(pwEntry, pwDatabase, userPrefs);
            pwEntry.IconId = PwIcon.World;
        }
    }
}
