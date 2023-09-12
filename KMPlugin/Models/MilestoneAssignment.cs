namespace KMPlugin.Models
{
    internal class MilestoneAssignment
    {
        public string MilestoneName { get; set; }

        public string UserAssigned { get; set; }

        public MilestoneAssignment(string msName, string user)
        {
            if (msName == null)
            {
                MilestoneName = "";
            }
            else
            {
                MilestoneName = msName;
            }
            if (user == null)
            {
                UserAssigned = "";
            } else {
                UserAssigned = user;
            }
        }
    }
}
