using System;

namespace CrmManagementAPI.Model
{
    public class EditEscalationPolicy
    {
        public int Id { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public int AgingDays { get; set; }
        public string SeverityLevel { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
        public string EscalateToRole { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }
}
