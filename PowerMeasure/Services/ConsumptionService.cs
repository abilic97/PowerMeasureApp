using Microsoft.EntityFrameworkCore;
using PowerMeasure.Data;
using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using PowerMeasure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Services
{
    public class ConsumptionService : IConsumptionService
    {
        private PowerMeasureDbContext _powerMeasureDbContext;
        private double pricePerkWh = 0.5295;
        private double transitPerKwh = 0.0900;
        private double distributionPerKwh = 0.2200;
        private double pdv = 0.13;
        private double supplyPerMonth = 7.40;
        private double measuringPlacePerMonth = 11.60;
        private double oiePerkwH = 0.1050;

        public ConsumptionService(PowerMeasureDbContext context)
        {
            _powerMeasureDbContext = context;

        }

        public async Task<IEnumerable<PowerOutages>> calculatePowerOutagesPerMonthAsync(int userId)
        {
            var powerOutageReadings = GetPowerOutageReadings(userId);

            var monthlyOutages = await powerOutageReadings
                .GroupBy(r => r.ReportDate.Month)
                .Select(group => new PowerOutages
                {
                    count = group.Count(),
                    date = group.Key
                })
                .ToListAsync();

            return monthlyOutages;
        }

        public async Task<double> getTotalCostPerDay(int userId, DateTime date)
        {
            Measurements m = getTotalConsumedEnergyValuePerDay(userId, date);
            double finalPrice = (pricePerkWh * pdv) + pricePerkWh;

            double dailyCost = (m.P) * finalPrice;
            return dailyCost;
        }

        public async Task<IEnumerable<MonthlyCost>> getConsumedEnergyForUserPerMonth(int userId)
        {
            Measurements m = new Measurements();
            var energyConsumed = (from u in _powerMeasureDbContext.Users
                                  join uc in _powerMeasureDbContext.Contracts on u.Id equals uc.UsersId
                                  join e in _powerMeasureDbContext.Meter on uc.Id equals e.UserContractRef
                                  join ec in _powerMeasureDbContext.Consumption on e.Id equals ec.EnergyMeterId
                                  where u.Id == userId && e.IndividualDevice == false
                                  select ec);
            var ddist = energyConsumed.Select(r => r.ReportDate.Date).Distinct().ToList().Select(r => new DateMonth
            {
                Dates = r.Date,
                power = getTotalConsumedEnergyValuePerDay(userId, r.Date).P
            }).GroupBy(x => x.Dates.Month).Select(g => new MonthlyCost
            {
                month = g.Key,
                sum = g.Sum(m => m.power)
            }).ToList();

            foreach (var value in ddist)
            {
                value.cost = calculateMonthlyCost(value.sum);
            }
            return ddist.OrderBy(x => x.month);
        }

        public Measurements getTotalConsumedEnergyValuePerDay(int userId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1).AddTicks(-1);

            var dailyMeasurements = GetDailyMeasurements(userId, startDate, endDate);

            return CalculateTotalDailyConsumption(dailyMeasurements);
        }

        public EnergyConsumed getLastEnergyValueDayIndividual(int userId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            return GetIndividualDeviceReadings(userId, startOfDay, endOfDay)
                .OrderByDescending(reading => reading.Id)
                .FirstOrDefault();
        }

        public EnergyConsumed getLastEnergyValueDay(int userId, DateTime date)
        {
            DateTime startDateTime = date;
            DateTime endDateTime = date.AddDays(1).AddTicks(-1);
            var lastPowerValue = (from u in _powerMeasureDbContext.Users
                                  join uc in _powerMeasureDbContext.Contracts on u.Id equals uc.UsersId
                                  join em in _powerMeasureDbContext.Meter on uc.Id equals em.UserContractRef
                                  join ec in _powerMeasureDbContext.Consumption on em.Id equals ec.EnergyMeterId
                                  where ec.ReportDate >= startDateTime && ec.ReportDate <= endDateTime && u.Id == userId
                                  && em.IndividualDevice == false
                                  orderby ec.Id descending
                                  select ec);

            return lastPowerValue.FirstOrDefault();
        }

        public async Task<IEnumerable<EnergyConsumed>> getAllConsumedEnergyValuePerDayIndividual(int userId, DateTime date)
        {
            DateTime startDateTime = date;
            DateTime endDateTime = date.AddDays(1).AddTicks(-1);
            var allMeasurmentValues = (from u in _powerMeasureDbContext.Users
                                       join uc in _powerMeasureDbContext.Contracts on u.Id equals uc.UsersId
                                       join em in _powerMeasureDbContext.Meter on uc.Id equals em.UserContractRef
                                       join ec in _powerMeasureDbContext.Consumption on em.Id equals ec.EnergyMeterId
                                       where ec.ReportDate >= startDateTime && ec.ReportDate <= endDateTime && u.Id == userId
                                       && em.IndividualDevice == true
                                       orderby ec.Id descending
                                       select ec);
            return await allMeasurmentValues.ToListAsync();

        }

        private double calculateMonthlyCost(double kwhvalue)
        {
            double mothlyCostwoPdv = (kwhvalue * pricePerkWh) + (kwhvalue * transitPerKwh) +
                (kwhvalue * distributionPerKwh) + supplyPerMonth + measuringPlacePerMonth +
                (kwhvalue * oiePerkwH);

            double totalMonthlyCost = mothlyCostwoPdv + (mothlyCostwoPdv * pdv);

            return totalMonthlyCost;
        }

        private IQueryable<EnergyConsumed> GetPowerOutageReadings(int userId)
        {
            return from user in _powerMeasureDbContext.Users
                   join contract in _powerMeasureDbContext.Contracts on user.Id equals contract.UsersId
                   join meter in _powerMeasureDbContext.Meter on contract.Id equals meter.UserContractRef
                   join reading in _powerMeasureDbContext.Consumption on meter.Id equals reading.EnergyMeterId
                   where user.Id == userId &&
                         meter.IndividualDevice == false &&
                         reading.Power == 0 &&
                         reading.Current == 0 &&
                         reading.Voltage == 0
                   select reading;
        }

        private List<Measurements> GetDailyMeasurements(int userId, DateTime start, DateTime end)
        {
            return (from user in _powerMeasureDbContext.Users
                    join contract in _powerMeasureDbContext.Contracts on user.Id equals contract.UsersId
                    join meter in _powerMeasureDbContext.Meter on contract.Id equals meter.UserContractRef
                    join reading in _powerMeasureDbContext.Consumption on meter.Id equals reading.EnergyMeterId
                    where user.Id == userId &&
                          meter.IndividualDevice == false &&
                          reading.ReportDate >= start &&
                          reading.ReportDate <= end
                    select new Measurements
                    {
                        C = reading.Current,
                        V = reading.Voltage,
                        P = reading.Power
                    }).ToList();
        }

        private Measurements CalculateTotalDailyConsumption(IEnumerable<Measurements> readings)
        {
            if (!readings.Any())
                return new Measurements(); 

            const double IntervalMinutes = 2;
            const double ConversionFactor = IntervalMinutes / 60.0; // To convert to kWh

            double totalKWh = readings.Sum(r => (r.P / 1000.0) * ConversionFactor);
            double averageVoltage = readings.Average(r => r.V);
            double averageCurrent = readings.Average(r => r.C);

            return new Measurements
            {
                P = totalKWh,
                V = averageVoltage,
                C = averageCurrent
            };
        }

        private IQueryable<EnergyConsumed> GetIndividualDeviceReadings(int userId, DateTime start, DateTime end)
        {
            return from user in _powerMeasureDbContext.Users
                   join contract in _powerMeasureDbContext.Contracts on user.Id equals contract.UsersId
                   join meter in _powerMeasureDbContext.Meter on contract.Id equals meter.UserContractRef
                   join reading in _powerMeasureDbContext.Consumption on meter.Id equals reading.EnergyMeterId
                   where user.Id == userId
                         && meter.IndividualDevice
                         && reading.ReportDate >= start
                         && reading.ReportDate <= end
                   select reading;
        }
    }
}
