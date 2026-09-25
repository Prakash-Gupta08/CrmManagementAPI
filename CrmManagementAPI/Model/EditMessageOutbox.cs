using System;

namespace CrmManagementAPI.Model
{
    public class EditMessageOutbox
    {
        public int Id { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string UseCase { get; set; } = string.Empty;
        public int? TemplateId { get; set; }
        public int? InvoiceId { get; set; }
        public int? TenderId { get; set; }
        public int? CustomerId { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientMobile { get; set; }
        public string? Subject { get; set; }
        public string BodyRendered { get; set; } = string.Empty;
        public string Status { get; set; } = "Queued";
        public int? SentById { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string? DeliveryRef { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
