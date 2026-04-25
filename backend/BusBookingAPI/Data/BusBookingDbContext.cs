using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Models;
using Route = BusBookingAPI.Models.Route;

namespace BusBookingAPI.Data
{
    public class BusBookingDbContext : DbContext
    {
        public BusBookingDbContext(DbContextOptions<BusBookingDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<BusOperator> BusOperators { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BusAvailability> BusAvailabilities { get; set; }
        public DbSet<BusSchedule> BusSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .ToTable("users");
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .Property(u => u.Id).HasColumnName("id");
            modelBuilder.Entity<User>()
                .Property(u => u.FullName).HasColumnName("full_name");
            modelBuilder.Entity<User>()
                .Property(u => u.Email).HasColumnName("email");
            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber).HasColumnName("phone_number");
            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash).HasColumnName("password_hash");
            modelBuilder.Entity<User>()
                .Property(u => u.DateOfBirth).HasColumnName("date_of_birth");
            modelBuilder.Entity<User>()
                .Property(u => u.Address).HasColumnName("address");
            modelBuilder.Entity<User>()
                .Property(u => u.IsActive).HasColumnName("is_active");
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt).HasColumnName("updated_at");
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Country configuration
            modelBuilder.Entity<Country>()
                .ToTable("countries");
            modelBuilder.Entity<Country>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Country>()
                .Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<Country>()
                .Property(c => c.CountryName).HasColumnName("country_name");
            modelBuilder.Entity<Country>()
                .Property(c => c.CountryCode).HasColumnName("country_code");
            modelBuilder.Entity<Country>()
                .Property(c => c.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<Country>()
                .HasIndex(c => c.CountryName)
                .IsUnique();
            modelBuilder.Entity<Country>()
                .HasIndex(c => c.CountryCode)
                .IsUnique();

            // State configuration
            modelBuilder.Entity<State>()
                .ToTable("states");
            modelBuilder.Entity<State>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<State>()
                .Property(s => s.Id).HasColumnName("id");
            modelBuilder.Entity<State>()
                .Property(s => s.StateName).HasColumnName("state_name");
            modelBuilder.Entity<State>()
                .Property(s => s.CountryId).HasColumnName("country_id");
            modelBuilder.Entity<State>()
                .Property(s => s.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<State>()
                .HasOne(s => s.Country)
                .WithMany(c => c.States)
                .HasForeignKey(s => s.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<State>()
                .HasIndex(s => s.StateName)
                .IsUnique();

            // District configuration
            modelBuilder.Entity<District>()
                .ToTable("districts");
            modelBuilder.Entity<District>()
                .HasKey(d => d.Id);
            modelBuilder.Entity<District>()
                .Property(d => d.Id).HasColumnName("id");
            modelBuilder.Entity<District>()
                .Property(d => d.DistrictName).HasColumnName("district_name");
            modelBuilder.Entity<District>()
                .Property(d => d.StateId).HasColumnName("state_id");
            modelBuilder.Entity<District>()
                .Property(d => d.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<District>()
                .HasOne(d => d.State)
                .WithMany(s => s.Districts)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<District>()
                .HasIndex(d => new { d.DistrictName, d.StateId })
                .IsUnique();

            // Location configuration
            modelBuilder.Entity<Location>()
                .ToTable("locations");
            modelBuilder.Entity<Location>()
                .HasKey(l => l.Id);
            modelBuilder.Entity<Location>()
                .Property(l => l.Id).HasColumnName("id");
            modelBuilder.Entity<Location>()
                .Property(l => l.StreetAddress).HasColumnName("street_address");
            modelBuilder.Entity<Location>()
                .Property(l => l.DistrictId).HasColumnName("district_id");
            modelBuilder.Entity<Location>()
                .Property(l => l.City).HasColumnName("city");
            modelBuilder.Entity<Location>()
                .Property(l => l.StateId).HasColumnName("state_id");
            modelBuilder.Entity<Location>()
                .Property(l => l.CountryId).HasColumnName("country_id");
            modelBuilder.Entity<Location>()
                .Property(l => l.PostalCode).HasColumnName("postal_code");
            modelBuilder.Entity<Location>()
                .Property(l => l.Latitude).HasColumnName("latitude");
            modelBuilder.Entity<Location>()
                .Property(l => l.Longitude).HasColumnName("longitude");
            modelBuilder.Entity<Location>()
                .Property(l => l.OperatorId).HasColumnName("operator_id");
            modelBuilder.Entity<Location>()
                .Property(l => l.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<Location>()
                .Property(l => l.UpdatedAt).HasColumnName("updated_at");
            modelBuilder.Entity<Location>()
                .HasOne(l => l.District)
                .WithMany(d => d.Locations)
                .HasForeignKey(l => l.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Location>()
                .HasOne(l => l.State)
                .WithMany(s => s.Locations)
                .HasForeignKey(l => l.StateId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Location>()
                .HasOne(l => l.Country)
                .WithMany(c => c.Locations)
                .HasForeignKey(l => l.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Location>()
                .HasOne(l => l.Operator)
                .WithMany(bo => bo.Locations)
                .HasForeignKey(l => l.OperatorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Route configuration
            modelBuilder.Entity<Route>()
                .ToTable("routes");
            modelBuilder.Entity<Route>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<Route>()
                .Property(r => r.Id).HasColumnName("id");
            modelBuilder.Entity<Route>()
                .Property(r => r.SourceLocationId).HasColumnName("source_location_id");
            modelBuilder.Entity<Route>()
                .Property(r => r.DestinationLocationId).HasColumnName("destination_location_id");
            modelBuilder.Entity<Route>()
                .Property(r => r.DistanceKm).HasColumnName("distance_km");
            modelBuilder.Entity<Route>()
                .Property(r => r.EstimatedDurationHours).HasColumnName("estimated_duration_hours");
            modelBuilder.Entity<Route>()
                .Property(r => r.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<Route>()
                .Property(r => r.UpdatedAt).HasColumnName("updated_at");
            modelBuilder.Entity<Route>()
                .HasOne(r => r.SourceLocation)
                .WithMany(l => l.SourceRoutes)
                .HasForeignKey(r => r.SourceLocationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Route>()
                .HasOne(r => r.DestinationLocation)
                .WithMany(l => l.DestinationRoutes)
                .HasForeignKey(r => r.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // BusOperator configuration
            modelBuilder.Entity<BusOperator>()
                .ToTable("bus_operators");
            modelBuilder.Entity<BusOperator>()
                .HasKey(bo => bo.Id);
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.Id).HasColumnName("id");
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.OperatorName).HasColumnName("operator_name");
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.Email).HasColumnName("email");
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.PhoneNumber).HasColumnName("phone_number");
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.LicenseNumber).HasColumnName("license_number");
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.Address).HasColumnName("address");
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.IsActive).HasColumnName("is_active");
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.PasswordHash).HasColumnName("password_hash");
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<BusOperator>()
                .Property(bo => bo.UpdatedAt).HasColumnName("updated_at");
            modelBuilder.Entity<BusOperator>()
                .HasIndex(bo => bo.Email)
                .IsUnique();
            modelBuilder.Entity<BusOperator>()
                .HasIndex(bo => bo.LicenseNumber)
                .IsUnique();

            // Bus configuration
            modelBuilder.Entity<Bus>()
                .ToTable("buses");
            modelBuilder.Entity<Bus>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<Bus>()
                .Property(b => b.Id).HasColumnName("id");
            modelBuilder.Entity<Bus>()
                .Property(b => b.RegistrationNumber).HasColumnName("registration_number");
            modelBuilder.Entity<Bus>()
                .Property(b => b.OperatorId).HasColumnName("operator_id");
            modelBuilder.Entity<Bus>()
                .Property(b => b.RouteId).HasColumnName("route_id");
            modelBuilder.Entity<Bus>()
                .Property(b => b.SourceLocationId).HasColumnName("source_location_id");
            modelBuilder.Entity<Bus>()
                .Property(b => b.DestinationLocationId).HasColumnName("destination_location_id");
            modelBuilder.Entity<Bus>()
                .Property(b => b.SeatingCapacity).HasColumnName("seating_capacity");
            modelBuilder.Entity<Bus>()
                .Property(b => b.Price).HasColumnName("price");
            modelBuilder.Entity<Bus>()
                .Property(b => b.IsActive).HasColumnName("is_active");
            modelBuilder.Entity<Bus>()
                .Property(b => b.OperatingDays).HasColumnName("operating_days");
            modelBuilder.Entity<Bus>()
                .Property(b => b.AdvanceBookingDays).HasColumnName("advance_booking_days");
            modelBuilder.Entity<Bus>()
                .Property(b => b.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<Bus>()
                .Property(b => b.UpdatedAt).HasColumnName("updated_at");
            modelBuilder.Entity<Bus>()
                .HasIndex(b => b.RegistrationNumber)
                .IsUnique();
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.Operator)
                .WithMany(bo => bo.Buses)
                .HasForeignKey(b => b.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.Route)
                .WithMany(r => r.Buses)
                .HasForeignKey(b => b.RouteId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.SourceLocation)
                .WithMany(l => l.SourceBuses)
                .HasForeignKey(b => b.SourceLocationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Bus>()
                .HasOne(b => b.DestinationLocation)
                .WithMany(l => l.DestinationBuses)
                .HasForeignKey(b => b.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking configuration
            modelBuilder.Entity<Booking>()
                .ToTable("bookings");
            modelBuilder.Entity<Booking>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<Booking>()
                .Property(b => b.Id).HasColumnName("id");
            modelBuilder.Entity<Booking>()
                .Property(b => b.UserId).HasColumnName("user_id");
            modelBuilder.Entity<Booking>()
                .Property(b => b.BusId).HasColumnName("bus_id");
            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingDate).HasColumnName("booking_date");
            modelBuilder.Entity<Booking>()
                .Property(b => b.TravelDate).HasColumnName("travel_date");
            modelBuilder.Entity<Booking>()
                .Property(b => b.SeatNumbers).HasColumnName("seat_numbers");
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalFare).HasColumnName("total_fare");
            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingStatus).HasColumnName("booking_status");
            modelBuilder.Entity<Booking>()
                .Property(b => b.PaymentStatus).HasColumnName("payment_status");
            modelBuilder.Entity<Booking>()
                .Property(b => b.TravelStatus).HasColumnName("travel_status");
            modelBuilder.Entity<Booking>()
                .Property(b => b.PickupLocationId).HasColumnName("pickup_location_id");
            modelBuilder.Entity<Booking>()
                .Property(b => b.DropLocationId).HasColumnName("drop_location_id");
            modelBuilder.Entity<Booking>()
                .Property(b => b.PickupTime).HasColumnName("pickup_time");
            modelBuilder.Entity<Booking>()
                .Property(b => b.DropTime).HasColumnName("drop_time");
            modelBuilder.Entity<Booking>()
                .Property(b => b.ScheduleId).HasColumnName("schedule_id");
            modelBuilder.Entity<Booking>()
                .Property(b => b.IsReserved).HasColumnName("is_reserved");
            modelBuilder.Entity<Booking>()
                .Property(b => b.ReservedUntil).HasColumnName("reserved_until");
            modelBuilder.Entity<Booking>()
                .Property(b => b.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<Booking>()
                .Property(b => b.UpdatedAt).HasColumnName("updated_at");
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Bus)
                .WithMany(bu => bu.Bookings)
                .HasForeignKey(b => b.BusId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.PickupLocation)
                .WithMany()
                .HasForeignKey(b => b.PickupLocationId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.DropLocation)
                .WithMany()
                .HasForeignKey(b => b.DropLocationId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            // BusAvailability configuration
            modelBuilder.Entity<BusAvailability>()
                .ToTable("bus_availability");
            modelBuilder.Entity<BusAvailability>()
                .HasKey(ba => ba.Id);
            modelBuilder.Entity<BusAvailability>()
                .Property(ba => ba.Id).HasColumnName("id");
            modelBuilder.Entity<BusAvailability>()
                .Property(ba => ba.BusId).HasColumnName("bus_id");
            modelBuilder.Entity<BusAvailability>()
                .Property(ba => ba.AvailableDate).HasColumnName("available_date");
            modelBuilder.Entity<BusAvailability>()
                .Property(ba => ba.TotalSeats).HasColumnName("total_seats");
            modelBuilder.Entity<BusAvailability>()
                .Property(ba => ba.AvailableSeats).HasColumnName("available_seats");
            modelBuilder.Entity<BusAvailability>()
                .Property(ba => ba.IsActive).HasColumnName("is_active");
            modelBuilder.Entity<BusAvailability>()
                .Property(ba => ba.ScheduleId).HasColumnName("schedule_id");
            modelBuilder.Entity<BusAvailability>()
                .Property(ba => ba.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<BusAvailability>()
                .Property(ba => ba.UpdatedAt).HasColumnName("updated_at");
            modelBuilder.Entity<BusAvailability>()
                .HasOne(ba => ba.Bus)
                .WithMany(b => b.Availabilities)
                .HasForeignKey(ba => ba.BusId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BusAvailability>()
                .HasOne(ba => ba.Schedule)
                .WithMany(s => s.Availabilities)
                .HasForeignKey(ba => ba.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BusAvailability>()
                .HasIndex(ba => new { ba.BusId, ba.AvailableDate })
                .IsUnique();

            // BusSchedule configuration
            modelBuilder.Entity<BusSchedule>()
                .ToTable("bus_schedules");
            modelBuilder.Entity<BusSchedule>()
                .HasKey(bs => bs.Id);
            modelBuilder.Entity<BusSchedule>()
                .Property(bs => bs.Id).HasColumnName("id");
            modelBuilder.Entity<BusSchedule>()
                .Property(bs => bs.BusId).HasColumnName("bus_id");
            modelBuilder.Entity<BusSchedule>()
                .Property(bs => bs.ScheduleName).HasColumnName("schedule_name");
            modelBuilder.Entity<BusSchedule>()
                .Property(bs => bs.IsActive).HasColumnName("is_active");
            modelBuilder.Entity<BusSchedule>()
                .Property(bs => bs.OperatingDays).HasColumnName("operating_days");
            modelBuilder.Entity<BusSchedule>()
                .Property(bs => bs.EffectiveFrom).HasColumnName("effective_from");
            modelBuilder.Entity<BusSchedule>()
                .Property(bs => bs.EffectiveTo).HasColumnName("effective_to");
            modelBuilder.Entity<BusSchedule>()
                .Property(bs => bs.CreatedAt).HasColumnName("created_at");
            modelBuilder.Entity<BusSchedule>()
                .Property(bs => bs.UpdatedAt).HasColumnName("updated_at");
            modelBuilder.Entity<BusSchedule>()
                .HasOne(bs => bs.Bus)
                .WithMany(b => b.Schedules)
                .HasForeignKey(bs => bs.BusId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
