using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;
using System.Security.Claims;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/operator-dashboard")]
    public class OperatorDashboardController : ControllerBase
    {
        private readonly IOperatorDashboardService _operatorDashboardService;
        private readonly ILogger<OperatorDashboardController> _logger;

        public OperatorDashboardController(IOperatorDashboardService operatorDashboardService, ILogger<OperatorDashboardController> logger)
        {
            _operatorDashboardService = operatorDashboardService;
            _logger = logger;
        }

        private int GetOperatorIdFromToken()
        {
            var operatorIdClaim = User.FindFirst("operatorId");
            if (operatorIdClaim == null || !int.TryParse(operatorIdClaim.Value, out var operatorId))
            {
                throw new UnauthorizedAccessException("Invalid operator token");
            }
            return operatorId;
        }

        /// <summary>
        /// Get all locations for the logged-in operator
        /// </summary>
        [Authorize]
        [HttpGet("locations")]
        public async Task<ActionResult<List<LocationDto>>> GetOperatorLocations()
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Getting locations for operator {operatorId}");
                var locations = await _operatorDashboardService.GetOperatorLocationsAsync(operatorId);
                return Ok(locations);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access: {ex.Message}");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving locations: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving locations" });
            }
        }

        /// <summary>
        /// Get a specific location by ID (must belong to logged-in operator)
        /// </summary>
        [Authorize]
        [HttpGet("locations/{id}")]
        public async Task<ActionResult<LocationDto>> GetLocationById(int id)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Getting location {id} for operator {operatorId}");
                var location = await _operatorDashboardService.GetLocationByIdAsync(id, operatorId);
                return Ok(location);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access: {ex.Message}");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Location not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving location: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the location" });
            }
        }

        /// <summary>
        /// Create a new location for the logged-in operator
        /// </summary>
        [Authorize]
        [HttpPost("locations")]
        public async Task<ActionResult<LocationDto>> CreateLocation([FromBody] CreateLocationDto createLocationDto)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Creating location for operator {operatorId}");
                var location = await _operatorDashboardService.CreateLocationAsync(createLocationDto, operatorId);
                return CreatedAtAction(nameof(GetLocationById), new { id = location.Id }, location);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid location data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating location: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the location" });
            }
        }

        /// <summary>
        /// Update an existing location (must belong to logged-in operator)
        /// </summary>
        [Authorize]
        [HttpPut("locations/{id}")]
        public async Task<ActionResult<LocationDto>> UpdateLocation(int id, [FromBody] UpdateLocationDto updateLocationDto)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Updating location {id} for operator {operatorId}");
                var location = await _operatorDashboardService.UpdateLocationAsync(id, updateLocationDto, operatorId);
                return Ok(location);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access: {ex.Message}");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Location not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid location data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating location: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the location" });
            }
        }

        /// <summary>
        /// Delete a location (must belong to logged-in operator)
        /// </summary>
        [Authorize]
        [HttpDelete("locations/{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            try
            {
                var operatorId = GetOperatorIdFromToken();
                _logger.LogInformation($"Deleting location {id} for operator {operatorId}");
                await _operatorDashboardService.DeleteLocationAsync(id, operatorId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized access: {ex.Message}");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Location not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting location: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the location" });
            }
        }
    }
}
