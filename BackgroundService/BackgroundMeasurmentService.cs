using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using PowerMeasure.Data;
using PowerMeasure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PowerMeasure.BackgroundService
{
    public class BackgroundMeasurmentService : IHostedService
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts =
                                                       new CancellationTokenSource();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _iLogger;
        private PowerMeasureDbContext _powerMeasureDbContext;
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
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
            // store task
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // return if running
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // running
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
                // Signal cancellation to the executing method
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
            int rangeC = 50;
            int rangeV = 220;
            int rangeP = 3000;
            int rangeM = 10;
            Random r = new Random();
            return Task.Run(async () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    _powerMeasureDbContext = scope.ServiceProvider.GetService<PowerMeasureDbContext>();
                    EnergyConsumed ec = new EnergyConsumed();
                    ec.Current = r.NextDouble() * rangeC;
                    ec.Voltage = r.NextDouble() * rangeV;
                    ec.Power = r.NextDouble() * rangeP;
                    ec.IsDailyFinal = false;
                    ec.ReportDate = DateTime.Now;
                    ec.EnergyMeterId = 1;

                    await _powerMeasureDbContext.Consumption.AddAsync(ec);

                    await _powerMeasureDbContext.SaveChangesAsync();
                    _iLogger.LogInformation($"tets ");
                }
            });
        }
    }
}
