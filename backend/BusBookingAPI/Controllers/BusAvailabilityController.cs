using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusAvailabilityController : ControllerBase
    {
        private readonly IBusAvailabilityService _availabilityService;
        private readonly ILogger<BusAvailabilityController> _logger;

        public BusAvailabilityController(IBusAvailabilityService availabilityService, ILogger<BusAvailabilityController> logger)
        {
            _availabilityService = availabilityService;
            _logger = logger;
        }

        /// <summary>
        /// Get available dates for a specific bus
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <param name="startDate">Start date (optional, defaults to today)</param>
        /// <param name="endDate">End date (optional, defaults to 90 days from today)</param>
        /// <returns>Available dates and seat counts</returns>
        [HttpGet("available-dates/{busId}")]
        public async Task<ActionResult<AvailableDatesResponse>> GetAvailableDates(
            int busId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation($"Getting available dates for bus {busId}");
                var availableDates = await _availabilityService.GetAvailableDatesAsync(busId, startDate, endDate);
                return Ok(availableDates);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting available dates: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving available dates" });
            }
        }

        /// <summary>
        /// Check if a specific date is available for booking
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <param name="date">Date to check</param>
        /// <param name="requiredSeats">Number of seats required (default: 1)</param>
        /// <returns>Boolean indicating availability</returns>
        [HttpGet("check-availability/{busId}")]
        public async Task<ActionResult<bool>> CheckDateAvailability(
            int busId, 
            [FromQuery] DateTime date, 
            [FromQuery] int requiredSeats = 1)
        {
            try
            {
                _logger.LogInformation($"Checking availability for bus {busId} on {date} for {requiredSeats} seats");
                var isAvailable = await _availabilityService.IsDateAvailableAsync(busId, date, requiredSeats);
                return Ok(new { isAvailable, busId, date, requiredSeats });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking date availability: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while checking availability" });
            }
        }

        /// <summary>
        /// Get detailed availability for a specific bus and date
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <param name="date">Date to get availability for</param>
        /// <returns>Detailed availability information</returns>
        [HttpGet("details/{busId}")]
        public async Task<ActionResult<List<BusAvailabilityDto>>> GetBusAvailabilityDetails(
            int busId, 
            [FromQuery] DateTime date)
        {
            try
            {
                _logger.LogInformation($"Getting availability details for bus {busId} on {date}");
                var availability = await _availabilityService.GetBusAvailabilityAsync(busId, date);
                return Ok(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting availability details: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving availability details" });
            }
        }

        /// <summary>
        /// Generate availability for a specific bus (90 days ahead)
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <returns>Success message</returns>
        [HttpPost("generate/{busId}")]
        public async Task<ActionResult> GenerateBusAvailability(int busId)
        {
            try
            {
                _logger.LogInformation($"Generating availability for bus {busId}");
                await _availabilityService.GenerateBusAvailabilityAsync(busId);
                return Ok(new { message = $"Availability generated successfully for bus {busId}" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating availability: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while generating availability" });
            }
        }

        /// <summary>
        /// Get bus schedule information
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <returns>Bus schedule details</returns>
        [HttpGet("schedule/{busId}")]
        public async Task<ActionResult<BusScheduleDto>> GetBusSchedule(int busId)
        {
            try
            {
                _logger.LogInformation($"Getting schedule for bus {busId}");
                var schedule = await _availabilityService.GetBusScheduleAsync(busId);
                return Ok(schedule);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting bus schedule: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving bus schedule" });
            }
        }

        /// <summary>
        /// Update bus schedule and regenerate availability
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <param name="scheduleDto">Updated schedule information</param>
        /// <returns>Success message</returns>
        [HttpPut("schedule/{busId}")]
        public async Task<ActionResult> UpdateBusSchedule(int busId, BusScheduleDto scheduleDto)
        {
            try
            {
                _logger.LogInformation($"Updating schedule for bus {busId}");
                var result = await _availabilityService.UpdateBusScheduleAsync(busId, scheduleDto);
                
                if (result)
                {
                    return Ok(new { message = $"Bus schedule updated successfully for bus {busId}" });
                }
                
                return StatusCode(500, new { message = "Failed to update bus schedule" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating bus schedule: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating bus schedule" });
            }
        }

        /// <summary>
        /// Generate availability for all active buses (Admin endpoint)
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("generate-all")]
        public async Task<ActionResult> GenerateAllBusAvailability()
        {
            try
            {
                _logger.LogInformation("Generating availability for all active buses");
                
                // Get all active buses and generate availability for each
                // This is a simplified implementation - in production you'd want to do this in batches
                var buses = await GetAllActiveBuses();
                var results = new List<string>();
                
                foreach (var busId in buses)
                {
                    try
                    {
                        await _availabilityService.GenerateBusAvailabilityAsync(busId);
                        results.Add($"Bus {busId}: Success");
                    }
                    catch (Exception ex)
                    {
                        results.Add($"Bus {busId}: Failed - {ex.Message}");
                    }
                }
                
                return Ok(new { 
                    message = "Availability generation completed",
                    results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating availability for all buses: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while generating availability" });
            }
        }

        private async Task<List<int>> GetAllActiveBuses()
        {
            // This is a placeholder - you'd need to inject IBusService or access the context directly
            // For now, return a hardcoded list or implement this properly
            return new List<int> { 1, 2, 3, 4, 5 }; // Replace with actual bus IDs
        }
    }
}