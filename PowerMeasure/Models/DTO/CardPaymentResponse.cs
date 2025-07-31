using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Models.DTO
{
    public class CardPaymentResponse
    {
        public bool Completed { get; set; }
        public string Secure3DHtml { get; set; }
        public string PayRequestId { get; set; }
        public string Response { get; set; }
    }
}
