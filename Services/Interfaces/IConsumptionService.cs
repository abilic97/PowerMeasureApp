using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerMeasure.Services.Interfaces
{
    public interface IConsumptionService
    {
        public Measurements getTotalConsumedEnergyValuePerDay(int userId, DateTime date);
        public EnergyConsumed getLastEnergyValueDay(int userId, DateTime date);
        public Task<IEnumerable<MonthlyCost>> getConsumedEnergyForUserPerMonth(int userId);
        public Task<IEnumerable<PowerOutages>> calculatePowerOutagesPerMonthAsync(int userId);
        public Task<double> getTotalCostPerDay(int userId, DateTime date);
        public Task<IEnumerable<EnergyConsumed>> getAllConsumedEnergyValuePerDayIndividual(int userId, DateTime date);
        public EnergyConsumed getLastEnergyValueDayIndividual(int userId, DateTime date);
    }
}
