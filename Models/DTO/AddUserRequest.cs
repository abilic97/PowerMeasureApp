using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Models.DTO
{
    public class AddUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsUserActive { get; set; }
        public string RoleName { get; set; }
        public DateTime ContractValidFrom { get; set; }
        public DateTime ContractValidTo { get; set; }
        public DateTime EmContractValidFrom { get; set; }
        public DateTime EmContractValidTo { get; set; }
        public string EmCode { get; set; }
        
        public bool EmActive { get; set; }

    }
}
