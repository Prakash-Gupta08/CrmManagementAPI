using System;

namespace CrmManagementAPI.Model
{
    public class EditLeadActivity
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public int? OwnerId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public string? FromStage { get; set; }
        public string? ToStage { get; set; }
        public string? Notes { get; set; }
        public string? NextAction { get; set; }
        public DateOnly? NextActionDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
