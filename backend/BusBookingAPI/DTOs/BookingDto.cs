namespace BusBookingAPI.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BusId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime TravelDate { get; set; }
        public string SeatNumbers { get; set; }
        public decimal TotalFare { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string TravelStatus { get; set; }
        
        // Pickup and Drop Information
        public int? PickupLocationId { get; set; }
        public int? DropLocationId { get; set; }
        public TimeSpan? PickupTime { get; set; }
        public TimeSpan? DropTime { get; set; }
        public int? ScheduleId { get; set; }
        public string PickupLocationName { get; set; }
        public string DropLocationName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
