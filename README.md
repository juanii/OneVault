# OneVault

OneVault is a [KeePass](https://keepass.info/) 2.x plugin to import 1Password vaults stored in OPVault format.

This plugin is [OnePIF](https://github.com/juanii/OnePIF) sidekick. Check the [Pros and cons of using OPVault vs. 1PIF](#pros-and-cons) section to decide which plugin is right for you.

This plugin takes some code from Dmitry Wolf's [1P2KeePass](https://github.com/diimdeep/1P2KeePass) I was lazy to rewrite myself and uses a PBKDF2 implementation by [Josip Medved](https://www.medo64.com/2012/04/pbkdf2-with-sha-256-and-others/)

## Features and support

### Brief list of features

* The plugin imports:
  - Most item types available in 1Password 6 and 7: Logins, Secure Notes, Credit Cards, Identities, Passwords, Bank Accounts, Databases, Driver Licenses, Email Accounts, Memberships, Outdoor Licenses, Passports, Reward Programs, Servers, Social Security Numbers, Software Licenses, Wireless Routers, User-defined Folders.<sup id="a1">[1](#f1)</sup>
  - Legacy item types from 1Password 3 converted to newer item types by 1Password when saving older vaults using the OPVault format: Email Account (v1), iTunes, MySQL Database, FTP Account, iCloud (a.k.a. MobileMe), Generic Account, Instant Messenger, Internet Provider, Amazon S3 (a.k.a. Amazon Web Services).
  - All template fields, web form fields and user-defined fields.
  - Previously used passwords list.
  - User-defined tags.
  - Favorite flag (as a tag).
  - Trashed items.
  - Custom user-provided icons<sup id="a2">[2](#f2)</sup> (not the rich icons provided by AgileBits).
  - File attachments.
  - TOTP fields formatted for KeeWeb, Tray TOTP or KeeOtp.
* Organizes items into the standard 1Password categories or using pre-existent user-defined folders.
* Proper handling of line endings in multiline fields.
* Customizable date formats: ISO 8601 or user locale.
* Customizable address formats: one-liner, multi-line in a single field or one field for each address component.

### (Un)tested and (un)supported file formats and platforms

* The plugin was tested using files created in 1Password 3, 6 and 7 for macOS and 1Password 4 for Windows and exported using 1Password 6 and 7 for macOS.
* All item types (from the above list in previous section) and template fields were tested.
* The plugin was tested with KeePass 2.40+ running on Windows.

### What's next

I expect to keep fixing bugs and adding some features. Here are some of the plans, in no particular order:

* Verify HMAC signature at band file level and item level.
* General error handling.
* Log/summary of import errors.

## Download and installation

You can get the latest release from the [Releases](https://github.com/juanii/OneVault/releases/latest) page. To install, unpack the archive and copy its contents to the KeePass Plugins directory. As stated in the [KeePass 2.x Plugins page](https://keepass.info/help/v2/plugins.html) the package `mono-complete` might be required on some Linux systems for the PLGX file to compile.

If you're using KeePass 2.08 or older, you'll have to build the DLL version of the plugin. See the [Building and debugging](#building-and-debugging) section for instructions.

## <span id="pros-and-cons">Pros and cons of using OPVault vs. 1PIF</span>

### Pros

* OPVault is an [almost perfectly](https://discussions.agilebits.com/discussion/100882/suggestion-to-enhance-the-opvault-design-document) documented format. This makes it highly improbable to come across a file that will fail to be imported. If it ever happens, a bug in the plugin is probably to be blamed.
  - 1PIF format is almost perfectly _undocumented_ and, to make things worse, there's a whole spectrum of different things one can find inside 1PIF files exported from different 1Password versions.
* OPVault is encrypted at rest (i.e. while saved on a disk) and only minimal parts of it are in plain text so you can rest easy knowing your secrets are safe. Nevertheless, **the vault is decrypted in memory during the import process** and the plugin code is probably not as secure as 1Passwords' so please use it in safe environments.
  - As you should already know from the big red legend while exporting them, 1PIF files are stored _completely_ in plain text, including all the secrets, so you must make sure to scrub really hard to remove any traces of the file after using it.

### Cons

* You can't directly export vaults from 1Password in OPVault format. The OPVault format is used for synchronization between different instances of the software. To get your hands over an OPVault bundle you must set your vault to be synced, locate and copy the resulting bundle.
  - 1PIF files are easily obtained using the normal 1Password export function.

## <span id="building-and-debugging">Building and debugging</span>

### Prerequisites

Before building the plugin you must either download and place a copy of KeePass software in the `KeePass` directory inside the solution directory, or adjust the paths all over the projects<sup id="a4">[4](#f4)</sup> to point to your current KeePass installation.

The plugin depends on [Newtonsoft Json.NET](https://www.newtonsoft.com/). If you're using Visual Studio, enable the NuGet automatic download and installation of missing packages in `Tools > Options > NuGet Package Manager > General`. If you're using MSBuild use the `nuget restore` command to restore dependencies before building.

### Building the PLGX version

You don't have to actually compile anything, just build the `PackagePLGX` project which consists of only a few post-build commands. KeePass will compile the plugin code upon first load of the PLGX package.

### Building the DLL version

Build the `OneVault` project using the `Release` configuration and you're ready to go.

To install, copy the plugin DLL along with the dependecies and (optionally) the localization satellite DLLs to the KeePass Plugins path<sup id="a5">[5](#f5).

### Debugging

To debug the plugin, configure `OneVault` project Debug settings to start the KeePass executable.

## Disclaimer

This software is provided as-is without any warranty of any kind. I take no responsability or liability for any damage it may cause. If it breaks your data you can keep its pieces.

**Vaults often contain very sensitive information. Thoroughly check imported data for completeness and correctess before deleting the original files.**

---

<b id="f1">1</b> Smart Folders are not supported since KeePass seems to lack a similar feature. [:leftwards_arrow_with_hook:](#a1)

<b id="f2">2</b> Only icons saved as image formats supported by .NET `System.Drawing.Bitmap` class, namely: BMP, GIF, EXIF, JPEG, PNG and TIFF. [:leftwards_arrow_with_hook:](#a2)

<b id="f3">3</b> Sample OPVault bundles completely or partially failing to be imported are welcome to expand support. **If they're from a real vault, don't forget to redact private information.** [:leftwards_arrow_with_hook:](#a3)

<b id="f4">4</b> Currently the post-build event in the PackagePLGX project, the build output path and the reference to the KeePass executable in the OneVault project are dependent on the KeePass installation path. [:leftwards_arrow_with_hook:](#a4)

<b id="f5">5</b> If you want to use the localization satellite DLLs and you're using KeePass version 2.40 or older, copy all the files to the _root_ KeePass installation path. Otherwise it will try (and fail) to load the satellite DLLs as plugins themselves. [:leftwards_arrow_with_hook:](#a5)
