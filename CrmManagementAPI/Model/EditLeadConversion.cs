using System;

namespace CrmManagementAPI.Model
{
    public class EditLeadConversion
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public int TenderId { get; set; }
        public int CustomerId { get; set; }
        public int ConvertedById { get; set; }
        public string? ConversionNotes { get; set; }
        public DateTime ConvertedAt { get; set; }
    }
}
