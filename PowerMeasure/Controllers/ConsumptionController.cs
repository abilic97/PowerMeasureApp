using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using PowerMeasure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerMeasure.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumptionController : Controller
    {
        IConsumptionService _consumptionService;
        public ConsumptionController(IConsumptionService service)
        {
            _consumptionService = service;

        }
        [HttpGet("getTotalDailyEnergy/{userId}")]
        public async Task<Measurements> getUserTotalEnergyValuePerDay(int userId, [FromQuery(Name = "date")] string date)
        {
            var parsedDate = DateTime.Parse(date);
            return _consumptionService.getTotalConsumedEnergyValuePerDay(userId, parsedDate);
        }

        [HttpGet("getLastPowerValue/{userId}")]
        public EnergyConsumed getUserLastEnergyValues(int userId, [FromQuery(Name = "date")] string date)
        {
            var parsedDate = DateTime.Parse(date);
            return _consumptionService.getLastEnergyValueDay(userId, parsedDate);
        }
        [Authorize]
        [HttpGet("noofPo/{userId}")]
        public async Task<IEnumerable<PowerOutages>> noofPO(int userId)
        {
            return await _consumptionService.calculatePowerOutagesPerMonth(userId);
        }

        [HttpGet("get-daily-cost/{userId}")]
        public async Task<double> getdailycost(int userId, [FromQuery(Name = "date")] string date)
        {
            var parsedDate = DateTime.Parse(date);

            return await _consumptionService.getTotalCostPerDay(userId, parsedDate);
        }

        [HttpGet("get-monthly-total/{userId}")]
        public async Task<IEnumerable<MonthlyCost>> getMonthlyTotal(int userId)
        {
            return await _consumptionService.getConsumedEnergyForUserPerMonth(userId);
        }

        [HttpGet("get-all-individual-values/{userId}")]
        public async Task<IEnumerable<EnergyConsumed>> getAllIndividualEnergy(int userId, [FromQuery(Name = "date")] string date)
        {
            var parsedDate = DateTime.Parse(date);
            return await _consumptionService.getAllConsumedEnergyValuePerDayIndividual(userId, parsedDate);
        }

        [HttpGet("get-last-individual-value/{userId}")]
        public EnergyConsumed getLastIndividualValue(int userId, [FromQuery(Name = "date")] string date)
        {
            var parsedDate = DateTime.Parse(date);
            return _consumptionService.getLastEnergyValueDayIndividual(userId, parsedDate);
        }
    }
}
