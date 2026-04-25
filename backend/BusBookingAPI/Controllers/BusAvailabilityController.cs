using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/busavailability")]
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
        /// Get all bus availability records
        /// </summary>
        /// <returns>List of all availability records</returns>
        [HttpGet]
        public async Task<ActionResult<List<BusAvailabilityDto>>> GetAllAvailabilities()
        {
            try
            {
                _logger.LogInformation("Getting all bus availability records");
                var availabilities = await _availabilityService.GetAllAvailabilitiesAsync();
                return Ok(availabilities);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all availabilities: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving availabilities" });
            }
        }

        /// <summary>
        /// Get a specific bus availability record by ID
        /// </summary>
        /// <param name="id">Availability ID</param>
        /// <returns>Availability record</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BusAvailabilityDto>> GetAvailabilityById(int id)
        {
            try
            {
                _logger.LogInformation($"Getting availability record {id}");
                var availability = await _availabilityService.GetAvailabilityByIdAsync(id);
                return Ok(availability);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Availability not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting availability: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving availability" });
            }
        }

        /// <summary>
        /// Get all availability records for a specific bus
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <returns>List of availability records for the bus</returns>
        [HttpGet("bus/{busId}")]
        public async Task<ActionResult<List<BusAvailabilityDto>>> GetAvailabilitiesByBus(int busId)
        {
            try
            {
                _logger.LogInformation($"Getting availability records for bus {busId}");
                var availabilities = await _availabilityService.GetAvailabilitiesByBusAsync(busId);
                return Ok(availabilities);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting availabilities for bus: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving availabilities" });
            }
        }

        /// <summary>
        /// Create a new bus availability record
        /// </summary>
        /// <param name="createDto">Availability data</param>
        /// <returns>Created availability record</returns>
        [HttpPost]
        public async Task<ActionResult<BusAvailabilityDto>> CreateAvailability(CreateBusAvailabilityDto createDto)
        {
            try
            {
                _logger.LogInformation($"Creating availability for bus {createDto.BusId} on {createDto.AvailableDate}");
                var availability = await _availabilityService.CreateAvailabilityAsync(createDto);
                return CreatedAtAction(nameof(GetAvailabilityById), new { id = availability.Id }, availability);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating availability: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating availability" });
            }
        }

        /// <summary>
        /// Update an existing bus availability record
        /// </summary>
        /// <param name="id">Availability ID</param>
        /// <param name="updateDto">Updated availability data</param>
        /// <returns>Updated availability record</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BusAvailabilityDto>> UpdateAvailability(int id, UpdateBusAvailabilityDto updateDto)
        {
            try
            {
                _logger.LogInformation($"Updating availability record {id}");
                var availability = await _availabilityService.UpdateAvailabilityAsync(id, updateDto);
                return Ok(availability);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Availability not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating availability: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating availability" });
            }
        }

        /// <summary>
        /// Delete a bus availability record
        /// </summary>
        /// <param name="id">Availability ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAvailability(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting availability record {id}");
                await _availabilityService.DeleteAvailabilityAsync(id);
                return Ok(new { message = "Availability deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Availability not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting availability: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting availability" });
            }
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

                // Log the data being returned for debugging
                _logger.LogInformation($"Returning {availability.Count} availability records");
                if (availability.Count > 0)
                {
                    var first = availability[0];
                    _logger.LogInformation($"First record - BusId: {first.BusId}, AvailableSeats: {first.AvailableSeats}");
                }

                return Ok(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting availability details: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while retrieving availability details", error = ex.Message });
            }
        }

        /// <summary>
        /// Debug endpoint: Get raw availability data with all fields
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <param name="date">Date to get availability for</param>
        /// <returns>Raw availability data for debugging</returns>
        [HttpGet("debug/{busId}")]
        public async Task<ActionResult> GetBusAvailabilityDebug(
            int busId,
            [FromQuery] DateTime date)
        {
            try
            {
                _logger.LogInformation($"DEBUG: Getting availability for bus {busId} on {date}");
                var availability = await _availabilityService.GetBusAvailabilityAsync(busId, date);

                return Ok(new
                {
                    success = true,
                    busId = busId,
                    requestedDate = date,
                    recordCount = availability.Count,
                    data = availability,
                    message = availability.Count == 0
                        ? "No availability records found. You may need to generate availability first."
                        : $"Found {availability.Count} availability record(s)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"DEBUG ERROR: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
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

                return Ok(new
                {
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

        /// <summary>
        /// Update timing for a specific bus availability date
        /// </summary>
        /// <param name="updateDto">Timing update details</param>
        /// <returns>Success message</returns>
        [HttpPut("update-timing")]
        public async Task<ActionResult> UpdateAvailabilityTiming(UpdateBusAvailabilityTimingDto updateDto)
        {
            try
            {
                _logger.LogInformation($"Updating timing for bus {updateDto.BusId} on {updateDto.AvailableDate}");
                var result = await _availabilityService.UpdateAvailabilityTimingAsync(updateDto);

                if (result)
                {
                    return Ok(new { message = "Availability timing updated successfully" });
                }

                return StatusCode(500, new { message = "Failed to update availability timing" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Availability not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating availability timing: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating availability timing" });
            }
        }

        /// <summary>
        /// Bulk update timing for multiple dates of a bus
        /// </summary>
        /// <param name="bulkUpdateDto">Bulk timing update details</param>
        /// <returns>Success message</returns>
        [HttpPut("bulk-update-timing")]
        public async Task<ActionResult> BulkUpdateAvailabilityTiming(BulkUpdateAvailabilityTimingDto bulkUpdateDto)
        {
            try
            {
                _logger.LogInformation($"Bulk updating timing for bus {bulkUpdateDto.BusId} for {bulkUpdateDto.Dates.Count} dates");
                var result = await _availabilityService.BulkUpdateAvailabilityTimingAsync(bulkUpdateDto);

                return Ok(new
                {
                    message = $"Successfully updated timing for {result.UpdatedCount} out of {bulkUpdateDto.Dates.Count} dates",
                    updatedCount = result.UpdatedCount,
                    failedDates = result.FailedDates
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error bulk updating availability timing: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while bulk updating availability timing" });
            }
        }

        /// <summary>
        /// Get available dates with timing information for a specific bus
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <param name="startDate">Start date (optional, defaults to today)</param>
        /// <param name="endDate">End date (optional, defaults to 90 days from today)</param>
        /// <returns>Available dates with timing information</returns>
        [HttpGet("available-dates-with-timing/{busId}")]
        public async Task<ActionResult<AvailableDatesResponse>> GetAvailableDatesWithTiming(
            int busId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation($"Getting available dates with timing for bus {busId}");
                var availableDates = await _availabilityService.GetAvailableDatesWithTimingAsync(busId, startDate, endDate);
                return Ok(availableDates);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting available dates with timing: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving available dates with timing" });
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