using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions;

namespace WebCrawler.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<WebsiteRecordModel> WebsiteRecords { get; set; }
        public DbSet<NodeModel> NodeRecords { get; set; }
        public DbSet<NodeNeighbourModel> NodeNeighbours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebsiteRecordModel>().ToTable("WebsiteRecord");
            modelBuilder.Entity<NodeModel>().ToTable("Node");
            modelBuilder.Entity<NodeNeighbourModel>().ToTable("NodeNeighbour");

            modelBuilder.Entity<NodeNeighbourModel>()
                           .HasKey(nn => new { nn.NodeId, nn.NeighbourNodeId });

            modelBuilder.Entity<NodeNeighbourModel>()
                .HasOne(nn => nn.Node)
                .WithMany(n => n.Neighbours)
                .HasForeignKey(nn => nn.NodeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NodeNeighbourModel>()
                .HasOne(nn => nn.NeighbourNode)
                .WithMany()
                .HasForeignKey(nn => nn.NeighbourNodeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
