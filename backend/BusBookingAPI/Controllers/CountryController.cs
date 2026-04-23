using Microsoft.AspNetCore.Mvc;
using BusBookingAPI.DTOs;
using BusBookingAPI.Services;

namespace BusBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryService countryService, ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        /// <summary>
        /// Get a country by ID
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <returns>Country details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            try
            {
                _logger.LogInformation($"Getting country with ID {id}");
                var country = await _countryService.GetCountryByIdAsync(id);
                return Ok(country);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Country not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving country: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving the country" });
            }
        }

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns>List of all countries</returns>
        [HttpGet]
        public async Task<ActionResult<List<CountryDto>>> GetAllCountries()
        {
            try
            {
                _logger.LogInformation("Getting all countries");
                var countries = await _countryService.GetAllCountriesAsync();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving countries: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while retrieving countries" });
            }
        }

        /// <summary>
        /// Create a new country
        /// </summary>
        /// <param name="createCountryDto">Country data to create</param>
        /// <returns>Created country details</returns>
        [HttpPost]
        public async Task<ActionResult<CountryDto>> CreateCountry(CreateCountryDto createCountryDto)
        {
            try
            {
                _logger.LogInformation($"Creating new country: {createCountryDto.CountryName}");
                var country = await _countryService.CreateCountryAsync(createCountryDto);
                return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating country: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the country" });
            }
        }

        /// <summary>
        /// Update an existing country
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <param name="updateCountryDto">Updated country data</param>
        /// <returns>Updated country details</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<CountryDto>> UpdateCountry(int id, UpdateCountryDto updateCountryDto)
        {
            try
            {
                _logger.LogInformation($"Updating country with ID {id}");
                var country = await _countryService.UpdateCountryAsync(id, updateCountryDto);
                return Ok(country);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Country not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating country: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the country" });
            }
        }

        /// <summary>
        /// Delete a country
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCountry(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting country with ID {id}");
                var result = await _countryService.DeleteCountryAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Country with ID {id} deleted successfully" });
                }
                return StatusCode(500, new { message = "Failed to delete country" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Country not found: {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting country: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the country" });
            }
        }
    }
}
