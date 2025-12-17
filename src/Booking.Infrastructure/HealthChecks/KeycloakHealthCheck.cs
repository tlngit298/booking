using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Booking.Infrastructure.HealthChecks;

public class KeycloakHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public KeycloakHealthCheck(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var keycloakUrl = _configuration["Keycloak:Url"];
        var realm = _configuration["Keycloak:Realm"];
        if (string.IsNullOrEmpty(keycloakUrl) || string.IsNullOrEmpty(realm))
        {
            return HealthCheckResult.Unhealthy("Keycloak URL or Realm is not configured.");
        }

        try
        {
            var wellKnown = $"{keycloakUrl}/realms/{realm}/.well-known/openid-configuration";
            var response = await _httpClient.GetAsync(wellKnown, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Unhealthy($"Keycloak is unreachable. Status code: {response.StatusCode}");
            }
            return HealthCheckResult.Healthy("Keycloak is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Keycloak health check failed.", ex);
        }
    }
}