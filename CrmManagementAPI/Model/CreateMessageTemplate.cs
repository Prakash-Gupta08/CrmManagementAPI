using System;

namespace CrmManagementAPI.Model
{
    public class CreateMessageTemplate
    {
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string UseCase { get; set; } = string.Empty;
        public string? SeverityLevel { get; set; }
        public int? AgingDaysMin { get; set; }
        public int? AgingDaysMax { get; set; }
        public int? LeadTimeDays { get; set; }
        public string? Subject { get; set; }
        public string BodyText { get; set; } = string.Empty;
        public string? Variables { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
