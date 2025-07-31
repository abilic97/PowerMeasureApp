using System;

namespace PowerMeasure.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public double EnergyCharge { get; set; }
        public double Tax { get; set; }
        public double BillAmount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool isPaid { get; set; }
        public int UsersId { get; set; }
        public DateTime ForMonth { get; set; }

    }
}
