using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GamesLibAPI.Models;

namespace GamesLibAPI.Context;

public partial class GtcymkznContext : DbContext
{
    public GtcymkznContext()
    {
    }

    public GtcymkznContext(DbContextOptions<GtcymkznContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameGenre> GameGenres { get; set; }

    public virtual DbSet<Genere> Generes { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserJwt> UserJwts { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=8567;Database=gtcymkzn");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("developer_pkey");

            entity.ToTable("developer");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Developer1).HasColumnName("developer");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("favorite_pk");

            entity.ToTable("favorite");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.IdGame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorite_fk");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorite_fk_1");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("games_pkey");

            entity.ToTable("games");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GameName).HasColumnName("game_name");
            entity.Property(e => e.IdDeveloper).HasColumnName("id_developer");
            entity.Property(e => e.IdPublisher).HasColumnName("id_publisher");
            entity.Property(e => e.MainImage)
                .HasMaxLength(255)
                .HasColumnName("main_image");
            entity.Property(e => e.ReleaseDate)
                .HasMaxLength(100)
                .HasColumnName("release_date");
            entity.Property(e => e.SystemRequestMin).HasColumnName("system_request_min");
            entity.Property(e => e.SystemRequestRec).HasColumnName("system_request_rec");

            entity.HasOne(d => d.IdDeveloperNavigation).WithMany(p => p.Games)
                .HasForeignKey(d => d.IdDeveloper)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("games_id_developer_fkey");

            entity.HasOne(d => d.IdPublisherNavigation).WithMany(p => p.Games)
                .HasForeignKey(d => d.IdPublisher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("games_id_publisher_fkey");
        });

        modelBuilder.Entity<GameGenre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_genres_pk");

            entity.ToTable("game_genres");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.IdGenre).HasColumnName("id_genre");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.GameGenres)
                .HasForeignKey(d => d.IdGame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("game_genres_fk_1");

            entity.HasOne(d => d.IdGenreNavigation).WithMany(p => p.GameGenres)
                .HasForeignKey(d => d.IdGenre)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("game_genres_fk");
        });

        modelBuilder.Entity<Genere>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genere_pkey");

            entity.ToTable("genere");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Gener)
                .HasMaxLength(250)
                .HasColumnName("gener");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("publisher_pkey");

            entity.ToTable("publisher");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Publisher1).HasColumnName("publisher");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .HasColumnName("email");
            entity.Property(e => e.Login)
                .HasMaxLength(250)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .HasColumnName("password");
            entity.Property(e => e.UserRole).HasColumnName("user_role");

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_user_role_fkey");
        });

        modelBuilder.Entity<UserJwt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_jwt_pk");

            entity.ToTable("user_jwt");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JwtAccess)
                .HasColumnType("character varying")
                .HasColumnName("jwt_access");
            entity.Property(e => e.JwtRefresh)
                .HasColumnType("character varying")
                .HasColumnName("jwt_refresh");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.UserJwts)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_jwt_fk");
        });

        modelBuilder.Entity<Userrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userrole_pkey");

            entity.ToTable("userrole");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.RoleName)
                .HasColumnType("character varying")
                .HasColumnName("role_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
