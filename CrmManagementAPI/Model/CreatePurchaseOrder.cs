using System;

namespace CrmManagementAPI.Model
{
    public class CreatePurchaseOrder
    {
        public string PoNumber { get; set; } = string.Empty;
        public DateOnly PoDate { get; set; }
        public int CustomerId { get; set; }
        public int? TenderId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public decimal PoValueInr { get; set; }
        public decimal? GstPct { get; set; }
        public decimal? GstAmountInr { get; set; }
        public decimal? TotalContractValueInr { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? CompletionDate { get; set; }
        public string? DeliverySchedule { get; set; }
        public string? PaymentTerms { get; set; }
        public int? WarrantyMonths { get; set; }
        public int? AmcYears { get; set; }
        public string? SlaTerms { get; set; }
        public string? LdPenaltyTerms { get; set; }
        public decimal? PbgAmountInr { get; set; }
        public DateOnly? PbgValidityDate { get; set; }
        public string? PerformanceObligations { get; set; }
        public string PoStatus { get; set; } = "Draft";
        public int? AccountOwnerId { get; set; }
        public int? ProjectManagerId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
