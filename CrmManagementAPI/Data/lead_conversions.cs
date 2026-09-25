using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class lead_conversions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("lead_id")]
        public int LeadId { get; set; }

        [Required]
        [Column("tender_id")]
        public int TenderId { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [Column("converted_by_id")]
        public int ConvertedById { get; set; }

        [Column("conversion_notes")]
        public string? ConversionNotes { get; set; }

        [Column("converted_at")]
        public DateTime ConvertedAt { get; set; }
    }

}
