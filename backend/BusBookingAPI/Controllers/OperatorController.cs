using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperatorController : ControllerBase
    {
        private readonly IOperatorService _operatorService;
        private readonly ILogger<OperatorController> _logger;

        public OperatorController(IOperatorService operatorService, ILogger<OperatorController> logger)
        {
            _operatorService = operatorService;
            _logger = logger;
        }

        /// <summary>
        /// Get an operator by ID
        /// </summary>
        /// <param name="id">Operator ID</param>
        /// <returns>Operator details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<OperatorDto>> GetOperator(int id)
        {
            try
            {
                _logger.LogInformation($"Getting operator with ID {id}");
                var op = await _operatorService.GetOperatorByIdAsync(id);
                return Ok(op);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Operator not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving operator: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the operator" });
            }
        }

        /// <summary>
        /// Get all operators
        /// </summary>
        /// <returns>List of all operators</returns>
        [HttpGet]
        public async Task<ActionResult<List<OperatorDto>>> GetAllOperators()
        {
            try
            {
                _logger.LogInformation("Getting all operators");
                var operators = await _operatorService.GetAllOperatorsAsync();
                return Ok(operators);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving operators: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving operators" });
            }
        }

        /// <summary>
        /// Create a new operator
        /// </summary>
        /// <param name="createOperatorDto">Operator details</param>
        /// <returns>Created operator</returns>
        [HttpPost]
        public async Task<ActionResult<OperatorDto>> CreateOperator(CreateOperatorDto createOperatorDto)
        {
            try
            {
                _logger.LogInformation("Creating new operator");
                var op = await _operatorService.CreateOperatorAsync(createOperatorDto);
                return CreatedAtAction(nameof(GetOperator), new { id = op.Id }, op);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid operator data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating operator: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the operator" });
            }
        }

        /// <summary>
        /// Update an existing operator
        /// </summary>
        /// <param name="id">Operator ID</param>
        /// <param name="updateOperatorDto">Updated operator details</param>
        /// <returns>Updated operator</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<OperatorDto>> UpdateOperator(int id, UpdateOperatorDto updateOperatorDto)
        {
            try
            {
                _logger.LogInformation($"Updating operator with ID {id}");
                var op = await _operatorService.UpdateOperatorAsync(id, updateOperatorDto);
                return Ok(op);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Operator not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid operator data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating operator: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the operator" });
            }
        }

        /// <summary>
        /// Delete an operator
        /// </summary>
        /// <param name="id">Operator ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOperator(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting operator with ID {id}");
                var result = await _operatorService.DeleteOperatorAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Operator with ID {id} deleted successfully" });
                }
                return StatusCode(500, new { message = "Failed to delete operator" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Operator not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting operator: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the operator" });
            }
        }
    }
}
