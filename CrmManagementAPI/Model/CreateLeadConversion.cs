using System;

namespace CrmManagementAPI.Model
{
    public class CreateLeadConversion
    {
        public int LeadId { get; set; }
        public int TenderId { get; set; }
        public int CustomerId { get; set; }
        public int ConvertedById { get; set; }
        public string? ConversionNotes { get; set; }
        public DateTimeOffset ConvertedAt { get; set; }
    }
}
