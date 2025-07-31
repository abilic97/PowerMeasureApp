using System;
using System.Collections.Generic;

namespace PowerMeasure.Models
{
    public class EnergyMeter
    {
        public int Id { get; set; }
        public string EnergyMeterCode { get; set; }
        public int UserContractRef { get; set; }
        public bool Active { get; set; }
        public DateTime DateActiveFrom { get; set; }
        public DateTime DateActiveTo { get; set; }
        public UserContract Contract { get; set; }
        public ICollection<EnergyConsumed> Consumptions { get; set; }
        public bool IndividualDevice { get; set; }
    }
}
