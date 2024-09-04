using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions;
using WebCrawler.Views;

namespace WebCrawler.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<WebsiteRecordModel> WebsiteRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebsiteRecordModel>().ToTable("WebsiteRecord");
            modelBuilder.Entity<NodeModel>().ToTable("Node");
        }
    }
}
