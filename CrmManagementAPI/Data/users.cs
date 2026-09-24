using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("employee_id")]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Column("designation")]
        public string? Designation { get; set; }

        [Column("department")]
        public string? Department { get; set; }

        [Column("reporting_manager")]
        public string? ReportingManager { get; set; }

        [Column("region")]
        public string? Region { get; set; }

        [Column("government_vertical")]
        public string? GovernmentVertical { get; set; }

        [Required]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("mobile")]
        public string? Mobile { get; set; }

        [Required]
        [Column("role")]
        public string Role { get; set; } = string.Empty;

        [Column("active")]
        public bool Active { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }

}
