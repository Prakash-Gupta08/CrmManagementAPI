using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class go_nogo_approvals
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("tender_id")]
        public int TenderId { get; set; }

        [Required]
        [Column("requested_by_id")]
        public int RequestedById { get; set; }

        [Required]
        [Column("approver_id")]
        public int ApproverId { get; set; }

        [Required]
        [Column("request_date")]
        public DateOnly RequestDate { get; set; }

        [Column("decision_date")]
        public DateOnly? DecisionDate { get; set; }

        [Column("tender_value_inr", TypeName = "numeric(18,2)")]
        public decimal? TenderValueInr { get; set; }

        [Column("eligibility_compliance")]
        public string? EligibilityCompliance { get; set; }

        [Column("technical_compliance")]
        public string? TechnicalCompliance { get; set; }

        [Column("oem_availability")]
        public string? OemAvailability { get; set; }

        [Column("competition_assessment")]
        public string? CompetitionAssessment { get; set; }

        [Column("expected_margin_pct", TypeName = "numeric(5,2)")]
        public decimal? ExpectedMarginPct { get; set; }

        [Column("payment_terms")]
        public string? PaymentTerms { get; set; }

        [Column("project_risk")]
        public string? ProjectRisk { get; set; }

        [Column("resource_availability")]
        public string? ResourceAvailability { get; set; }

        [Column("win_probability_pct")]
        public int? WinProbabilityPct { get; set; }

        [Column("decision")]
        public string? Decision { get; set; }

        [Column("approver_remarks")]
        public string? ApproverRemarks { get; set; }

        [Column("conditions")]
        public string? Conditions { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

    }
}
