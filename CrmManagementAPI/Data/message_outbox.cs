using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class message_outbox
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("channel")]
        public string Channel { get; set; } = string.Empty;

        [Required]
        [Column("use_case")]
        public string UseCase { get; set; } = string.Empty;

        [Column("template_id")]
        public int? TemplateId { get; set; }

        [Column("invoice_id")]
        public int? InvoiceId { get; set; }

        [Column("tender_id")]
        public int? TenderId { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("recipient_name")]
        public string? RecipientName { get; set; }

        [Column("recipient_email")]
        public string? RecipientEmail { get; set; }

        [Column("recipient_mobile")]
        public string? RecipientMobile { get; set; }

        [Column("subject")]
        public string? Subject { get; set; }

        [Required]
        [Column("body_rendered")]
        public string BodyRendered { get; set; } = string.Empty;

        [Column("status")]
        public string Status { get; set; } = "Queued";

        [Column("sent_by_id")]
        public int? SentById { get; set; }

        [Column("scheduled_at")]
        public DateTime? ScheduledAt { get; set; }

        [Column("sent_at")]
        public DateTime? SentAt { get; set; }

        [Column("delivery_ref")]
        public string? DeliveryRef { get; set; }

        [Column("error_message")]
        public string? ErrorMessage { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
