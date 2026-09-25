using System;

namespace CrmManagementAPI.Model
{
    public class CreateGoNoGoApproval
    {
        public int TenderId { get; set; }
        public int RequestedById { get; set; }
        public int ApproverId { get; set; }
        public DateOnly RequestDate { get; set; }
        public DateOnly? DecisionDate { get; set; }
        public decimal? TenderValueInr { get; set; }
        public string? EligibilityCompliance { get; set; }
        public string? TechnicalCompliance { get; set; }
        public string? OemAvailability { get; set; }
        public string? CompetitionAssessment { get; set; }
        public decimal? ExpectedMarginPct { get; set; }
        public string? PaymentTerms { get; set; }
        public string? ProjectRisk { get; set; }
        public string? ResourceAvailability { get; set; }
        public int? WinProbabilityPct { get; set; }
        public string? Decision { get; set; }
        public string? ApproverRemarks { get; set; }
        public string? Conditions { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
