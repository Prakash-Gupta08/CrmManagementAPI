using System;

namespace CrmManagementAPI.Model
{
    public class CreateTenderAlert
    {
        public int TenderId { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public DateOnly AlertDate { get; set; }
        public string Status { get; set; } = "Open";
        public string? Notes { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
