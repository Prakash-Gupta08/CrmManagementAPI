using System;

namespace CrmManagementAPI.Model
{
    public class EditPayment
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public DateOnly PaymentDate { get; set; }
        public decimal AmountReceivedInr { get; set; }
        public decimal TdsDeductedInr { get; set; } = 0;
        public decimal OtherDeductionInr { get; set; } = 0;
        public string? PaymentMode { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? BankName { get; set; }
        public string? Notes { get; set; }
        public int? RecordedById { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
