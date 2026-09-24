using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class message_templates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("template_code")]
        public string TemplateCode { get; set; } = string.Empty;

        [Required]
        [Column("template_name")]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        [Column("channel")]
        public string Channel { get; set; } = string.Empty;

        [Required]
        [Column("use_case")]
        public string UseCase { get; set; } = string.Empty;

        [Column("severity_level")]
        public string? SeverityLevel { get; set; }

        [Column("aging_days_min")]
        public int? AgingDaysMin { get; set; }

        [Column("aging_days_max")]
        public int? AgingDaysMax { get; set; }

        [Column("lead_time_days")]
        public int? LeadTimeDays { get; set; }

        [Column("subject")]
        public string? Subject { get; set; }

        [Required]
        [Column("body_text")]
        public string BodyText { get; set; } = string.Empty;

        [Column("variables")]
        public string? Variables { get; set; }

        [Column("active")]
        public bool Active { get; set; } = true;

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

    }
}
