using System;

namespace CrmManagementAPI.Model
{
    public class EditTender
    {
        public int Id { get; set; }
        public string TenderNumber { get; set; } = string.Empty;
        public string TenderTitle { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string? TenderPortal { get; set; }
        public string? GemBidNumber { get; set; }
        public DateOnly? PublicationDate { get; set; }
        public DateOnly? PreBidMeetingDate { get; set; }
        public DateOnly? QuerySubmissionDeadline { get; set; }
        public DateTimeOffset? BidSubmissionDeadline { get; set; }
        public DateOnly? TechnicalOpeningDate { get; set; }
        public DateOnly? FinancialOpeningDate { get; set; }
        public decimal? EstimatedTenderValueInr { get; set; }
        public decimal? EmdAmountInr { get; set; }
        public decimal? TenderFeeInr { get; set; }
        public decimal? PbgRequiredPct { get; set; }
        public string? EligibilityCriteria { get; set; }
        public string? TechnicalRequirements { get; set; }
        public string? OemRequirement { get; set; }
        public bool ConsortiumRequirement { get; set; } = false;
        public string? MiiClass { get; set; }
        public string? ExperienceCriteria { get; set; }
        public decimal? TurnoverCriteriaInr { get; set; }
        public int? BidOwnerId { get; set; }
        public int? PresalesOwnerId { get; set; }
        public int? CommercialOwnerId { get; set; }
        public string? PartnerOem { get; set; }
        public string? Competitors { get; set; }
        public string BidStatus { get; set; } = "Identified";
        public int? WinProbabilityPct { get; set; }
        public decimal? BidValueSubmittedInr { get; set; }
        public string? L1Bidder { get; set; }
        public decimal? L1ValueInr { get; set; }
        public string? OutcomeNotes { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
