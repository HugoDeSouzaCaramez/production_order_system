using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Models;
using ProductionOrderApi.Enums;

namespace ProductionOrderApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ProductionOrder> ProductionOrder { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Resource> Resource { get; set; }
        public DbSet<ProductionLog> ProductionLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductionOrder>()
                .Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Resource>()
                .Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<ProductionOrder>()
                .HasCheckConstraint("CHK_ProductionOrder_Status",
                    "Status IN ('Planejada', 'EmProducao', 'Finalizada')");

            modelBuilder.Entity<Resource>()
                .HasCheckConstraint("CHK_Resource_Status",
                    "Status IN ('Disponivel', 'EmUso', 'Parado')");

            modelBuilder.Entity<ProductionOrder>()
                .HasOne(po => po.Product)
                .WithMany(p => p.ProductionOrders)
                .HasForeignKey(po => po.ProductCode)
                .HasPrincipalKey(p => p.Code)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductionLog>()
                .HasOne(pl => pl.ProductionOrder)
                .WithMany(po => po.ProductionLogs)
                .HasForeignKey(pl => pl.ProductionOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductionLog>()
                .HasOne(pl => pl.Resource)
                .WithMany(r => r.ProductionLogs)
                .HasForeignKey(pl => pl.ResourceId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<Resource>()
                .HasIndex(r => r.Code)
                .IsUnique();

            modelBuilder.Entity<ProductionOrder>()
                .HasCheckConstraint("CHK_QuantityProduced", "QuantityProduced <= QuantityPlanned");

            modelBuilder.Entity<ProductionOrder>()
                .HasCheckConstraint("CHK_EndDate", "EndDate IS NULL OR EndDate > StartDate");
        }
    }
}
