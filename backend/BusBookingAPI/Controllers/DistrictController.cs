using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _districtService;
        private readonly ILogger<DistrictController> _logger;

        public DistrictController(IDistrictService districtService, ILogger<DistrictController> logger)
        {
            _districtService = districtService;
            _logger = logger;
        }

        /// <summary>
        /// Get a district by ID
        /// </summary>
        /// <param name="id">District ID</param>
        /// <returns>District details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DistrictDto>> GetDistrict(int id)
        {
            try
            {
                _logger.LogInformation($"Getting district with ID {id}");
                var district = await _districtService.GetDistrictByIdAsync(id);
                return Ok(district);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"District not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving district: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the district" });
            }
        }

        /// <summary>
        /// Get all districts
        /// </summary>
        /// <returns>List of all districts</returns>
        [HttpGet]
        public async Task<ActionResult<List<DistrictDto>>> GetAllDistricts()
        {
            try
            {
                _logger.LogInformation("Getting all districts");
                var districts = await _districtService.GetAllDistrictsAsync();
                return Ok(districts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving districts: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving districts" });
            }
        }

        /// <summary>
        /// Get all districts by state ID
        /// </summary>
        /// <param name="stateId">State ID</param>
        /// <returns>List of districts in the state</returns>
        [HttpGet("state/{stateId}")]
        public async Task<ActionResult<List<DistrictDto>>> GetDistrictsByState(int stateId)
        {
            try
            {
                _logger.LogInformation($"Getting all districts for state ID {stateId}");
                var districts = await _districtService.GetDistrictsByStateIdAsync(stateId);
                return Ok(districts);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"State not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving districts: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving districts" });
            }
        }

        /// <summary>
        /// Create a new district
        /// </summary>
        /// <param name="createDistrictDto">District data to create</param>
        /// <returns>Created district details</returns>
        [HttpPost]
        public async Task<ActionResult<DistrictDto>> CreateDistrict(CreateDistrictDto createDistrictDto)
        {
            try
            {
                _logger.LogInformation($"Creating new district: {createDistrictDto.DistrictName}");
                var district = await _districtService.CreateDistrictAsync(createDistrictDto);
                return CreatedAtAction(nameof(GetDistrict), new { id = district.Id }, district);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating district: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the district" });
            }
        }

        /// <summary>
        /// Update an existing district
        /// </summary>
        /// <param name="id">District ID</param>
        /// <param name="updateDistrictDto">Updated district data</param>
        /// <returns>Updated district details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<DistrictDto>> UpdateDistrict(int id, UpdateDistrictDto updateDistrictDto)
        {
            try
            {
                _logger.LogInformation($"Updating district with ID {id}");
                var district = await _districtService.UpdateDistrictAsync(id, updateDistrictDto);
                return Ok(district);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"District not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating district: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the district" });
            }
        }

        /// <summary>
        /// Delete a district
        /// </summary>
        /// <param name="id">District ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDistrict(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting district with ID {id}");
                await _districtService.DeleteDistrictAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"District not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting district: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the district" });
            }
        }
    }
}
