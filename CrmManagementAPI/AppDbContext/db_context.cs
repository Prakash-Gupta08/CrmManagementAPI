using CrmManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CrmManagementAPI.AppDbContext
{
    public class db_context : DbContext
    {
        public db_context(DbContextOptions<db_context> options)
            : base(options)
        {
        }

        public DbSet<users> users { get; set; }
        public DbSet<customers> customers { get; set; }
        public DbSet<leads> leads { get; set; }
        public DbSet<tenders> tenders { get; set; }
        public DbSet<tender_documents> tender_documents { get; set; }
        public DbSet<tender_alerts> tender_alerts { get; set; }
        public DbSet<tender_activities> tender_activities { get; set; }
        public DbSet<go_nogo_approvals> go_nogo_approvals { get; set; }
        public DbSet<purchase_orders> purchase_orders { get; set; }
        public DbSet<project_milestones> project_milestones { get; set; }
        public DbSet<invoices> invoices { get; set; }
        public DbSet<payments> payments { get; set; }
        public DbSet<message_templates> message_templates { get; set; }
        public DbSet<message_outbox> message_outbox { get; set; }
        public DbSet<escalation_policy> escalation_policy { get; set; }
        public DbSet<lead_activities> lead_activities { get; set; }
        public DbSet<lead_conversions> lead_conversions { get; set; }
    }
}
