using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class project_milestones
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("po_id")]
        public int PoId { get; set; }

        [Required]
        [Column("milestone_name")]
        public string MilestoneName { get; set; } = string.Empty;

        [Column("milestone_type")]
        public string? MilestoneType { get; set; }

        [Column("planned_date")]
        public DateOnly? PlannedDate { get; set; }

        [Column("actual_date")]
        public DateOnly? ActualDate { get; set; }

        [Column("billing_pct", TypeName = "numeric(5,2)")]
        public decimal? BillingPct { get; set; }

        [Column("billing_amount_inr", TypeName = "numeric(18,2)")]
        public decimal? BillingAmountInr { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; } = "Planned";

        [Column("completion_notes")]
        public string? CompletionNotes { get; set; }

        [Column("billable")]
        public bool Billable { get; set; } = false;

        [Column("invoiced")]
        public bool Invoiced { get; set; } = false;

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
