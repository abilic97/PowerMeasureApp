using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Models
{
    public class EnergyConsumed
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public double Voltage { get; set; }
        public double Power { get; set; }
        public double Current { get; set; }
        public bool IsDailyFinal { get; set; }
        public int EnergyMeterId { get; set; }
    }
}
