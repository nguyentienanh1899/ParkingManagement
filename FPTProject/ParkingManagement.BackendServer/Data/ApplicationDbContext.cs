using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.BackendServer.Data.Entities;

namespace ParkingManagement.BackendServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // configure primary key with multiple keys
            builder.Entity<User>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<Permission>().HasKey(c => new { c.RoleId, c.FunctionId, c.CommandId });
            builder.Entity<CommandInFunction>().HasKey(c => new { c.CommandId, c.FunctionId });

        }
        public DbSet<ActivityLog> ActivityLogs { set; get; }
        public DbSet<BookingOffice> BookingOffices { set; get; }
        public DbSet<Car> Cars { set; get; }
        public DbSet<Command> Commands { set; get; }
        public DbSet<CommandInFunction> CommandInFunctions { set; get; }
        public DbSet<Function> Functions { set; get; }
        public DbSet<ParkingLot> ParkingLots { set; get; }
        public DbSet<Permission> Permissions { set; get; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Trip> Trips { set; get; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Attachment> Attachments { set; get; }
    }
}
