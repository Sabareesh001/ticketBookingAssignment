namespace BusBookingAPI.DTOs
{
    public class AvailableBusesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<BusDto> Buses { get; set; } = new List<BusDto>();
    }
}
