using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class tender_activities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("tender_id")]
        public int TenderId { get; set; }

        [Column("owner_id")]
        public int? OwnerId { get; set; }

        [Required]
        [Column("activity_type")]
        public string ActivityType { get; set; } = string.Empty;

        [Column("activity_date")]
        public DateTime ActivityDate { get; set; }

        [Column("outcome")]
        public string? Outcome { get; set; }

        [Column("next_action")]
        public string? NextAction { get; set; }

        [Column("next_action_date")]
        public DateOnly? NextActionDate { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
