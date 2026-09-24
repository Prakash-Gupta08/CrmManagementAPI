using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class purchase_orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("po_number")]
        public string PoNumber { get; set; } = string.Empty;

        [Required]
        [Column("po_date")]
        public DateOnly PoDate { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("tender_id")]
        public int? TenderId { get; set; }

        [Required]
        [Column("project_name")]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        [Column("po_value_inr", TypeName = "numeric(18,2)")]
        public decimal PoValueInr { get; set; }

        [Column("gst_pct", TypeName = "numeric(5,2)")]
        public decimal? GstPct { get; set; }

        [Column("gst_amount_inr", TypeName = "numeric(18,2)")]
        public decimal? GstAmountInr { get; set; }

        [Column("total_contract_value_inr", TypeName = "numeric(18,2)")]
        public decimal? TotalContractValueInr { get; set; }

        [Column("start_date")]
        public DateOnly? StartDate { get; set; }

        [Column("completion_date")]
        public DateOnly? CompletionDate { get; set; }

        [Column("delivery_schedule")]
        public string? DeliverySchedule { get; set; }

        [Column("payment_terms")]
        public string? PaymentTerms { get; set; }

        [Column("warranty_months")]
        public int? WarrantyMonths { get; set; }

        [Column("amc_years")]
        public int? AmcYears { get; set; }

        [Column("sla_terms")]
        public string? SlaTerms { get; set; }

        [Column("ld_penalty_terms")]
        public string? LdPenaltyTerms { get; set; }

        [Column("pbg_amount_inr", TypeName = "numeric(18,2)")]
        public decimal? PbgAmountInr { get; set; }

        [Column("pbg_validity_date")]
        public DateOnly? PbgValidityDate { get; set; }

        [Column("performance_obligations")]
        public string? PerformanceObligations { get; set; }

        [Column("po_status")]
        public string PoStatus { get; set; } = "Draft";

        [Column("account_owner_id")]
        public int? AccountOwnerId { get; set; }

        [Column("project_manager_id")]
        public int? ProjectManagerId { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
