using BusBookingAPI.Data;
using BusBookingAPI.DTOs;
using BusBookingAPI.Models;
using Microsoft.EntityFrameworkCore;
using Route = BusBookingAPI.Models.Route;

namespace BusBookingAPI.Services
{
    public interface IRouteService
    {
        Task<RouteDto> GetRouteByIdAsync(int id);
        Task<List<RouteDto>> GetAllRoutesAsync();
        Task<RouteDetailDto> GetRouteDetailByIdAsync(int id);
        Task<List<RouteDetailDto>> GetAllRouteDetailsAsync();
        Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto);
        Task<RouteDto> UpdateRouteAsync(int id, UpdateRouteDto updateRouteDto);
        Task<bool> DeleteRouteAsync(int id);
    }

    public class RouteService : IRouteService
    {
        private readonly BusBookingDbContext _context;
        private readonly ILogger<RouteService> _logger;

        public RouteService(BusBookingDbContext context, ILogger<RouteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RouteDto> GetRouteByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching route with ID {id}");

            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                _logger.LogWarning($"Route with ID {id} not found");
                throw new KeyNotFoundException($"Route with ID {id} not found");
            }

            return MapToDto(route);
        }

        public async Task<List<RouteDto>> GetAllRoutesAsync()
        {
            _logger.LogInformation("Fetching all routes");

            var routes = await _context.Routes.ToListAsync();

            return routes.Select(MapToDto).ToList();
        }

        public async Task<RouteDetailDto> GetRouteDetailByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching route details with ID {id}");

            var route = await _context.Routes
                .Include(r => r.SourceLocation)
                .ThenInclude(l => l.District)
                .Include(r => r.DestinationLocation)
                .ThenInclude(l => l.District)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                _logger.LogWarning($"Route with ID {id} not found");
                throw new KeyNotFoundException($"Route with ID {id} not found");
            }

            return MapToDetailDto(route);
        }

        public async Task<List<RouteDetailDto>> GetAllRouteDetailsAsync()
        {
            _logger.LogInformation("Fetching all route details");

            var routes = await _context.Routes
                .Include(r => r.SourceLocation)
                .ThenInclude(l => l.District)
                .Include(r => r.DestinationLocation)
                .ThenInclude(l => l.District)
                .ToListAsync();

            return routes.Select(MapToDetailDto).ToList();
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto)
        {
            _logger.LogInformation($"Creating new route from location {createRouteDto.SourceLocationId} to {createRouteDto.DestinationLocationId}");

            // Validate that source and destination locations exist
            var sourceLocationExists = await _context.Locations.AnyAsync(l => l.Id == createRouteDto.SourceLocationId);
            if (!sourceLocationExists)
            {
                throw new ArgumentException($"Source location with ID {createRouteDto.SourceLocationId} not found");
            }

            var destinationLocationExists = await _context.Locations.AnyAsync(l => l.Id == createRouteDto.DestinationLocationId);
            if (!destinationLocationExists)
            {
                throw new ArgumentException($"Destination location with ID {createRouteDto.DestinationLocationId} not found");
            }

            // Validate that source and destination are not the same
            if (createRouteDto.SourceLocationId == createRouteDto.DestinationLocationId)
            {
                throw new ArgumentException("Source and destination locations cannot be the same");
            }

            var route = new Route
            {
                SourceLocationId = createRouteDto.SourceLocationId,
                DestinationLocationId = createRouteDto.DestinationLocationId,
                DistanceKm = createRouteDto.DistanceKm,
                EstimatedDurationHours = createRouteDto.EstimatedDurationHours,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Route created successfully with ID {route.Id}");

            return MapToDto(route);
        }

        public async Task<RouteDto> UpdateRouteAsync(int id, UpdateRouteDto updateRouteDto)
        {
            _logger.LogInformation($"Updating route with ID {id}");

            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                _logger.LogWarning($"Route with ID {id} not found");
                throw new KeyNotFoundException($"Route with ID {id} not found");
            }

            // Validate that source and destination locations exist
            var sourceLocationExists = await _context.Locations.AnyAsync(l => l.Id == updateRouteDto.SourceLocationId);
            if (!sourceLocationExists)
            {
                throw new ArgumentException($"Source location with ID {updateRouteDto.SourceLocationId} not found");
            }

            var destinationLocationExists = await _context.Locations.AnyAsync(l => l.Id == updateRouteDto.DestinationLocationId);
            if (!destinationLocationExists)
            {
                throw new ArgumentException($"Destination location with ID {updateRouteDto.DestinationLocationId} not found");
            }

            // Validate that source and destination are not the same
            if (updateRouteDto.SourceLocationId == updateRouteDto.DestinationLocationId)
            {
                throw new ArgumentException("Source and destination locations cannot be the same");
            }

            route.SourceLocationId = updateRouteDto.SourceLocationId;
            route.DestinationLocationId = updateRouteDto.DestinationLocationId;
            route.DistanceKm = updateRouteDto.DistanceKm;
            route.EstimatedDurationHours = updateRouteDto.EstimatedDurationHours;
            route.UpdatedAt = DateTime.UtcNow;

            _context.Routes.Update(route);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Route with ID {id} updated successfully");

            return MapToDto(route);
        }

        public async Task<bool> DeleteRouteAsync(int id)
        {
            _logger.LogInformation($"Deleting route with ID {id}");

            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                _logger.LogWarning($"Route with ID {id} not found");
                throw new KeyNotFoundException($"Route with ID {id} not found");
            }

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Route with ID {id} deleted successfully");

            return true;
        }

        private RouteDto MapToDto(Route route)
        {
            return new RouteDto
            {
                Id = route.Id,
                SourceLocationId = route.SourceLocationId,
                DestinationLocationId = route.DestinationLocationId,
                DistanceKm = route.DistanceKm,
                EstimatedDurationHours = route.EstimatedDurationHours,
                CreatedAt = route.CreatedAt,
                UpdatedAt = route.UpdatedAt
            };
        }

        private RouteDetailDto MapToDetailDto(Route route)
        {
            var sourceDistrictName = route.SourceLocation?.District?.DistrictName ?? "Unknown";
            var destDistrictName = route.DestinationLocation?.District?.DistrictName ?? "Unknown";

            return new RouteDetailDto
            {
                Id = route.Id,
                SourceLocationId = route.SourceLocationId,
                DestinationLocationId = route.DestinationLocationId,
                SourceDistrictName = sourceDistrictName,
                DestinationDistrictName = destDistrictName,
                DistanceKm = route.DistanceKm,
                EstimatedDurationHours = route.EstimatedDurationHours,
                CreatedAt = route.CreatedAt,
                UpdatedAt = route.UpdatedAt
            };
        }
    }
}
