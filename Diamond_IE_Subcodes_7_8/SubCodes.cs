using System.Collections.Generic;

namespace Diamond_IE_Subcodes_7_8
{
    public class SubCode7_8
    {
        public string PublicationNumber { get; set; }
        public List<IPC_Classification> IpcClassifications { get; set; }
        public string Title { get; set; }
        public List<string> Grantee_Assignee_OwnerInformation { get; set; }
        public string LegalEventDate { get; set; }
    }

    public class IPC_Classification
    {
        public string Classification { get; set; }
        public string IPC_Version { get; set; }
    }
}
