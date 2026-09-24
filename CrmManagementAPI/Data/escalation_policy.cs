using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class escalation_policy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("policy_name")]
        public string PolicyName { get; set; } = string.Empty;

        [Required]
        [Column("aging_days")]
        public int AgingDays { get; set; }

        [Required]
        [Column("severity_level")]
        public string SeverityLevel { get; set; } = string.Empty;

        [Required]
        [Column("channel")]
        public string Channel { get; set; } = string.Empty;

        [Required]
        [Column("template_code")]
        public string TemplateCode { get; set; } = string.Empty;

        [Required]
        [Column("escalate_to_role")]
        public string EscalateToRole { get; set; } = string.Empty;

        [Column("active")]
        public bool Active { get; set; } = true;
    }
}
