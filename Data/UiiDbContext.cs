using Microsoft.EntityFrameworkCore;
using Wigo.Models.Entities;
using Wigo.Models.ValueObjects;

namespace Wigo.Data;

public class UiiDbContext : DbContext
{
    public UiiDbContext(DbContextOptions<UiiDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<BudgetCategory> BudgetCategories { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<EvidenceFile> EvidenceFiles { get; set; }
    public DbSet<ExpenseOcrResult> ExpenseOcrResults { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<TrackingToken> TrackingTokens { get; set; }
    public DbSet<ExpenseItem> ExpenseItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasColumnType("varchar(256)").IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).HasColumnType("varchar(512)");
            entity.Property(e => e.Role).HasColumnType("smallint");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.Users)
                .HasForeignKey(e => e.OrganizationId);
        });

        // Organization configuration
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.Property(e => e.Name).HasColumnType("varchar(200)").IsRequired();
            entity.Property(e => e.Code).HasColumnType("varchar(50)").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
        });

        // Project configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.Name).HasColumnType("varchar(200)").IsRequired();
            entity.Property(e => e.Status).HasColumnType("smallint");
            entity.Property(e => e.PublicEnabled).HasDefaultValue(false);
            entity.Property(e => e.SubmissionsOpen).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.Projects)
                .HasForeignKey(e => e.OrganizationId);

            entity.HasMany(e => e.Expenses)
                .WithOne(p => p.Project)
                .HasForeignKey(e => e.ProjectId);

            // Configure Money properties as owned
            entity.OwnsOne(e => e.Budget, ba =>
            {
                ba.Property(b => b.Amount).HasColumnType("decimal(18,2)");
                ba.Property(b => b.Currency).HasColumnType("varchar(10)");
            });

            entity.OwnsOne(e => e.BaseSpent, ba =>
            {
                ba.Property(b => b.Amount).HasColumnType("decimal(18,2)");
                ba.Property(b => b.Currency).HasColumnType("varchar(10)");
            });
        });

        // BudgetCategory configuration
        modelBuilder.Entity<BudgetCategory>(entity =>
        {
            entity.Property(e => e.Name).HasColumnType("varchar(200)").IsRequired();
            entity.Property(e => e.Color).HasColumnType("varchar(20)");
            entity.Property(e => e.SortOrder).HasColumnType("int");

            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId);

            // Configure Money properties as owned
            entity.OwnsOne(e => e.TotalAllocated, ba =>
            {
                ba.Property(b => b.Amount).HasColumnType("decimal(18,2)");
                ba.Property(b => b.Currency).HasColumnType("varchar(10)");
            });

            entity.OwnsOne(e => e.Spent, ba =>
            {
                ba.Property(b => b.Amount).HasColumnType("decimal(18,2)");
                ba.Property(b => b.Currency).HasColumnType("varchar(10)");
            });
        });

        // Expense configuration
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.SubmittedBy).HasColumnType("varchar(200)").IsRequired();
            entity.Property(e => e.ReviewNotes).HasColumnType("varchar(500)");
            entity.Property(e => e.AiExtractedFields).HasColumnType("jsonb");
            entity.Property(e => e.AiConfidence).HasColumnType("double precision");

            entity.HasOne(e => e.Project)
                .WithMany(p => p.Expenses)
                .HasForeignKey(e => e.ProjectId);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId);

            entity.HasOne(e => e.TrackingToken)
                .WithMany()
                .HasForeignKey(e => e.TrackingTokenId);

            entity.Property(e => e.Status).HasColumnType("smallint");

            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TrackingTokenId);

            // Configure Money Amount as owned
            entity.OwnsOne(e => e.Amount, ba =>
            {
                ba.Property(b => b.Amount).HasColumnType("decimal(18,2)");
                ba.Property(b => b.Currency).HasColumnType("varchar(10)");
            });

            // Configure new fields
            entity.Property(e => e.Title).HasColumnType("varchar(200)");
            entity.Property(e => e.Payer).HasColumnType("varchar(200)");
            entity.Property(e => e.Time).HasColumnType("varchar(100)");
            entity.Property(e => e.FundingSource).HasColumnType("varchar(200)");
            entity.Property(e => e.InvoiceNumber).HasColumnType("varchar(100)");
            entity.Property(e => e.Risk).HasColumnType("smallint");
            entity.Property(e => e.Vendor).HasColumnType("varchar(200)");
            entity.Property(e => e.Date).HasColumnType("varchar(20)");
            entity.Property(e => e.IsFresh).HasColumnType("boolean");
        });

        // ExpenseItem configuration
        modelBuilder.Entity<ExpenseItem>(entity =>
        {
            entity.ToTable("ExpenseItems");

            entity.HasOne(e => e.Expense)
                .WithMany(exp => exp.Items)
                .HasForeignKey(e => e.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.OwnsOne(e => e.UnitPrice, ba =>
            {
                ba.Property(b => b.Amount).HasColumnType("decimal(18,2)");
                ba.Property(b => b.Currency).HasColumnType("varchar(10)");
            });
        });

        // EvidenceFile configuration
        modelBuilder.Entity<EvidenceFile>(entity =>
        {
            entity.Property(e => e.BlobUrl).HasColumnType("varchar(500)").IsRequired();
            entity.Property(e => e.FileName).HasColumnType("varchar(200)").IsRequired();
            entity.Property(e => e.MimeType).HasColumnType("varchar(100)").IsRequired();
            entity.Property(e => e.RawText).HasColumnType("jsonb");
            entity.Property(e => e.ExtractedFields).HasColumnType("jsonb");

            entity.HasIndex(e => e.ExpenseId);

            entity.Property(e => e.FileSize).HasColumnType("bigint");
        });

        // ExpenseOcrResult configuration
        modelBuilder.Entity<ExpenseOcrResult>(entity =>
        {
            entity.Property(e => e.RawText).HasColumnType("jsonb");
            entity.Property(e => e.ExtractedFields).HasColumnType("jsonb");
            entity.Property(e => e.Provider).HasColumnType("varchar(100)");

            entity.HasIndex(e => e.ExpenseId);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(e => e.EntityType).HasColumnType("varchar(100)").IsRequired();
            entity.Property(e => e.Action).HasColumnType("varchar(100)").IsRequired();
            entity.Property(e => e.OldValues).HasColumnType("jsonb");
            entity.Property(e => e.NewValues).HasColumnType("jsonb");
            entity.Property(e => e.IpAddress).HasColumnType("varchar(200)");
            entity.Property(e => e.UserAgent).HasColumnType("varchar(500)");

            entity.HasIndex(e => new { e.EntityType, e.EntityId });
        });

        // TrackingToken configuration
        modelBuilder.Entity<TrackingToken>(entity =>
        {
            entity.Property(e => e.Value).HasColumnType("varchar(256)").IsRequired();
            entity.Property(e => e.Hash).HasColumnType("varchar(64)").IsRequired();

            entity.HasIndex(e => e.Hash).IsUnique();

            entity.Property(e => e.Expiry).HasColumnType("timestamp with time zone");
        });
    }
}