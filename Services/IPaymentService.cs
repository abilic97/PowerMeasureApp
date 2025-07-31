using Newtonsoft.Json.Linq;
using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Services
{
    public interface IPaymentService
    {
        Task<CardPaymentResponse> AddNewCard(NewCard card);
        Task<JToken?> GetVaultedCard(string vaultId);
        Task<JToken?> QueryTransaction(string payRequestId);
        public Task<IEnumerable<Bill>> getAllBills(int userId);
        public Task<IEnumerable<Bill>> getAllPendingBills(int userId);
        public Task<Bill> addBill(Bill bill);
    }
}
