using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILocationService locationService, ILogger<LocationController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        /// <summary>
        /// Get a location by ID
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <returns>Location details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetLocation(int id)
        {
            try
            {
                _logger.LogInformation($"Getting location with ID {id}");
                var location = await _locationService.GetLocationByIdAsync(id);
                return Ok(location);
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
        /// Get all locations
        /// </summary>
        /// <returns>List of all locations</returns>
        [HttpGet]
        public async Task<ActionResult<List<LocationDto>>> GetAllLocations()
        {
            try
            {
                _logger.LogInformation("Getting all locations");
                var locations = await _locationService.GetAllLocationsAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving locations: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving locations" });
            }
        }

        /// <summary>
        /// Create a new location
        /// </summary>
        /// <param name="createLocationDto">Location details</param>
        /// <returns>Created location</returns>
        [HttpPost]
        public async Task<ActionResult<LocationDto>> CreateLocation(CreateLocationDto createLocationDto)
        {
            try
            {
                _logger.LogInformation("Creating new location");
                var location = await _locationService.CreateLocationAsync(createLocationDto);
                return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
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
        /// Update an existing location
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <param name="updateLocationDto">Updated location details</param>
        /// <returns>Updated location</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<LocationDto>> UpdateLocation(int id, UpdateLocationDto updateLocationDto)
        {
            try
            {
                _logger.LogInformation($"Updating location with ID {id}");
                var location = await _locationService.UpdateLocationAsync(id, updateLocationDto);
                return Ok(location);
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
        /// Delete a location
        /// </summary>
        /// <param name="id">Location ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLocation(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting location with ID {id}");
                var result = await _locationService.DeleteLocationAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Location with ID {id} deleted successfully" });
                }
                return StatusCode(500, new { message = "Failed to delete location" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Location not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
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
