using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/operator-auth")]
    public class OperatorAuthController : ControllerBase
    {
        private readonly IOperatorAuthService _operatorAuthService;
        private readonly ILogger<OperatorAuthController> _logger;

        public OperatorAuthController(IOperatorAuthService operatorAuthService, ILogger<OperatorAuthController> logger)
        {
            _operatorAuthService = operatorAuthService;
            _logger = logger;
        }

        /// <summary>
        /// Operator login
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Auth token and operator details</returns>
        [HttpPost("login")]
        public async Task<ActionResult<OperatorAuthResponse>> Login([FromBody] OperatorLoginRequest request)
        {
            try
            {
                _logger.LogInformation($"Operator login request for email: {request.Email}");
                var response = await _operatorAuthService.LoginAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid login request: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized login attempt: {ex.Message}");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Operator signup
        /// </summary>
        /// <param name="request">Signup details</param>
        /// <returns>Auth token and operator details</returns>
        [HttpPost("signup")]
        public async Task<ActionResult<OperatorAuthResponse>> Signup([FromBody] OperatorSignupRequest request)
        {
            try
            {
                _logger.LogInformation($"Operator signup request for email: {request.Email}");
                var response = await _operatorAuthService.SignupAsync(request);
                return CreatedAtAction(nameof(Signup), response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid signup request: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Signup error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred during signup" });
            }
        }
    }
}
