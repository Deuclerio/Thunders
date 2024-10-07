using Thunders.Api.Monitoring.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Thunders.Api.Monitoring
{
    public static class StartupHealthCheck
    {
        private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

        public static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";

            var assembly = Assembly.GetExecutingAssembly();
            var fileInfo = new FileInfo(assembly.Location);
            var lastModified = fileInfo.LastWriteTime.ToUniversalTime();

            var healthCheck = new HealthCheckDTO()
            {
                Result = result.Status,
                RefreshDate = DateTime.UtcNow,
                ReleaseDate = lastModified,
                Aggregates = result.Entries.Values.Select(v => v.Data).SelectMany(d => d.Values).Cast<HealthCheckAggregatesDTO>().ToList()
            };

            var json = JsonSerializer.Serialize(healthCheck, _options);

            return context.Response.WriteAsync(json);
        }
    }
}
