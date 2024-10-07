using Thunders.Api.Monitoring.Dtos;
using Thunders.Api.Monitoring.Enums;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Thunders.Api.Monitoring
{
    public class SqlConnectionHealthCheck(string? connectionString, string testQuery) : IHealthCheck
    {
        private static readonly string DefaultTestQuery = "Select 1";

        public string ConnectionString { get; } = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        public string TestQuery { get; } = testQuery;

        public SqlConnectionHealthCheck(string? connectionString)
            : this(connectionString, testQuery: DefaultTestQuery)
        {
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var healthCheckData = new Dictionary<string, object>();

            var healthCheckAggregates = new HealthCheckAggregatesDTO("DB Check", HealthCheckTypeEnum.TrueFalse, 0);

            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);

                    if (!string.IsNullOrWhiteSpace(TestQuery))
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = TestQuery;

                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
                catch (DbException)
                {
                    healthCheckData.Add("DB Check", healthCheckAggregates);
                    return new HealthCheckResult(status: context.Registration.FailureStatus, data: healthCheckData);
                }
            }
            healthCheckAggregates.Data = 1;
            healthCheckData.Add("DB Check", healthCheckAggregates);
            return new HealthCheckResult(HealthStatus.Healthy, "Service is UP", null, healthCheckData);
        }
    }
}
