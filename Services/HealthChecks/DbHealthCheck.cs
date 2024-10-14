using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using SP_000.Data;

namespace SP_000.Services.HealthChecks
{
    public class DbHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _ctx;

        public DbHealthCheck(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
            )
        {
            if (CheckDatabaseHealth().IsHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            else
            {
                return Task.FromResult(
                    new HealthCheckResult(
                        context.Registration.FailureStatus,
                        CheckDatabaseHealth().ErrorMessage
                    )
                );
            }
        }

        private (bool IsHealthy, string? ErrorMessage) CheckDatabaseHealth()
        {
            try
            {
                _ctx.Database.OpenConnection();
                _ctx.Database.CloseConnection();
                return (true, null);
            }
            catch (Exception e)
            {
                return (false, $"Database Connection Error: {e.Message}");
            }
        }
    }
}