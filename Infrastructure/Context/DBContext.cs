using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Context;

public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options) { }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>().ToTable("rooms");
        modelBuilder.Entity<Company>().ToTable("companies");

        modelBuilder.Entity<Workspace>(e =>
        {
            e.ToTable("workspaces");
            e.HasOne(w => w.Room)
             .WithMany(r => r.Workspaces)
             .HasForeignKey(w => w.RoomId);
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.ToTable("bookings");
            e.HasOne(b => b.Workspace)
             .WithMany(w => w.Bookings)
             .HasForeignKey(b => b.WorkspaceId);

            e.HasOne(b => b.Company)
             .WithMany(c => c.Bookings)
             .HasForeignKey(b => b.CompanyId);
        });

        base.OnModelCreating(modelBuilder);
    }
}