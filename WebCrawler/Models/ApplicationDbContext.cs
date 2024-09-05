using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions;

namespace WebCrawler.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<WebsiteRecordModel> WebsiteRecords { get; set; }
        public DbSet<NodeModel> NodeRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebsiteRecordModel>().ToTable("WebsiteRecord");
            modelBuilder.Entity<NodeModel>().ToTable("Node");

            modelBuilder.Entity<NodeModel>().HasOne(n => n.WebsiteRecord).WithMany(w => w.Nodes).HasForeignKey(n => n.WebsiteRecordId).OnDelete(DeleteBehavior.Cascade); ;
        }
    }
}
