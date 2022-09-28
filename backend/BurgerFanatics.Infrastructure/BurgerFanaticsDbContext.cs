using BurgerFanatics.Domain.Domain.Models;

using Microsoft.EntityFrameworkCore;

namespace BurgerFanatics.Infrastructure;

public class BurgerFanaticsDbContext : DbContext
{
    public BurgerFanaticsDbContext()
    {
    }

    public BurgerFanaticsDbContext(DbContextOptions<BurgerFanaticsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; } = null!;
    public virtual DbSet<FileAttachment> FileAttachments { get; set; } = null!;
    public virtual DbSet<Restaurant> Restaurants { get; set; } = null!;
    public virtual DbSet<Review> Reviews { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<WorkCalendar> WorkCalendars { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("address");

            entity.Property(e => e.AddressId)
                .HasColumnName("address_id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Location)
                .HasColumnType("geography(Point,4326)")
                .HasColumnName("location");

            entity.Property(e => e.Text).HasColumnName("text");
        });

        modelBuilder.Entity<FileAttachment>(entity =>
        {
            entity.ToTable("file_attachment");

            entity.Property(e => e.FileAttachmentId)
                .HasColumnName("file_attachment_id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.FileName).HasColumnName("file_name");

            entity.Property(e => e.Path).HasColumnName("path");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");

            entity.HasOne(d => d.Review)
                .WithMany(p => p.FileAttachments)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_file_attachment_review");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.ToTable("restaurant");

            entity.Property(e => e.RestaurantId)
                .HasColumnName("restaurant_id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.AddressId).HasColumnName("address_id");

            entity.Property(e => e.AdministratorId).HasColumnName("administrator_id");

            entity.Property(e => e.CreatedUtc)
                .HasColumnName("created_utc")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.Name).HasColumnName("name");

            entity.Property(e => e.UpdatedUtc)
                .HasColumnName("updated_utc")
                .HasDefaultValueSql("now()");

            entity.HasOne(d => d.Address)
                .WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_restaurant_address");

            entity.HasOne(d => d.Administrator)
                .WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.AdministratorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_restaurant_user");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("review");

            entity.Property(e => e.ReviewId)
                .HasColumnName("review_id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.CreatedUtc)
                .HasColumnName("created_utc")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.RatingTaste).HasColumnName("rating_taste");

            entity.Property(e => e.RatingTexture).HasColumnName("rating_texture");

            entity.Property(e => e.RatingVisual).HasColumnName("rating_visual");

            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

            entity.Property(e => e.UpdatedUtc)
                .HasColumnName("updated_utc")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Restaurant)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("fk_review_restaurant");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_review_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Username).HasColumnName("username");
        });

        modelBuilder.Entity<WorkCalendar>(entity =>
        {
            entity.HasKey(e => new { e.RestaurantId, e.WeekDay, e.OpeningHour, e.ClosingHour })
                .HasName("work_calendar_pkey");

            entity.ToTable("work_calendar");

            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");

            entity.Property(e => e.WeekDay).HasColumnName("week_day");

            entity.Property(e => e.OpeningHour)
                .HasColumnType("time(0) without time zone")
                .HasColumnName("opening_hour");

            entity.Property(e => e.ClosingHour)
                .HasColumnType("time(0) without time zone")
                .HasColumnName("closing_hour");

            entity.HasOne(d => d.Restaurant)
                .WithMany(p => p.WorkCalendars)
                .HasForeignKey(d => d.RestaurantId)
                .HasConstraintName("fk_work_calendar_restaurant");
        });
    }
}