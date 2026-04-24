namespace BusBookingAPI.Services
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(1); // Run every minute

        public ReservationCleanupService(IServiceProvider serviceProvider, ILogger<ReservationCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Cleanup Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredReservations();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in reservation cleanup: {ex.Message}");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Reservation Cleanup Service stopped");
        }

        private async Task CleanupExpiredReservations()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                await bookingService.CleanupExpiredReservationsAsync();
            }
        }
    }
}
