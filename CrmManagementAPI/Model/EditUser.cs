namespace CrmManagementAPI.Model
{
    public class EditUser
    {
        public int Id { get; set; } 
        public string? EmployeeId { get; set; }
        public string? FullName { get; set; }
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string? ReportingManager { get; set; }
        public string? Region { get; set; }
        public string? GovernmentVertical { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Mobile { get; set; }
        public string? Role { get; set; }
       
    }
}
