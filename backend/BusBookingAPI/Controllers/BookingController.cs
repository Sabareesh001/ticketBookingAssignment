using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        /// <summary>
        /// Get a booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            try
            {
                _logger.LogInformation($"Getting booking with ID {id}");
                var booking = await _bookingService.GetBookingByIdAsync(id);
                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Booking not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving booking: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the booking" });
            }
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        /// <returns>List of all bookings</returns>
        [HttpGet]
        public async Task<ActionResult<List<BookingDto>>> GetAllBookings()
        {
            try
            {
                _logger.LogInformation("Getting all bookings");
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving bookings: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving bookings" });
            }
        }

        /// <summary>
        /// Get all bookings for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of bookings for the user</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<BookingDto>>> GetBookingsByUserId(int userId)
        {
            try
            {
                _logger.LogInformation($"Getting bookings for user with ID {userId}");
                var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
                return Ok(bookings);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"User not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user bookings: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving user bookings" });
            }
        }

        /// <summary>
        /// Get all bookings for a specific bus
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <returns>List of bookings for the bus</returns>
        [HttpGet("bus/{busId}")]
        public async Task<ActionResult<List<BookingDto>>> GetBookingsByBusId(int busId)
        {
            try
            {
                _logger.LogInformation($"Getting bookings for bus with ID {busId}");
                var bookings = await _bookingService.GetBookingsByBusIdAsync(busId);
                return Ok(bookings);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving bus bookings: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving bus bookings" });
            }
        }

        /// <summary>
        /// Get booked seats for a specific bus and date
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <param name="travelDate">Travel date (YYYY-MM-DD format)</param>
        /// <returns>List of bookings for the bus on the specified date</returns>
        [HttpGet("bus/{busId}/seats")]
        public async Task<ActionResult<List<BookingDto>>> GetBookedSeats(int busId, [FromQuery] string travelDate)
        {
            try
            {
                _logger.LogInformation($"Getting booked seats for bus {busId} on {travelDate}");
                
                if (!DateTime.TryParse(travelDate, out DateTime parsedDate))
                {
                    return BadRequest(new { message = "Invalid date format. Use YYYY-MM-DD format." });
                }

                var bookings = await _bookingService.GetBookingsByBusIdAsync(busId);
                
                // Filter bookings for the specific date and confirmed status
                var dateBookings = bookings.Where(b => 
                    b.TravelDate.Date == parsedDate.Date && 
                    b.BookingStatus == "confirmed"
                ).ToList();

                return Ok(dateBookings);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving booked seats: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving booked seats" });
            }
        }

        /// <summary>
        /// Get bookings within a date range
        /// </summary>
        /// <param name="startDate">Start date (ISO 8601 format)</param>
        /// <param name="endDate">End date (ISO 8601 format)</param>
        /// <returns>List of bookings within the date range</returns>
        [HttpGet("date-range")]
        public async Task<ActionResult<List<BookingDto>>> GetBookingsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                _logger.LogInformation($"Getting bookings between {startDate} and {endDate}");
                var bookings = await _bookingService.GetBookingsByDateRangeAsync(startDate, endDate);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving bookings by date range: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving bookings" });
            }
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        /// <param name="createBookingDto">Booking details</param>
        /// <returns>Created booking</returns>
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto createBookingDto)
        {
            try
            {
                _logger.LogInformation("Creating new booking");
                var booking = await _bookingService.CreateBookingAsync(createBookingDto);
                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid booking data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating booking: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the booking" });
            }
        }

        /// <summary>
        /// Update an existing booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="updateBookingDto">Updated booking details</param>
        /// <returns>Updated booking</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BookingDto>> UpdateBooking(int id, UpdateBookingDto updateBookingDto)
        {
            try
            {
                _logger.LogInformation($"Updating booking with ID {id}");
                var booking = await _bookingService.UpdateBookingAsync(id, updateBookingDto);
                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Booking not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid booking data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating booking: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the booking" });
            }
        }

        /// <summary>
        /// Delete a booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBooking(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting booking with ID {id}");
                var result = await _bookingService.DeleteBookingAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Booking with ID {id} deleted successfully" });
                }
                return StatusCode(500, new { message = "Failed to delete booking" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Booking not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting booking: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the booking" });
            }
        }
    }
}
