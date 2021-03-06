﻿using KeePassLib;
using KeePassLib.Security;
using Newtonsoft.Json;
using OneVault.Converters;
using System;
using System.Collections.Generic;

namespace OneVault.Records
{
    public class PasswordHistory
    {
#pragma warning disable IDE1006
        public string value { get; set; }

        [JsonConverter(typeof(UnixEpochConverter))]
        public DateTime time { get; set; }
#pragma warning restore IDE1006
    }

    //wallet.financial.CreditCard (history field: cvv)
    //wallet.financial.BankAccountUS (history field: telephonePin)
    //wallet.computer.Database (history field: password)
    //wallet.membership.Membership (history field: pin)
    //wallet.onlineservices.Email.v2 (history field: pop_password)
    //wallet.membership.RewardProgram (history field: pin)
    //wallet.computer.UnixServer (history field: password)
    //wallet.government.SsnUS (history field: number)
    //wallet.computer.Router (history field: password)
    public class PasswordHistoryDetails : Details
    {
#pragma warning disable IDE1006
        public List<PasswordHistory> passwordHistory { get; set; }
#pragma warning restore IDE1006

        public void CreateHistoryEntries(PwEntry pwEntry, PwDatabase pwDatabase, UserPrefs userPrefs)
        {
            if (this.passwordHistory != null)
            {
                List<PwEntry> historyEntries = new List<PwEntry>();

                foreach (PasswordHistory passwordHistory in this.passwordHistory)
                {
                    PwEntry historyEntry = pwEntry.CloneDeep();

                    historyEntry.CreationTime = passwordHistory.time;
                    historyEntry.LastModificationTime = passwordHistory.time;

                    historyEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(pwDatabase.MemoryProtection.ProtectPassword, passwordHistory.value));

                    historyEntries.Add(historyEntry);
                }

                foreach (PwEntry historyEntry in historyEntries)
                    pwEntry.History.Add(historyEntry);
            }
        }
    }
}
