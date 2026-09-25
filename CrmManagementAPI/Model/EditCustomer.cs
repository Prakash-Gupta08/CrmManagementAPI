namespace CrmManagementAPI.Model
{
    public class EditCustomer
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string? MinistryParent { get; set; }
        public string? Category { get; set; }
        public string? State { get; set; }
        public string? DistrictCity { get; set; }
        public string? OfficeAddress { get; set; }
        public string? Website { get; set; }
        public string? GemSellerId { get; set; }
        public string? Gstin { get; set; }
        public string? AccountOwner { get; set; }
        public string? KeyContactName { get; set; }
        public string? KeyContactDesignation { get; set; }
        public string? KeyContactEmail { get; set; }
        public string? KeyContactMobile { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
