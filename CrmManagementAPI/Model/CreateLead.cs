using System;

namespace CrmManagementAPI.Model
{
    public class CreateLead
    {
        public string LeadCode { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string? MinistryParent { get; set; }
        public string? Category { get; set; }
        public string? State { get; set; }
        public string? DistrictCity { get; set; }
        public string? ContactName { get; set; }
        public string? ContactDesignation { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactMobile { get; set; }
        public string? Source { get; set; }
        public string OpportunityTitle { get; set; } = string.Empty;
        public string? OpportunitySummary { get; set; }
        public decimal? EstimatedValueInr { get; set; }
        public DateOnly? ExpectedRfpDate { get; set; }
        public string? FiscalYear { get; set; }
        public string? BudgetLineItem { get; set; }
        public bool? BudgetConfirmed { get; set; }
        public string? DecisionMaker { get; set; }
        public string? CompetitionStatus { get; set; }
        public string? FitNotes { get; set; }
        public string LeadStage { get; set; } = "New";
        public string? DisqualificationReason { get; set; }
        public int? QualificationScore { get; set; }
        public string? BantBudget { get; set; }
        public string? BantAuthority { get; set; }
        public string? BantNeed { get; set; }
        public string? BantTimeline { get; set; }
        public string? NextAction { get; set; }
        public DateOnly? NextActionDate { get; set; }
        public int? OwnerId { get; set; }
        public int? ConvertedTenderId { get; set; }
        public int? ConvertedCustomerId { get; set; }
        public DateTimeOffset? ConvertedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
