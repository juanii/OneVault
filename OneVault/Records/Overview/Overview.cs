using KeePassLib;
using KeePassLib.Security;

namespace OneVault.Records
{
    // regular folders
    public class Overview
    {
#pragma warning disable IDE1006
        public string title { get; set; }
#pragma warning restore IDE1006

        public virtual void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            if (!string.IsNullOrEmpty(this.title))
                pwEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(pwDatabase.MemoryProtection.ProtectTitle, this.title));
        }
    }
}
