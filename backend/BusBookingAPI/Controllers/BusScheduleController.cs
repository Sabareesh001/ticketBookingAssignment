using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusScheduleController : ControllerBase
    {
        private readonly IBusScheduleService _scheduleService;
        private readonly ILogger<BusScheduleController> _logger;

        public BusScheduleController(IBusScheduleService scheduleService, ILogger<BusScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        /// <summary>
        /// Get all schedules for a specific bus
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <returns>List of schedules for the bus</returns>
        [HttpGet("bus/{busId}")]
        public async Task<ActionResult<List<BusScheduleDto>>> GetBusSchedules(int busId)
        {
            try
            {
                _logger.LogInformation($"Getting schedules for bus {busId}");
                var schedules = await _scheduleService.GetBusSchedulesAsync(busId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting bus schedules: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving bus schedules" });
            }
        }

        /// <summary>
        /// Get active schedules for a specific bus and date
        /// </summary>
        /// <param name="busId">Bus ID</param>
        /// <param name="date">Date to check (YYYY-MM-DD format)</param>
        /// <returns>List of active schedules for the date</returns>
        [HttpGet("bus/{busId}/active")]
        public async Task<ActionResult<List<BusScheduleDto>>> GetActiveSchedulesForDate(int busId, [FromQuery] string date)
        {
            try
            {
                if (!DateTime.TryParse(date, out DateTime parsedDate))
                {
                    return BadRequest(new { message = "Invalid date format. Use YYYY-MM-DD format." });
                }

                _logger.LogInformation($"Getting active schedules for bus {busId} on {date}");
                var schedules = await _scheduleService.GetActiveSchedulesForDateAsync(busId, parsedDate);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting active schedules: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving active schedules" });
            }
        }

        /// <summary>
        /// Get a specific schedule by ID
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>Schedule details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BusScheduleDto>> GetBusSchedule(int id)
        {
            try
            {
                _logger.LogInformation($"Getting schedule {id}");
                var schedule = await _scheduleService.GetBusScheduleByIdAsync(id);
                return Ok(schedule);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Schedule not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting schedule: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the schedule" });
            }
        }

        /// <summary>
        /// Create a new bus schedule
        /// </summary>
        /// <param name="createScheduleDto">Schedule details</param>
        /// <returns>Created schedule</returns>
        [HttpPost]
        public async Task<ActionResult<BusScheduleDto>> CreateBusSchedule(CreateBusScheduleDto createScheduleDto)
        {
            try
            {
                _logger.LogInformation("Creating new bus schedule");
                var schedule = await _scheduleService.CreateBusScheduleAsync(createScheduleDto);
                return CreatedAtAction(nameof(GetBusSchedule), new { id = schedule.Id }, schedule);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid schedule data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating schedule: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the schedule" });
            }
        }

        /// <summary>
        /// Update an existing bus schedule
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <param name="updateScheduleDto">Updated schedule details</param>
        /// <returns>Updated schedule</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BusScheduleDto>> UpdateBusSchedule(int id, UpdateBusScheduleDto updateScheduleDto)
        {
            try
            {
                _logger.LogInformation($"Updating schedule {id}");
                var schedule = await _scheduleService.UpdateBusScheduleAsync(id, updateScheduleDto);
                return Ok(schedule);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Schedule not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid schedule data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating schedule: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the schedule" });
            }
        }

        /// <summary>
        /// Delete a bus schedule
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBusSchedule(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting schedule {id}");
                var result = await _scheduleService.DeleteBusScheduleAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Schedule {id} deleted successfully" });
                }
                return StatusCode(500, new { message = "Failed to delete schedule" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Schedule not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting schedule: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the schedule" });
            }
        }
    }
}