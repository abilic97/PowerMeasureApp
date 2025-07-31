using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using NCrontab;
using PowerMeasure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PowerMeasure.BackgroundService.Helper;
using System.Text.Json;
using System.Text;
using PowerMeasure.Models;

namespace PowerMeasure.BackgroundService.Mqt
{
    public class MqttBroker : IHostedService
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

        public MqttBroker(IServiceScopeFactory scopeFactory, ILogger<MqttBroker> iLogger)
        {
            _scopeFactory = scopeFactory;
            _iLogger = iLogger;

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

                await Process();

                await Task.Delay(5000, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }
        protected async Task Process()
        {
            var current = (String)null;
            var energy = (String)null;
            var voltage = (String)null;
            var factory = new MqttFactory();
            IMqttClient mqttClient = factory.CreateMqttClient();
            var clientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("test.mosquitto.org", 1883)
                .WithCleanSession()
                .Build();
            mqttClient.ApplicationMessageReceivedAsync += e =>
           {
               var output = "NULL";
               if (e != null)

               {
                   output = JsonSerializer.Serialize(e, new JsonSerializerOptions
                   {
                       WriteIndented = true
                   });
               }
               if (e.ApplicationMessage.Topic == "netio/PowerCable-BE/output/1/current")
               {
                   current = e.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(e.ApplicationMessage?.Payload);
               }
               if (e.ApplicationMessage.Topic == "netio/PowerCable-BE/output/1/voltage")
               {
                   voltage = e.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(e.ApplicationMessage?.Payload);
               }
               if (e.ApplicationMessage.Topic == "netio/PowerCable-BE/output/1/energy")
               {
                   energy = e.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(e.ApplicationMessage?.Payload);
               }
               if (current != null && voltage != null && energy != null)
               {
                   using (var scope = _scopeFactory.CreateScope())
                   {

                       _powerMeasureDbContext = scope.ServiceProvider.GetService<PowerMeasureDbContext>();
                       EnergyConsumed ec = new EnergyConsumed();
                       ec.Current = Convert.ToDouble(current);
                       ec.Voltage = Convert.ToDouble(voltage);
                       ec.Power = Convert.ToDouble(energy);
                       ec.IsDailyFinal = false;
                       ec.ReportDate = DateTime.Now;
                       ec.EnergyMeterId = 10;
                       _powerMeasureDbContext.Consumption.AddAsync(ec);

                       _powerMeasureDbContext.SaveChangesAsync();
                       _iLogger.LogInformation($"saved to db ");


                   }

               }

               return Task.CompletedTask;
           };
            await mqttClient.ConnectAsync(clientOptions, CancellationToken.None);

            var mqttSubscribeOptions = factory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic("netio/#"); })
                .Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        }
    }
}
