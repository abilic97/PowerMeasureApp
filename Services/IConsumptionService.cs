using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Services
{
    public interface IConsumptionService
    {
        public Measurements getTotalConsumedEnergyValuePerDay(int userId, DateTime date); //done
        public EnergyConsumed getLastEnergyValueDay(int userId, DateTime date); //done

        public Task<IEnumerable<MonthlyCost>> getConsumedEnergyForUserPerMonth(int userId);
        public Task<IEnumerable<double>> calculateUserCostPerMonth(int userId);
        public Task<IEnumerable<PowerOutages>> calculatePowerOutagesPerMonth(int userId); //done

        public Task<double> getTotalCostPerDay(int userId, DateTime date);

        public Task<IEnumerable<EnergyConsumed>> getAllConsumedEnergyValuePerDayIndividual(int userId, DateTime date); //done
        public EnergyConsumed getLastEnergyValueDayIndividual(int userId, DateTime date); //done
    }
}
