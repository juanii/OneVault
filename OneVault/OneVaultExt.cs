﻿using KeePass.Plugins;
using System.Reflection;

namespace OneVault
{
    public class OneVaultExt : Plugin
    {
        private IPluginHost pluginHost = null;
        private OneVaultFormatProvider oneVaultFormatProvider;

        public override bool Initialize(IPluginHost host)
        {
            pluginHost = host;

            /* Ughh...
             * While creating the PLGX package, KeePass embeds the localized resource files in
             * the main assembly DLL, but the .NET ResourceManager class can only find localized
             * resources in satellite assembly DLLs.  The ResourceManager is instantiated in the
             * resource's designer source code file, which is automatically re-generated by the
             * ResXFileCodeGenerator tool every time a resource is modified. So it's tricky to
             * replace the default ResourceManager with our custom class. Manually editing the
             * code is very inconvenient. Automated find-and-replace requires third party tools
             * like F.A.R.T., so this hacky solution was chosen.
             * https://stackoverflow.com/questions/35987762/resxfilecodegenerator-overriding-the-output-want-to-use-a-custom-resourcemana
             */
            FieldInfo resourceManagerField = typeof(Properties.Strings).GetField("resourceMan", BindingFlags.NonPublic | BindingFlags.Static);
            if (resourceManagerField != null)
                resourceManagerField.SetValue(null, new EmbeddedResourceManager("OneVault.Properties.Strings", typeof(Properties.Strings).Assembly));

            oneVaultFormatProvider = new OneVaultFormatProvider();
            pluginHost.FileFormatPool.Add(oneVaultFormatProvider);

            return true;
        }

        public override void Terminate()
        {
            pluginHost.FileFormatPool.Remove(oneVaultFormatProvider);
        }
    }
}
