using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;

namespace Thunders.Api.Monitoring.Dtos
{
    public class HealthCheckDTO
    {
        public DateTime RefreshDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public HealthStatus Result { get; set; }
        public List<HealthCheckAggregatesDTO> Aggregates { get; set; } = [];
        public string Version { get; } = "0.0.5";
    }
}