using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InternProject.Models;

public partial class DbInternProContext : DbContext
{
    public DbInternProContext()
    {
    }

    public DbInternProContext(DbContextOptions<DbInternProContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Connection> Connections { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => 
        optionsBuilder.UseSqlServer("Server=DESKTOP-ABCTORG\\SQLEXPRESS;Database=DbInternPro;Trusted_Connection=True;TrustServerCertificate=True;");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Connection>(entity =>
        {
            entity.HasKey(e => e.DbId);

            entity.ToTable("connection");

            entity.Property(e => e.DbId).HasColumnName("dbID");
            entity.Property(e => e.DbLocation)
                .HasMaxLength(50)
                .HasColumnName("dbLocation");
            entity.Property(e => e.DbName)
                .HasMaxLength(200)
                .HasColumnName("dbName");
            entity.Property(e => e.DbPassword)
                .HasMaxLength(100)
                .HasColumnName("dbPassword");
            entity.Property(e => e.DbSec).HasColumnName("dbSec");
            entity.Property(e => e.DbStatus).HasColumnName("dbStatus");
            entity.Property(e => e.DbUserName)
                .HasMaxLength(100)
                .HasColumnName("dbUserName");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .HasColumnName("password");
            entity.Property(e => e.UserStatus).HasColumnName("userStatus");
            entity.Property(e => e.Username)
                .HasMaxLength(300)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
