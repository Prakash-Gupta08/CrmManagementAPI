using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class customers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("organization_name")]
        public string OrganizationName { get; set; } = string.Empty;

        [Column("ministry_parent")]
        public string? MinistryParent { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("state")]
        public string? State { get; set; }

        [Column("district_city")]
        public string? DistrictCity { get; set; }

        [Column("office_address")]
        public string? OfficeAddress { get; set; }

        [Column("website")]
        public string? Website { get; set; }

        [Column("gem_seller_id")]
        public string? GemSellerId { get; set; }

        [Column("gstin")]
        public string? Gstin { get; set; }

        [Column("account_owner")]
        public string? AccountOwner { get; set; }

        [Column("key_contact_name")]
        public string? KeyContactName { get; set; }

        [Column("key_contact_designation")]
        public string? KeyContactDesignation { get; set; }

        [Column("key_contact_email")]
        public string? KeyContactEmail { get; set; }

        [Column("key_contact_mobile")]
        public string? KeyContactMobile { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }

}
