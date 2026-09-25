using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class payments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Required]
        [Column("payment_date")]
        public DateOnly PaymentDate { get; set; }

        [Required]
        [Column("amount_received_inr", TypeName = "numeric(18,2)")]
        public decimal AmountReceivedInr { get; set; }

        [Column("tds_deducted_inr", TypeName = "numeric(18,2)")]
        public decimal TdsDeductedInr { get; set; } = 0;

        [Column("other_deduction_inr", TypeName = "numeric(18,2)")]
        public decimal OtherDeductionInr { get; set; } = 0;

        [Column("payment_mode")]
        public string? PaymentMode { get; set; }

        [Column("reference_number")]
        public string? ReferenceNumber { get; set; }

        [Column("bank_name")]
        public string? BankName { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("recorded_by_id")]
        public int? RecordedById { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}
