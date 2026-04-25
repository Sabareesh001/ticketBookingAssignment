using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;
        private readonly ILogger<StateController> _logger;

        public StateController(IStateService stateService, ILogger<StateController> logger)
        {
            _stateService = stateService;
            _logger = logger;
        }

        /// <summary>
        /// Get a state by ID
        /// </summary>
        /// <param name="id">State ID</param>
        /// <returns>State details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<StateDto>> GetState(int id)
        {
            try
            {
                _logger.LogInformation($"Getting state with ID {id}");
                var state = await _stateService.GetStateByIdAsync(id);
                return Ok(state);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"State not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving state: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the state" });
            }
        }

        /// <summary>
        /// Get all states
        /// </summary>
        /// <returns>List of all states</returns>
        [HttpGet]
        public async Task<ActionResult<List<StateDto>>> GetAllStates()
        {
            try
            {
                _logger.LogInformation("Getting all states");
                var states = await _stateService.GetAllStatesAsync();
                return Ok(states);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving states: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving states" });
            }
        }

        /// <summary>
        /// Get states by country ID
        /// </summary>
        /// <param name="countryId">Country ID</param>
        /// <returns>List of states for the specified country</returns>
        [HttpGet("country/{countryId}")]
        public async Task<ActionResult<List<StateDto>>> GetStatesByCountry(int countryId)
        {
            try
            {
                _logger.LogInformation($"Getting states for country ID {countryId}");
                var states = await _stateService.GetStatesByCountryIdAsync(countryId);
                return Ok(states);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Country not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving states: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving states" });
            }
        }

        /// <summary>
        /// Create a new state
        /// </summary>
        /// <param name="createStateDto">State data to create</param>
        /// <returns>Created state details</returns>
        [HttpPost]
        public async Task<ActionResult<StateDto>> CreateState(CreateStateDto createStateDto)
        {
            try
            {
                _logger.LogInformation($"Creating new state: {createStateDto.StateName}");
                var state = await _stateService.CreateStateAsync(createStateDto);
                return CreatedAtAction(nameof(GetState), new { id = state.Id }, state);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating state: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the state" });
            }
        }

        /// <summary>
        /// Update an existing state
        /// </summary>
        /// <param name="id">State ID</param>
        /// <param name="updateStateDto">Updated state data</param>
        /// <returns>Updated state details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<StateDto>> UpdateState(int id, UpdateStateDto updateStateDto)
        {
            try
            {
                _logger.LogInformation($"Updating state with ID {id}");
                var state = await _stateService.UpdateStateAsync(id, updateStateDto);
                return Ok(state);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"State not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating state: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the state" });
            }
        }

        /// <summary>
        /// Delete a state
        /// </summary>
        /// <param name="id">State ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteState(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting state with ID {id}");
                await _stateService.DeleteStateAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"State not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting state: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the state" });
            }
        }
    }
}
