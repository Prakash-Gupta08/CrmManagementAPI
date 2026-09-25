using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class leads
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("lead_code")]
        public string LeadCode { get; set; } = string.Empty;

        [Required]
        [Column("organization_name")]
        public string OrganizationName { get; set; } = string.Empty;

        [Column("ministry_parent")]
        public string? MinistryParent { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("state")]
        public string? State { get; set; }

        [Column("district_city")]
        public string? DistrictCity { get; set; }

        [Column("contact_name")]
        public string? ContactName { get; set; }

        [Column("contact_designation")]
        public string? ContactDesignation { get; set; }

        [Column("contact_email")]
        public string? ContactEmail { get; set; }

        [Column("contact_mobile")]
        public string? ContactMobile { get; set; }

        [Column("source")]
        public string? Source { get; set; }

        [Required]
        [Column("opportunity_title")]
        public string OpportunityTitle { get; set; } = string.Empty;

        [Column("opportunity_summary")]
        public string? OpportunitySummary { get; set; }

        [Column("estimated_value_inr", TypeName = "numeric(18,2)")]
        public decimal? EstimatedValueInr { get; set; }

        [Column("expected_rfp_date")]
        public DateOnly? ExpectedRfpDate { get; set; }

        [Column("fiscal_year")]
        public string? FiscalYear { get; set; }

        [Column("budget_line_item")]
        public string? BudgetLineItem { get; set; }

        [Column("budget_confirmed")]
        public bool? BudgetConfirmed { get; set; }

        [Column("decision_maker")]
        public string? DecisionMaker { get; set; }

        [Column("competition_status")]
        public string? CompetitionStatus { get; set; }

        [Column("fit_notes")]
        public string? FitNotes { get; set; }

        [Column("lead_stage")]
        public string LeadStage { get; set; } = "New";

        [Column("disqualification_reason")]
        public string? DisqualificationReason { get; set; }

        [Column("qualification_score")]
        public int? QualificationScore { get; set; }

        [Column("bant_budget")]
        public string? BantBudget { get; set; }

        [Column("bant_authority")]
        public string? BantAuthority { get; set; }

        [Column("bant_need")]
        public string? BantNeed { get; set; }

        [Column("bant_timeline")]
        public string? BantTimeline { get; set; }

        [Column("next_action")]
        public string? NextAction { get; set; }

        [Column("next_action_date")]
        public DateOnly? NextActionDate { get; set; }

        [Column("owner_id")]
        public int? OwnerId { get; set; }

        [Column("converted_tender_id")]
        public int? ConvertedTenderId { get; set; }

        [Column("converted_customer_id")]
        public int? ConvertedCustomerId { get; set; }

        [Column("converted_at")]
        public DateTime? ConvertedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

}
