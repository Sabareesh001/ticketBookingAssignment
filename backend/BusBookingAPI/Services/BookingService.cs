using Microsoft.EntityFrameworkCore;
using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;

namespace BusBookingAPI.Services
{
    public interface IBookingService
    {
        Task<BookingDto> GetBookingByIdAsync(int id);
        Task<List<BookingDto>> GetAllBookingsAsync();
        Task<List<BookingDto>> GetBookingsByUserIdAsync(int userId);
        Task<List<BookingDto>> GetBookingsByBusIdAsync(int busId);
        Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<BookingDto> UpdateBookingAsync(int id, UpdateBookingDto updateBookingDto);
        Task<bool> DeleteBookingAsync(int id);
        Task<List<BookingDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }

    public class BookingService : IBookingService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<BookingService> _logger;
        private readonly IBusAvailabilityService _availabilityService;

        public BookingService(BusBookingDbContext context, ILogger<BookingService> logger, IBusAvailabilityService availabilityService)
        {
            _context = context;
            _logger = logger;
            _availabilityService = availabilityService;
        }

        public async Task<BookingDto> GetBookingByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching booking with ID {id}");

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Bus)
                .Include(b => b.Bus.Operator)
                .Include(b => b.Bus.Route)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID {id} not found");
                throw new KeyNotFoundException($"Booking with ID {id} not found");
            }

            return MapToDto(booking);
        }

        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            _logger.LogInformation("Fetching all bookings");

            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Bus)
                .Include(b => b.Bus.Operator)
                .Include(b => b.Bus.Route)
                .ToListAsync();

            return bookings.Select(MapToDto).ToList();
        }

        public async Task<List<BookingDto>> GetBookingsByUserIdAsync(int userId)
        {
            _logger.LogInformation($"Fetching bookings for user with ID {userId}");

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.User)
                .Include(b => b.Bus)
                .Include(b => b.Bus.Operator)
                .Include(b => b.Bus.Route)
                .ToListAsync();

            return bookings.Select(MapToDto).ToList();
        }

        public async Task<List<BookingDto>> GetBookingsByBusIdAsync(int busId)
        {
            _logger.LogInformation($"Fetching bookings for bus with ID {busId}");

            var busExists = await _context.Buses.AnyAsync(b => b.Id == busId);
            if (!busExists)
            {
                throw new KeyNotFoundException($"Bus with ID {busId} not found");
            }

            var bookings = await _context.Bookings
                .Where(b => b.BusId == busId)
                .Include(b => b.User)
                .Include(b => b.Bus)
                .Include(b => b.Bus.Operator)
                .Include(b => b.Bus.Route)
                .ToListAsync();

            return bookings.Select(MapToDto).ToList();
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto)
        {
            _logger.LogInformation($"Creating new booking for user {createBookingDto.UserId} on bus {createBookingDto.BusId}");

            // Validate that the user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == createBookingDto.UserId);
            if (!userExists)
            {
                throw new ArgumentException($"User with ID {createBookingDto.UserId} not found");
            }

            // Validate that the bus exists
            var busExists = await _context.Buses.AnyAsync(b => b.Id == createBookingDto.BusId);
            if (!busExists)
            {
                throw new ArgumentException($"Bus with ID {createBookingDto.BusId} not found");
            }

            // Validate that travel date is in the future
            var travelDateUtc = createBookingDto.TravelDate.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(createBookingDto.TravelDate, DateTimeKind.Utc)
                : createBookingDto.TravelDate.ToUniversalTime();
                
            if (travelDateUtc <= DateTime.UtcNow)
            {
                throw new ArgumentException("Travel date must be in the future");
            }

            // Check if the date is available for booking
            var seatNumbers = createBookingDto.SeatNumbers.Split(',').Select(s => s.Trim()).ToList();
            var requiredSeats = seatNumbers.Count;
            
            var isDateAvailable = await _availabilityService.IsDateAvailableAsync(createBookingDto.BusId, travelDateUtc, requiredSeats);
            if (!isDateAvailable)
            {
                throw new InvalidOperationException($"The selected date is not available for booking or insufficient seats available");
            }

            // Validate seat availability (check if seats are already booked)
            var existingBooking = await _context.Bookings
                .Where(b => b.BusId == createBookingDto.BusId &&
                           b.TravelDate.Date == travelDateUtc.Date &&
                           b.BookingStatus == "confirmed")
                .ToListAsync();

            foreach (var existingB in existingBooking)
            {
                var existingSeats = existingB.SeatNumbers.Split(',').Select(s => s.Trim()).ToList();
                var overlap = seatNumbers.Intersect(existingSeats).Any();
                if (overlap)
                {
                    throw new InvalidOperationException($"Some of the requested seats are already booked for this journey");
                }
            }

            var booking = new Booking
            {
                UserId = createBookingDto.UserId,
                BusId = createBookingDto.BusId,
                BookingDate = DateTime.UtcNow,
                TravelDate = travelDateUtc,
                SeatNumbers = createBookingDto.SeatNumbers,
                TotalFare = createBookingDto.TotalFare,
                BookingStatus = "confirmed",
                PaymentStatus = "pending",
                TravelStatus = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Update availability after successful booking
            await _availabilityService.UpdateAvailabilityOnBookingAsync(createBookingDto.BusId, travelDateUtc, requiredSeats, true);

            _logger.LogInformation($"Booking created successfully with ID {booking.Id}");

            return await GetBookingByIdAsync(booking.Id);
        }

        public async Task<BookingDto> UpdateBookingAsync(int id, UpdateBookingDto updateBookingDto)
        {
            _logger.LogInformation($"Updating booking with ID {id}");

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID {id} not found");
                throw new KeyNotFoundException($"Booking with ID {id} not found");
            }

            var oldSeatCount = booking.SeatNumbers.Split(',').Length;
            var oldTravelDate = booking.TravelDate;
            var oldBookingStatus = booking.BookingStatus;

            // Validate that travel date is in the future (if being changed)
            var travelDateUtc = updateBookingDto.TravelDate.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(updateBookingDto.TravelDate, DateTimeKind.Utc)
                : updateBookingDto.TravelDate.ToUniversalTime();
                
            if (updateBookingDto.TravelDate != booking.TravelDate && travelDateUtc <= DateTime.UtcNow)
            {
                throw new ArgumentException("Travel date must be in the future");
            }

            // Check availability for new date if date is being changed
            if (updateBookingDto.TravelDate != booking.TravelDate)
            {
                var newSeatCount = updateBookingDto.SeatNumbers.Split(',').Length;
                var isNewDateAvailable = await _availabilityService.IsDateAvailableAsync(booking.BusId, travelDateUtc, newSeatCount);
                if (!isNewDateAvailable)
                {
                    throw new InvalidOperationException($"The new travel date is not available for booking");
                }
            }

            // Validate seat availability if seats are being changed
            if (updateBookingDto.SeatNumbers != booking.SeatNumbers)
            {
                var seatNumbers = updateBookingDto.SeatNumbers.Split(',').Select(s => s.Trim()).ToList();
                var existingBooking = await _context.Bookings
                    .Where(b => b.BusId == booking.BusId &&
                               b.TravelDate.Date == travelDateUtc.Date &&
                               b.Id != id &&
                               b.BookingStatus == "confirmed")
                    .ToListAsync();

                foreach (var existingB in existingBooking)
                {
                    var existingSeats = existingB.SeatNumbers.Split(',').Select(s => s.Trim()).ToList();
                    var overlap = seatNumbers.Intersect(existingSeats).Any();
                    if (overlap)
                    {
                        throw new InvalidOperationException($"Some of the requested seats are already booked for this journey");
                    }
                }
            }

            booking.TravelDate = travelDateUtc;
            booking.SeatNumbers = updateBookingDto.SeatNumbers;
            booking.TotalFare = updateBookingDto.TotalFare;
            booking.BookingStatus = updateBookingDto.BookingStatus;
            booking.PaymentStatus = updateBookingDto.PaymentStatus;
            booking.TravelStatus = updateBookingDto.TravelStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            // Update availability based on booking status changes
            if (oldBookingStatus == "confirmed" && updateBookingDto.BookingStatus == "cancelled")
            {
                // Restore availability when booking is cancelled
                await _availabilityService.UpdateAvailabilityOnBookingAsync(booking.BusId, oldTravelDate, oldSeatCount, false);
            }
            else if (oldBookingStatus != "confirmed" && updateBookingDto.BookingStatus == "confirmed")
            {
                // Reduce availability when booking is confirmed
                var newSeatCount = updateBookingDto.SeatNumbers.Split(',').Length;
                await _availabilityService.UpdateAvailabilityOnBookingAsync(booking.BusId, travelDateUtc, newSeatCount, true);
            }

            _logger.LogInformation($"Booking with ID {id} updated successfully");

            return await GetBookingByIdAsync(id);
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            _logger.LogInformation($"Deleting booking with ID {id}");

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                _logger.LogWarning($"Booking with ID {id} not found");
                throw new KeyNotFoundException($"Booking with ID {id} not found");
            }

            // Only allow deletion if booking is not completed
            if (booking.TravelStatus == "completed")
            {
                throw new InvalidOperationException($"Cannot delete a completed booking");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Booking with ID {id} deleted successfully");

            return true;
        }

        public async Task<List<BookingDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation($"Fetching bookings between {startDate} and {endDate}");

            // Ensure dates are in UTC
            var startDateUtc = startDate.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(startDate, DateTimeKind.Utc)
                : startDate.ToUniversalTime();
                
            var endDateUtc = endDate.Kind == DateTimeKind.Unspecified 
                ? DateTime.SpecifyKind(endDate, DateTimeKind.Utc)
                : endDate.ToUniversalTime();

            var bookings = await _context.Bookings
                .Where(b => b.TravelDate >= startDateUtc && b.TravelDate <= endDateUtc)
                .Include(b => b.User)
                .Include(b => b.Bus)
                .Include(b => b.Bus.Operator)
                .Include(b => b.Bus.Route)
                .ToListAsync();

            return bookings.Select(MapToDto).ToList();
        }

        private BookingDto MapToDto(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                BusId = booking.BusId,
                BookingDate = booking.BookingDate,
                TravelDate = booking.TravelDate,
                SeatNumbers = booking.SeatNumbers,
                TotalFare = booking.TotalFare,
                BookingStatus = booking.BookingStatus,
                PaymentStatus = booking.PaymentStatus,
                TravelStatus = booking.TravelStatus,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }
    }
}
