using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using PowerMeasure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _payment;

        public PaymentController(IPaymentService payment)
        {
            _payment = payment;
        }
        // GET
        public IActionResult Index()
        {
            return Ok(new { message = "Hello world" });
        }

        [HttpPost(Name = nameof(AddNewCard))]
        public async Task<IActionResult> AddNewCard([FromBody] NewCard model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values.SelectMany(err => err.Errors[0].ErrorMessage));
                }

                CardPaymentResponse response = await _payment.AddNewCard(model);
                return Ok(response);
            }
            catch (ApplicationException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpGet("{vaultId}", Name = nameof(GetVaultedCard))]
        public async Task<IActionResult> GetVaultedCard([FromRoute] string vaultId)
        {
            try
            {
                JToken result = await _payment.GetVaultedCard(vaultId);
                return Ok(result?.ToString());
            }
            catch (ApplicationException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpGet("query/{payRequestId}", Name = nameof(QueryTransaction))]
        public async Task<IActionResult> QueryTransaction([FromRoute] string payRequestId)
        {
            try
            {
                JToken result = await _payment.QueryTransaction(payRequestId);
                return Ok(result?.ToString());
            }
            catch (ApplicationException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPost("addBill")]
        public Task<Bill> addBill([FromBody] Bill bill)
        {
            return _payment.addBill(bill);
        }

        [HttpGet("getBills/{userId}")]
        public async Task<IEnumerable<Bill>> getBills(int userId)
        {
            return await _payment.getAllBills(userId);
        }

        [HttpGet("getUnpaidBills/{userId}")]
        public async Task<IEnumerable<Bill>> getUnpaidBills(int userId)
        {
            return await _payment.getAllPendingBills(userId);
        }

    }
}
