using System;

namespace CrmManagementAPI.Model
{
    public class EditTenderActivity
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public int? OwnerId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public DateTimeOffset ActivityDate { get; set; }
        public string? Outcome { get; set; }
        public string? NextAction { get; set; }
        public DateOnly? NextActionDate { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
