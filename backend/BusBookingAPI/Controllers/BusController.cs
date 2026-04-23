using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusController : ControllerBase
    {
        private readonly IBusService _busService;
        private readonly ILogger<BusController> _logger;

        public BusController(IBusService busService, ILogger<BusController> logger)
        {
            _busService = busService;
            _logger = logger;
        }

        /// <summary>
        /// Get a bus by ID
        /// </summary>
        /// <param name="id">Bus ID</param>
        /// <returns>Bus details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BusDto>> GetBus(int id)
        {
            try
            {
                _logger.LogInformation($"Getting bus with ID {id}");
                var bus = await _busService.GetBusByIdAsync(id);
                return Ok(bus);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving bus: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the bus" });
            }
        }

        /// <summary>
        /// Get all buses
        /// </summary>
        /// <returns>List of all buses</returns>
        [HttpGet]
        public async Task<ActionResult<List<BusDto>>> GetAllBuses()
        {
            try
            {
                _logger.LogInformation("Getting all buses");
                var buses = await _busService.GetAllBusesAsync();
                return Ok(buses);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving buses: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving buses" });
            }
        }

        /// <summary>
        /// Create a new bus
        /// </summary>
        /// <param name="createBusDto">Bus details</param>
        /// <returns>Created bus</returns>
        [HttpPost]
        public async Task<ActionResult<BusDto>> CreateBus(CreateBusDto createBusDto)
        {
            try
            {
                _logger.LogInformation("Creating new bus");
                var bus = await _busService.CreateBusAsync(createBusDto);
                return CreatedAtAction(nameof(GetBus), new { id = bus.Id }, bus);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid bus data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating bus: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the bus" });
            }
        }

        /// <summary>
        /// Update an existing bus
        /// </summary>
        /// <param name="id">Bus ID</param>
        /// <param name="updateBusDto">Updated bus details</param>
        /// <returns>Updated bus</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<BusDto>> UpdateBus(int id, UpdateBusDto updateBusDto)
        {
            try
            {
                _logger.LogInformation($"Updating bus with ID {id}");
                var bus = await _busService.UpdateBusAsync(id, updateBusDto);
                return Ok(bus);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid bus data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating bus: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the bus" });
            }
        }

        /// <summary>
        /// Delete a bus
        /// </summary>
        /// <param name="id">Bus ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBus(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting bus with ID {id}");
                var result = await _busService.DeleteBusAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Bus with ID {id} deleted successfully" });
                }
                return StatusCode(500, new { message = "Failed to delete bus" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Bus not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting bus: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the bus" });
            }
        }

        /// <summary>
        /// Get available buses by route (source and destination districts)
        /// </summary>
        /// <param name="sourceDistrict">Source district name</param>
        /// <param name="destinationDistrict">Destination district name</param>
        /// <returns>List of available buses</returns>
        [HttpGet("available")]
        public async Task<ActionResult<AvailableBusesResponse>> GetAvailableBuses([FromQuery] string sourceDistrict, [FromQuery] string destinationDistrict)
        {
            try
            {
                _logger.LogInformation($"Getting available buses from {sourceDistrict} to {destinationDistrict}");
                var response = await _busService.GetAvailableBusesAsync(sourceDistrict, destinationDistrict);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving available buses: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving available buses" });
            }
        }
    }
}
