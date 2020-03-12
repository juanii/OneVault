namespace OneVault.Records
{
    public class SavedSearchRecord : FolderRecord
    {
#pragma warning disable IDE1006
        public SavedSearchOverview overview { get; set; }
#pragma warning restore IDE1006

        //public override Overview GetOpenContents() { return this.openContents; }
    }
}
