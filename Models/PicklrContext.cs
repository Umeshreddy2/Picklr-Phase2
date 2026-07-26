using Microsoft.EntityFrameworkCore;

namespace Picklr.Models
{
    public class PicklrContext : DbContext
    {
        public PicklrContext(DbContextOptions<PicklrContext> options) : base(options) { }

        public DbSet<Club> Clubs { get; set; } = null!;
        public DbSet<PicklProgram> Programs { get; set; } = null!;
        public DbSet<AppUser> Users { get; set; } = null!;

        // Phase 2: saved/paid bookings created from the shopping cart.
        public DbSet<Reservation> Reservations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Phase 2: explicit relationship configuration (Club 1 -- * PicklProgram,
            // PicklProgram 1 -- * Reservation). EF Core's conventions would infer
            // these from the ClubID / ProgramID property names anyway, but we spell
            // them out with the Fluent API for clarity, same as Ch4's approach.
            modelBuilder.Entity<PicklProgram>()
                .HasOne(p => p.Club)
                .WithMany()
                .HasForeignKey(p => p.ClubID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Program)
                .WithMany()
                .HasForeignKey(r => r.ProgramID)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Clubs
            modelBuilder.Entity<Club>().HasData(
                new Club
                {
                    ClubID = 1,
                    Name = "Picklr Downtown",
                    Location = "123 Main St, Chicago, IL",
                    Description = "Our flagship downtown club with 10 indoor courts."
                },
                new Club
                {
                    ClubID = 2,
                    Name = "Picklr Northside",
                    Location = "456 Oak Ave, Evanston, IL",
                    Description = "A vibrant outdoor facility with 8 courts and a pro shop."
                },
                new Club
                {
                    ClubID = 3,
                    Name = "Picklr New York",
                    Location = "789 Broadway, New York, NY",
                    Description = "Our newest club, open year-round with 6 indoor courts."
                }
            );

            // Seed Programs (Phase 2: now tied to a Club, with AvailableDays)
            modelBuilder.Entity<PicklProgram>().HasData(
                new PicklProgram
                {
                    ProgramID = 1,
                    Name = "Beginner Open Play",
                    Description = "Drop-in open play for new players. No experience needed.",
                    Fee = 10.00m,
                    ClubID = 1, // Picklr Downtown
                    AvailableDays = "Monday,Wednesday,Friday"
                },
                new PicklProgram
                {
                    ProgramID = 2,
                    Name = "Intermediate Clinic",
                    Description = "Weekly skill-building clinic led by a certified coach.",
                    Fee = 25.00m,
                    ClubID = 1, // Picklr Downtown
                    AvailableDays = "Tuesday,Thursday"
                },
                new PicklProgram
                {
                    ProgramID = 3,
                    Name = "Advanced Tournament",
                    Description = "Competitive round-robin tournament for rated players.",
                    Fee = 40.00m,
                    ClubID = 2, // Picklr Northside
                    AvailableDays = "Saturday,Sunday"
                },
                new PicklProgram
                {
                    ProgramID = 4,
                    Name = "Picklr 101",
                    Description = "The program is designed for the beginners.",
                    Fee = 10.00m,
                    ClubID = 3, // Picklr New York
                    AvailableDays = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday"
                },
                new PicklProgram
                {
                    ProgramID = 5,
                    Name = "Picklr Social",
                    Description = "Casual weekend social play, all levels welcome.",
                    Fee = 0.00m,
                    ClubID = 2, // Picklr Northside
                    AvailableDays = "Saturday"
                }
            );

            // Seed Users
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    UserID = 1,
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice@picklr.com",
                    Role = "Admin"
                },
                new AppUser
                {
                    UserID = 2,
                    FirstName = "Bob",
                    LastName = "Jones",
                    Email = "bob@picklr.com",
                    Role = "Client"
                }
            );
        }
    }
}
