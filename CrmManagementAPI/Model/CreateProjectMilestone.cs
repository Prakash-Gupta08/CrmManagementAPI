using System;

namespace CrmManagementAPI.Model
{
    public class CreateProjectMilestone
    {
        public int PoId { get; set; }
        public string MilestoneName { get; set; } = string.Empty;
        public string? MilestoneType { get; set; }
        public DateOnly? PlannedDate { get; set; }
        public DateOnly? ActualDate { get; set; }
        public decimal? BillingPct { get; set; }
        public decimal? BillingAmountInr { get; set; }
        public string Status { get; set; } = "Planned";
        public string? CompletionNotes { get; set; }
        public bool Billable { get; set; } = false;
        public bool Invoiced { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
    }
}
