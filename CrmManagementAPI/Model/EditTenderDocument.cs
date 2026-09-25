using System;

namespace CrmManagementAPI.Model
{
    public class EditTenderDocument
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentUrl { get; set; } = string.Empty;
        public int? UploadedById { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? Notes { get; set; }
    }
}
