using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ILogger<RouteController> _logger;

        public RouteController(IRouteService routeService, ILogger<RouteController> logger)
        {
            _routeService = routeService;
            _logger = logger;
        }

        /// <summary>
        /// Get a route by ID
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <returns>Route details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RouteDto>> GetRoute(int id)
        {
            try
            {
                _logger.LogInformation($"Getting route with ID {id}");
                var route = await _routeService.GetRouteByIdAsync(id);
                return Ok(route);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Route not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving route: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the route" });
            }
        }

        /// <summary>
        /// Get route details with district names by ID
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <returns>Route details with district names</returns>
        [HttpGet("{id}/details")]
        public async Task<ActionResult<RouteDetailDto>> GetRouteDetails(int id)
        {
            try
            {
                _logger.LogInformation($"Getting route details with ID {id}");
                var route = await _routeService.GetRouteDetailByIdAsync(id);
                return Ok(route);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Route not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving route details: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the route details" });
            }
        }

        /// <summary>
        /// Get all routes
        /// </summary>
        /// <returns>List of all routes</returns>
        [HttpGet]
        public async Task<ActionResult<List<RouteDto>>> GetAllRoutes()
        {
            try
            {
                _logger.LogInformation("Getting all routes");
                var routes = await _routeService.GetAllRoutesAsync();
                return Ok(routes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving routes: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving routes" });
            }
        }

        /// <summary>
        /// Get all routes with district details
        /// </summary>
        /// <returns>List of all routes with district names</returns>
        [HttpGet("details/all")]
        public async Task<ActionResult<List<RouteDetailDto>>> GetAllRouteDetails()
        {
            try
            {
                _logger.LogInformation("Getting all route details");
                var routes = await _routeService.GetAllRouteDetailsAsync();
                return Ok(routes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving route details: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving route details" });
            }
        }

        /// <summary>
        /// Create a new route
        /// </summary>
        /// <param name="createRouteDto">Route details</param>
        /// <returns>Created route</returns>
        [HttpPost]
        public async Task<ActionResult<RouteDto>> CreateRoute(CreateRouteDto createRouteDto)
        {
            try
            {
                _logger.LogInformation("Creating new route");
                var route = await _routeService.CreateRouteAsync(createRouteDto);
                return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid route data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating route: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the route" });
            }
        }

        /// <summary>
        /// Update an existing route
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <param name="updateRouteDto">Updated route details</param>
        /// <returns>Updated route</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<RouteDto>> UpdateRoute(int id, UpdateRouteDto updateRouteDto)
        {
            try
            {
                _logger.LogInformation($"Updating route with ID {id}");
                var route = await _routeService.UpdateRouteAsync(id, updateRouteDto);
                return Ok(route);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Route not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid route data: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating route: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the route" });
            }
        }

        /// <summary>
        /// Delete a route
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRoute(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting route with ID {id}");
                var result = await _routeService.DeleteRouteAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Route with ID {id} deleted successfully" });
                }
                return StatusCode(500, new { message = "Failed to delete route" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Route not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting route: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the route" });
            }
        }
    }
}
