using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using System.Security.Claims;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/operator-dashboard")]
    [Authorize]
    public class OperatorDashboardController : ControllerBase
    {
        private readonly IBusService _busService;
        private readonly ILocationService _locationService;
        private readonly IOperatorService _operatorService;
        private readonly ILogger<OperatorDashboardController> _logger;

        public OperatorDashboardController(
            IBusService busService,
            ILocationService locationService,
            IOperatorService operatorService,
            ILogger<OperatorDashboardController> logger)
        {
            _busService = busService;
            _locationService = locationService;
            _operatorService = operatorService;
            _logger = logger;
        }

        private int GetOperatorIdFromToken()
        {
            var operatorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (operatorIdClaim == null || !int.TryParse(operatorIdClaim.Value, out int operatorId))
            {
                throw new UnauthorizedAccessException("Invalid operator token");
            }
            return operatorId;
        }

        /// <summary>
        /// Get all buses for the current operator
        /// </summary>
        [HttpGet("buses")]
        public async Task<ActionResult<List<BusDto>>> GetMyBuses()
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Fetching buses for operator {operatorId}");
                var buses = await _operatorService.GetOperatorBusesAsync(operatorId);
                return Ok(buses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching operator buses: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching buses" });
            }
        }

        /// <summary>
        /// Create a new bus for the current operator
        /// </summary>
        [HttpPost("buses")]
        public async Task<ActionResult<BusDto>> CreateBus([FromBody] CreateBusDto createBusDto)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Creating bus for operator {operatorId}");

                // Ensure the bus is created for the current operator
                createBusDto.OperatorId = operatorId;

                var bus = await _busService.CreateBusAsync(createBusDto);
                return CreatedAtAction(nameof(GetBusById), new { id = bus.Id }, bus);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
        /// Get a specific bus by ID (must belong to current operator)
        /// </summary>
        [HttpGet("buses/{id}")]
        public async Task<ActionResult<BusDto>> GetBusById(int id)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Fetching bus {id} for operator {operatorId}");

                var bus = await _busService.GetBusByIdAsync(id);

                if (bus.OperatorId != operatorId)
                {
                    return Forbid();
                }

                return Ok(bus);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching bus: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching the bus" });
            }
        }

        /// <summary>
        /// Update a bus (must belong to current operator)
        /// </summary>
        [HttpPut("buses/{id}")]
        public async Task<ActionResult<BusDto>> UpdateBus(int id, [FromBody] UpdateBusDto updateBusDto)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Updating bus {id} for operator {operatorId}");

                var existingBus = await _busService.GetBusByIdAsync(id);

                if (existingBus.OperatorId != operatorId)
                {
                    return Forbid();
                }

                var updatedBus = await _busService.UpdateBusAsync(id, updateBusDto);
                return Ok(updatedBus);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating bus: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the bus" });
            }
        }

        /// <summary>
        /// Delete a bus (must belong to current operator)
        /// </summary>
        [HttpDelete("buses/{id}")]
        public async Task<ActionResult> DeleteBus(int id)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Deleting bus {id} for operator {operatorId}");

                var existingBus = await _busService.GetBusByIdAsync(id);

                if (existingBus.OperatorId != operatorId)
                {
                    return Forbid();
                }

                var result = await _busService.DeleteBusAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Bus with ID {id} deleted successfully" });
                }
                return StatusCode(500, new { message = "Failed to delete bus" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting bus: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the bus" });
            }
        }

        /// <summary>
        /// Get all locations for the current operator
        /// </summary>
        [HttpGet("locations")]
        public async Task<ActionResult<List<LocationDto>>> GetMyLocations()
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Fetching locations for operator {operatorId}");
                var locations = await _operatorService.GetOperatorLocationsAsync(operatorId);
                return Ok(locations);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching operator locations: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching locations" });
            }
        }

        /// <summary>
        /// Create a new location for the current operator
        /// </summary>
        [HttpPost("locations")]
        public async Task<ActionResult<LocationDto>> CreateLocation([FromBody] CreateLocationDto createLocationDto)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Creating location for operator {operatorId}");

                // Ensure the location is created for the current operator
                createLocationDto.OperatorId = operatorId;

                var location = await _locationService.CreateLocationAsync(createLocationDto);
                return CreatedAtAction(nameof(GetLocationById), new { id = location.Id }, location);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating location: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the location" });
            }
        }

        /// <summary>
        /// Get a specific location by ID (must belong to current operator)
        /// </summary>
        [HttpGet("locations/{id}")]
        public async Task<ActionResult<LocationDto>> GetLocationById(int id)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Fetching location {id} for operator {operatorId}");

                var location = await _locationService.GetLocationByIdAsync(id);

                if (location.OperatorId != operatorId)
                {
                    return Forbid();
                }

                return Ok(location);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching location: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching the location" });
            }
        }

        /// <summary>
        /// Update a location (must belong to current operator)
        /// </summary>
        [HttpPut("locations/{id}")]
        public async Task<ActionResult<LocationDto>> UpdateLocation(int id, [FromBody] UpdateLocationDto updateLocationDto)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Updating location {id} for operator {operatorId}");

                var existingLocation = await _locationService.GetLocationByIdAsync(id);

                if (existingLocation.OperatorId != operatorId)
                {
                    return Forbid();
                }

                var updatedLocation = await _locationService.UpdateLocationAsync(id, updateLocationDto);
                return Ok(updatedLocation);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating location: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the location" });
            }
        }

        /// <summary>
        /// Delete a location (must belong to current operator)
        /// </summary>
        [HttpDelete("locations/{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Deleting location {id} for operator {operatorId}");

                var existingLocation = await _locationService.GetLocationByIdAsync(id);

                if (existingLocation.OperatorId != operatorId)
                {
                    return Forbid();
                }

                var result = await _locationService.DeleteLocationAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Location with ID {id} deleted successfully" });
                }
                return StatusCode(500, new { message = "Failed to delete location" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting location: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the location" });
            }
        }
    }
}
