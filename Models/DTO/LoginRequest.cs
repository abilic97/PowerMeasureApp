using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Models.DTO
{
    public class LoginRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}
