using KeePassLib;
using KeePassLib.Security;
using System.Collections.Generic;

namespace OneVault.Records
{
    public class URL
    {
#pragma warning disable IDE1006
        public string l { get; set; }

        public string u { get; set; }
#pragma warning restore IDE1006
    }

    //passwords.Password
    public class SiteOverview : ItemOverview
    {
#pragma warning disable IDE1006
        public string url { get; set; }

        public List<URL> URLs { get; set; }
#pragma warning restore IDE1006

        public override void PopulateEntry(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            base.PopulateEntry(pwEntry, pwDatabase, userPrefs);

            if (this.URLs != null)
            {
                int i = 1;

                string mainURLString = null;

                if (pwEntry.Strings.Exists(PwDefs.UrlField))
                    mainURLString = pwEntry.Strings.Get(PwDefs.UrlField).ReadString();

                foreach (URL url in this.URLs)
                {
                    if (url.u.Equals(mainURLString))
                        continue;

                    string urlLabel = url.l;

                    if (string.IsNullOrEmpty(url.l))
                        urlLabel = string.Format("URL {0}", i++);

                    pwEntry.Strings.Set(urlLabel, new ProtectedString(pwDatabase.MemoryProtection.ProtectUrl, url.u));
                }
            }
        }
    }
}
