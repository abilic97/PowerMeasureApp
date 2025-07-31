using Microsoft.EntityFrameworkCore;
using PowerMeasure.Data;
using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
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

        private double calculateMonthlyCost(double kwhvalue)
        {
            double mothlyCostwoPdv = (kwhvalue * pricePerkWh) + (kwhvalue * transitPerKwh) +
                (kwhvalue * distributionPerKwh) + supplyPerMonth + measuringPlacePerMonth +
                (kwhvalue * oiePerkwH);

            double totalMonthlyCost = mothlyCostwoPdv + (mothlyCostwoPdv * pdv);

            return totalMonthlyCost;
        }

        public async Task<IEnumerable<PowerOutages>> calculatePowerOutagesPerMonth(int userId)
        {
            var energyConsumed = (from u in _powerMeasureDbContext.Users
                                  join uc in _powerMeasureDbContext.Contracts on u.Id equals uc.UsersId
                                  join e in _powerMeasureDbContext.Meter on uc.Id equals e.UserContractRef
                                  join ec in _powerMeasureDbContext.Consumption on e.Id equals ec.EnergyMeterId
                                  where ec.Power == 0 && ec.Current == 0 && ec.Voltage == 0 &&
                                  u.Id == userId && e.IndividualDevice == false
                                  select ec); ;

            var thisss = await energyConsumed.GroupBy(x => x.ReportDate.Month).Select(x => new PowerOutages
            {
                count = x.Count(),
                date = x.Key
            }).ToListAsync();

            return thisss;
        }

        public Task<IEnumerable<double>> calculateUserCostPerMonth(int userId)
        {
            throw new NotImplementedException();
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
                sum = g.Sum(m=> m.power)
            }).ToList();

            foreach (var value in ddist) {
                value.cost = calculateMonthlyCost(value.sum);
            }
            return ddist.OrderBy(x=> x.month);
        }

        public Measurements getTotalConsumedEnergyValuePerDay(int userId, DateTime date)
        {
            DateTime startDateTime = date;
            DateTime endDateTime = date.AddDays(1).AddTicks(-1);
            double intermediateSumkWh = 0;
            double intermediateSumV = 0;
            double intermediateSUmC = 0;
            var energyConsumed = (from u in _powerMeasureDbContext.Users
                                  join uc in _powerMeasureDbContext.Contracts on u.Id equals uc.UsersId
                                  join e in _powerMeasureDbContext.Meter on uc.Id equals e.UserContractRef
                                  join ec in _powerMeasureDbContext.Consumption on e.Id equals ec.EnergyMeterId
                                  where ec.ReportDate >= startDateTime && ec.ReportDate <= endDateTime &&
                                  u.Id == userId && e.IndividualDevice == false
                                  select new Measurements
                                  {
                                      C = ec.Current,
                                      V = ec.Voltage,
                                      P = ec.Power
                                  }).ToList();

            energyConsumed.ForEach(value =>
            {
                intermediateSumkWh += (value.P / 1000) * 2;
                intermediateSumV = (value.V);
                intermediateSUmC = (value.C);
            });

            Measurements m = new Measurements();
            m.P = intermediateSumkWh / 60;
            m.C = intermediateSUmC;
            m.V = intermediateSumV;
            return m;
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

        public async Task<double> getTotalCostPerDay(int userId, DateTime date)
        {
            Measurements m =  getTotalConsumedEnergyValuePerDay(userId, date);
            double finalPrice = (pricePerkWh * pdv) + pricePerkWh;

            double dailyCost = (m.P) * finalPrice;
            return dailyCost;

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
        public EnergyConsumed getLastEnergyValueDayIndividual(int userId, DateTime date) 
        {
            DateTime startDateTime = date;
            DateTime endDateTime = date.AddDays(1).AddTicks(-1);
            var lastPowerValue = (from u in _powerMeasureDbContext.Users
                                  join uc in _powerMeasureDbContext.Contracts on u.Id equals uc.UsersId
                                  join em in _powerMeasureDbContext.Meter on uc.Id equals em.UserContractRef
                                  join ec in _powerMeasureDbContext.Consumption on em.Id equals ec.EnergyMeterId
                                  where ec.ReportDate >= startDateTime && ec.ReportDate <= endDateTime && u.Id == userId
                                  && em.IndividualDevice == true
                                  orderby ec.Id descending
                                  select ec);

            return lastPowerValue.FirstOrDefault();
        }
    }
}
