﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Models.DTO
{
    public class NewCard
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.CreditCard)]
        public long CardNumber { get; set; }

        [Required]
        public uint CardExpiry { get; set; }

        [Required]
        public uint Cvv { get; set; }
        public bool Vault { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public int billId { get; set; }
    }
}
