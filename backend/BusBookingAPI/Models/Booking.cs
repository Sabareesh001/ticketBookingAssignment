namespace BusBookingAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BusId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public DateTime TravelDate { get; set; }
        public string SeatNumbers { get; set; }
        public decimal TotalFare { get; set; }
        public string BookingStatus { get; set; } = "confirmed";
        public string PaymentStatus { get; set; } = "pending";
        public string TravelStatus { get; set; } = "active";
        
        // Seat reservation fields
        public bool IsReserved { get; set; } = false;
        public DateTime? ReservedUntil { get; set; }
        
        // Pickup and Drop Information
        public int? PickupLocationId { get; set; }
        public int? DropLocationId { get; set; }
        public TimeSpan? PickupTime { get; set; }
        public TimeSpan? DropTime { get; set; }
        public int? ScheduleId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public User User { get; set; }
        public Bus Bus { get; set; }
        public Location PickupLocation { get; set; }
        public Location DropLocation { get; set; }
        public BusSchedule Schedule { get; set; }
    }
}
