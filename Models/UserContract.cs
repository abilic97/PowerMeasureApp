using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Models
{
    public class UserContract
    {
        public int Id { get; set; }
        public EnergyMeter EnergyMeter{ get; set; }
        public DateTime ContractValidFrom { get; set; }
        public DateTime ContractValidTo { get; set; }

        public int UsersId { get; set; }
    }
}
