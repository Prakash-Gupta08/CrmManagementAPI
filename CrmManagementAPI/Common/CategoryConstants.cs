namespace CrmManagementAPI.Common
{
    public class CategoryConstants
    {
        public static readonly List<string> Categories = new()
        {
            "PSU Bank",
            "Private Bank",
            "NBFC",
            "Financial Regulator",
            "Insurance PSU",
            "Insurance Private",
            "Central PSU",
            "Central Ministry",
            "State Government",
            "Muncipal Corporation",
            "Fintech",
            "Payment Aggregator",
            "Stock Exchange",
            "Depository",
            "Healthcare",
            "Listed Corporated",
            "Consultant/PMC",
            "Other"
           
        };

        public static readonly List<string> Project_risk = new()
        {
            "Low",
            "Medium",
            "High"

        };
        public static readonly List<string> Decision = new()
        {
            "Pending",
            "Go",
            "Conditional Go",
            "No-Go"

        };
        public static readonly List<string> Invoice_status = new()
        {
            "Not Due",
            "Due",
            "Over Due",
            "Partially Paid ",
            "Paid",
            "Disputed"
        };
        public static readonly List<string> Activity_type = new()
        {
            "Call",
            "Meeting",
            "Email",
            "WhatsApp",
            "Site Visit",
            "Presentation",
            "Note",
            "Stage Change",
            "Follow-up"
        };
    }
}
