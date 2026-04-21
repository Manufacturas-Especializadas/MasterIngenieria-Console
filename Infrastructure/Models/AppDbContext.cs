using System;
using System.Collections.Generic;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Master> Masters { get; set; }

    public virtual DbSet<MasterImprovement> MasterImprovements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Master>(entity =>
        {
            entity.ToTable("MasterIndustrial");

            entity.HasKey(e => e.Id).HasName("PK__Master__3214EC07953C8F2D");

            entity.Property(e => e.ChildPartNumber)
                .IsUnicode(false)
                .HasColumnName("childPartNumber");

            entity.Property(e => e.Client)
                .IsUnicode(false)
                .HasColumnName("client");

            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");

            entity.Property(e => e.Development)
                .IsUnicode(false)
                .HasColumnName("development");

            entity.Property(e => e.ExternalDiameter)
                .IsUnicode(false)
                .HasColumnName("externalDiameter");

            entity.Property(e => e.Family)
                .IsUnicode(false)
                .HasColumnName("family");

            entity.Property(e => e.Line).HasColumnName("line");

            entity.Property(e => e.MajorSetup)
                .IsUnicode(false)
                .HasColumnName("majorSetup");

            entity.Property(e => e.MinorSetup)
                .IsUnicode(false)
                .HasColumnName("minorSetup");

            entity.Property(e => e.Oper)
            .HasColumnType("decimal(10, 3)")
            .HasColumnName("oper");

            entity.Property(e => e.OperSetup)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("operSetup");

            entity.Property(e => e.TCiclo)
                .HasColumnType("decimal(10, 3)")
                .HasColumnName("tCiclo");

            entity.Property(e => e.Operation)
                .IsUnicode(false)
                .HasColumnName("operation");

            entity.Property(e => e.ParentPartNumber)
                .IsUnicode(false)
                .HasColumnName("parentPartNumber");

            entity.Property(e => e.PartOfPurchase)
                .IsUnicode(false)
                .HasColumnName("partOfPurchase");

            entity.Property(e => e.ProcessComments)
                .IsUnicode(false)
                .HasColumnName("processComments");

            entity.Property(e => e.PzsHr).HasColumnName("pzsHr");

            entity.Property(e => e.QuantityXquantity).HasColumnName("quantityXQuantity");

            entity.Property(e => e.Sequence).HasColumnName("sequence");

            entity.Property(e => e.Type)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.Property(e => e.Verification)
                .IsUnicode(false)
                .HasColumnName("verification");

            entity.Property(e => e.WallThickness)
                .IsUnicode(false)
                .HasColumnName("wallThickness");
        });

        modelBuilder.Entity<MasterImprovement>(entity =>
        {
            entity.ToTable("MasterImprovements");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OldCycleTime).HasColumnType("decimal(10, 3)");
            entity.Property(e => e.NewCycleTime).HasColumnType("decimal(10, 3)");
            entity.Property(e => e.ImprovementDate).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}