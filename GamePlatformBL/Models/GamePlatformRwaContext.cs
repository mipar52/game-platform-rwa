﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GamePlatformBL.Models;

public partial class GamePlatformRwaContext : DbContext
{
    public GamePlatformRwaContext()
    {
    }

    public GamePlatformRwaContext(DbContextOptions<GamePlatformRwaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameGenre> GameGenres { get; set; }

    public virtual DbSet<GameType> GameTypes { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<LogEntry> LogEntries { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=.;Database=GamePlatformRWA;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Game__3214EC0752704E17");

            entity.ToTable("Game");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.GameUrl).HasMaxLength(255);
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.WonGameOfTheYear).HasDefaultValue(false);

            entity.HasOne(d => d.GameType).WithMany(p => p.Games)
                .HasForeignKey(d => d.GameTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Game__GameTypeId__534D60F1");
        });

        modelBuilder.Entity<GameGenre>(entity =>
        {
            entity.HasKey(e => new { e.GameId, e.GenreId }).HasName("PK__GameGenr__DA80C7AA23477FE3");

            entity.ToTable("GameGenre");

            entity.Property(e => e.AddedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Game).WithMany(p => p.GameGenres)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__GameGenre__GameI__5629CD9C");

            entity.HasOne(d => d.Genre).WithMany(p => p.GameGenres)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK__GameGenre__Genre__571DF1D5");
        });

        modelBuilder.Entity<GameType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GameType__3214EC077F7239B7");

            entity.ToTable("GameType");

            entity.HasIndex(e => e.Name, "UQ__GameType__737584F66EA0792F").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genre__3214EC0710282574");

            entity.ToTable("Genre");

            entity.HasIndex(e => e.Name, "UQ__Genre__737584F6FAD1EE1F").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<LogEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LogEntry__3214EC07D75FB14F");

            entity.ToTable("LogEntry");

            entity.Property(e => e.Level).HasMaxLength(20);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Review_Id");

            entity.ToTable("Review");

            entity.Property(e => e.Approved).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReviewText).HasMaxLength(2000);

            entity.HasOne(d => d.Game).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__Review__GameId__5DCAEF64");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Review__UserId__5CD6CB2B");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07C5CFBEC7");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F6C18EEB33").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0772760B55");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E4205498AA").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534FA879BAA").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(256);
            entity.Property(e => e.LastName).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(256);
            entity.Property(e => e.PwdHash).HasMaxLength(256);
            entity.Property(e => e.PwdSalt).HasMaxLength(256);
            entity.Property(e => e.RoleId).HasDefaultValue(2);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
