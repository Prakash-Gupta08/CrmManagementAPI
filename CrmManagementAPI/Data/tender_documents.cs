using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class tender_documents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("tender_id")]
        public int TenderId { get; set; }

        [Required]
        [Column("document_type")]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [Column("document_name")]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        [Column("document_url")]
        public string DocumentUrl { get; set; } = string.Empty;

        [Column("uploaded_by_id")]
        public int? UploadedById { get; set; }

        [Column("uploaded_at")]
        public DateTime UploadedAt { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

    }
}
