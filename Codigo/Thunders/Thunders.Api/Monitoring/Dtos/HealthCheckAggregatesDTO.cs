using Thunders.Api.Monitoring.Enums;

namespace Thunders.Api.Monitoring.Dtos
{
    public class HealthCheckAggregatesDTO(string name, HealthCheckTypeEnum type, int data)
    {
        public string Name { get; set; } = name;
        public HealthCheckTypeEnum Type { get; set; } = type;
        public int Data { get; set; } = data;
        public int Limit1 { get; set; }
        public int Limit2 { get; set; }
    }
}
