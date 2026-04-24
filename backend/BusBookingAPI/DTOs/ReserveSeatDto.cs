namespace BusBookingAPI.DTOs
{
    public class ReserveSeatDto
    {
        public int UserId { get; set; }
        public int BusId { get; set; }
        public DateTime TravelDate { get; set; }
        public string SeatNumbers { get; set; }
    }

    public class ReserveSeatResponse
    {
        public int ReservationId { get; set; }
        public string SeatNumbers { get; set; }
        public DateTime ReservedUntil { get; set; }
        public int RemainingSeconds { get; set; }
    }

    public class ReleaseReservationDto
    {
        public int ReservationId { get; set; }
    }
}
