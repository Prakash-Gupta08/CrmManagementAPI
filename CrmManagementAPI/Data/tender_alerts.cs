using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class tender_alerts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("tender_id")]
        public int TenderId { get; set; }

        [Required]
        [Column("alert_type")]
        public string AlertType { get; set; } = string.Empty;

        [Required]
        [Column("alert_date")]
        public DateOnly AlertDate { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; } = "Open";

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }

}
