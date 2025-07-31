using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using PowerMeasure.Data;
using PowerMeasure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerMeasure.BackgroundService
{
    public class BackgroundMeasurmentService : IHostedService
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _iLogger;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;

        private const int MaxCurrent = 50;
        private const int MaxVoltage = 220;
        private const int MaxPower = 3000;
        private const int DefaultEnergyMeterId = 1;

        protected string Schedule => "*/2 * * * *";

        public BackgroundMeasurmentService(IServiceScopeFactory scopeFactory, ILogger<BackgroundMeasurmentService> iLogger)
        {
            _scopeFactory = scopeFactory;
            _iLogger = iLogger;
            _schedule = CrontabSchedule.Parse(Schedule);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                                                          cancellationToken));
            }
        }

        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                var nextrun = _schedule.GetNextOccurrence(now);

                if (now > _nextRun)
                {
                    await Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }

                await Task.Delay(5000, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        protected Task Process()
        {
            return Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<PowerMeasureDbContext>();

                var energyData = GenerateEnergyConsumed();

                await dbContext.Consumption.AddAsync(energyData);
                await dbContext.SaveChangesAsync();

                _iLogger.LogInformation("Energy consumption entry saved.");
            });
        }

        private EnergyConsumed GenerateEnergyConsumed()
        {
            var random = new Random();
            return new EnergyConsumed
            {
                Current = random.NextDouble() * MaxCurrent,
                Voltage = random.NextDouble() * MaxVoltage,
                Power = random.NextDouble() * MaxPower,
                IsDailyFinal = false,
                ReportDate = DateTime.Now,
                EnergyMeterId = DefaultEnergyMeterId
            };
        }
    }
}
