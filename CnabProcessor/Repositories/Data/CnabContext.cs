using CnabProcessor.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace CnabProcessor.Repositories.Data;

public class CnabContext : DbContext
{
    public CnabContext(DbContextOptions<CnabContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Store> Stores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Cpf).HasMaxLength(11);
            entity.Property(e => e.Card).HasMaxLength(12);
            entity.Property(e => e.StoreOwner).HasMaxLength(14);
            entity.Property(e => e.StoreName).HasMaxLength(19);

            entity.HasOne(e => e.Store)
                  .WithMany(s => s.Transactions)
                  .HasForeignKey(e => e.StoreId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Owner).HasMaxLength(14);
            entity.Property(e => e.Name).HasMaxLength(19);

            entity.HasIndex(e => new { e.Owner, e.Name }).IsUnique();
        });
    }
}
