using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IBusService _busService;
        private readonly ILogger<UserController> _logger;

        public UserController(IBusService busService, ILogger<UserController> logger)
        {
            _busService = busService;
            _logger = logger;
        }

        /// <summary>
        /// Get available buses for a given source and destination districts
        /// </summary>
        /// <param name="request">Request containing source and destination districts</param>
        /// <returns>List of available active buses for the route</returns>
        [HttpPost("available-buses")]
        public async Task<ActionResult<AvailableBusesResponse>> GetAvailableBuses([FromBody] AvailableBusesRequest request)
        {
            _logger.LogInformation($"Getting available buses from {request.SourceDistrict} to {request.DestinationDistrict}");

            var result = await _busService.GetAvailableBusesAsync(request.SourceDistrict, request.DestinationDistrict);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get available buses for a given source and destination districts (GET method)
        /// </summary>
        /// <param name="sourceDistrict">Source district name</param>
        /// <param name="destinationDistrict">Destination district name</param>
        /// <returns>List of available active buses for the route</returns>
        [HttpGet("available-buses")]
        public async Task<ActionResult<AvailableBusesResponse>> GetAvailableBusesGet(
            [FromQuery] string sourceDistrict,
            [FromQuery] string destinationDistrict)
        {
            _logger.LogInformation($"Getting available buses from {sourceDistrict} to {destinationDistrict}");

            var result = await _busService.GetAvailableBusesAsync(sourceDistrict, destinationDistrict);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
