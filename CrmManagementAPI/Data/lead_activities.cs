using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class lead_activities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("lead_id")]
        public int LeadId { get; set; }

        [Column("owner_id")]
        public int? OwnerId { get; set; }

        [Required]
        [Column("activity_type")]
        public string ActivityType { get; set; } = string.Empty;

        [Column("activity_date")]
        public DateTime ActivityDate { get; set; }

        [Column("from_stage")]
        public string? FromStage { get; set; }

        [Column("to_stage")]
        public string? ToStage { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("next_action")]
        public string? NextAction { get; set; }

        [Column("next_action_date")]
        public DateOnly? NextActionDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }

}
