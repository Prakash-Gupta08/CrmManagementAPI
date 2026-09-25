using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class tenders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("tender_number")]
        public string TenderNumber { get; set; } = string.Empty;

        [Required]
        [Column("tender_title")]
        public string TenderTitle { get; set; } = string.Empty;

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("tender_portal")]
        public string? TenderPortal { get; set; }

        [Column("gem_bid_number")]
        public string? GemBidNumber { get; set; }

        [Column("publication_date")]
        public DateOnly? PublicationDate { get; set; }

        [Column("pre_bid_meeting_date")]
        public DateOnly? PreBidMeetingDate { get; set; }

        [Column("query_submission_deadline")]
        public DateOnly? QuerySubmissionDeadline { get; set; }

        [Column("bid_submission_deadline")]
        public DateTime? BidSubmissionDeadline { get; set; }

        [Column("technical_opening_date")]
        public DateOnly? TechnicalOpeningDate { get; set; }

        [Column("financial_opening_date")]
        public DateOnly? FinancialOpeningDate { get; set; }

        [Column("estimated_tender_value_inr", TypeName = "numeric(18,2)")]
        public decimal? EstimatedTenderValueInr { get; set; }

        [Column("emd_amount_inr", TypeName = "numeric(18,2)")]
        public decimal? EmdAmountInr { get; set; }

        [Column("tender_fee_inr", TypeName = "numeric(18,2)")]
        public decimal? TenderFeeInr { get; set; }

        [Column("pbg_required_pct", TypeName = "numeric(5,2)")]
        public decimal? PbgRequiredPct { get; set; }

        [Column("eligibility_criteria")]
        public string? EligibilityCriteria { get; set; }

        [Column("technical_requirements")]
        public string? TechnicalRequirements { get; set; }

        [Column("oem_requirement")]
        public string? OemRequirement { get; set; }

        [Column("consortium_requirement")]
        public bool ConsortiumRequirement { get; set; } = false;

        [Column("mii_class")]
        public string? MiiClass { get; set; }

        [Column("experience_criteria")]
        public string? ExperienceCriteria { get; set; }

        [Column("turnover_criteria_inr", TypeName = "numeric(18,2)")]
        public decimal? TurnoverCriteriaInr { get; set; }

        [Column("bid_owner_id")]
        public int? BidOwnerId { get; set; }

        [Column("presales_owner_id")]
        public int? PresalesOwnerId { get; set; }

        [Column("commercial_owner_id")]
        public int? CommercialOwnerId { get; set; }

        [Column("partner_oem")]
        public string? PartnerOem { get; set; }

        [Column("competitors")]
        public string? Competitors { get; set; }

        [Column("bid_status")]
        public string BidStatus { get; set; } = "Identified";

        [Column("win_probability_pct")]
        public int? WinProbabilityPct { get; set; }

        [Column("bid_value_submitted_inr", TypeName = "numeric(18,2)")]
        public decimal? BidValueSubmittedInr { get; set; }

        [Column("l1_bidder")]
        public string? L1Bidder { get; set; }

        [Column("l1_value_inr", TypeName = "numeric(18,2)")]
        public decimal? L1ValueInr { get; set; }

        [Column("outcome_notes")]
        public string? OutcomeNotes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

}
