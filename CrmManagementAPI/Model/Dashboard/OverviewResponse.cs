namespace CrmManagementAPI.Model.Dashboard
{
    public class OverviewResponse
    {
        public int TotalCustomer { get; set; }
        public int TotalUser { get; set; }
        public int CustomerAddedThisWeek { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
