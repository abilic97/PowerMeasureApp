using PowerMeasure.Models.DTO;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using PowerMeasure.Models;
using PowerMeasure.Data;

namespace PowerMeasure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly RestClient _client;
        private PowerMeasureDbContext _powerMeasureDbContext;
        public PaymentService(PowerMeasureDbContext context)
        {
            _client = new RestClient("https://secure.paygate.co.za");
            _powerMeasureDbContext = context;
        }
        public async Task<CardPaymentResponse> AddNewCard(NewCard card)
        {
            CardPaymentResponse payResponse = new CardPaymentResponse
            {
                Completed = false,
            };
            RestRequest request = new RestRequest("/payhost/process.trans", Method.Post);
            request.AddHeader("Content-Type", "text/xml");
            request.AddHeader("SOAPAction", "WebPaymentRequest");
            

            // request body
            string body;
            using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/Templates/SinglePaymentRequest.xml"))
            {
                body = await reader.ReadToEndAsync();
            }

            body = body.Replace("{PayGateId}", "");
            body = body.Replace("{Password}", "");
            body = body.Replace("{FirstName}", card.FirstName);
            body = body.Replace("{LastName}", card.LastName);
            body = body.Replace("{Mobile}", "");
            body = body.Replace("{Email}", card.Email);
            body = body.Replace("{CardNumber}", card.CardNumber.ToString());
            body = body.Replace("{CardExpiryDate}", card.CardExpiry.ToString());
            body = body.Replace("{CVV}", card.Cvv.ToString());
            // body = body.Replace("{Vault}", false.ToString());
            body = body.Replace("{MerchantOrderId}", Guid.NewGuid().ToString());
            // convert amount to cents (amount * 100)
            body = body.Replace("{Amount}", (card.Amount * 100).ToString("0000"));
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            RestResponse response = await _client.ExecuteAsync(request);

            
            string[] map = { "SinglePaymentResponse", "CardPaymentResponse" };
            JToken? result = MapXmlResponseToObject(response.Content, map);
            // check payment response
            if (result?["Status"] != null)
            {
                payResponse.Response = JsonConvert.SerializeObject(result);
                JToken? paymentStatus = result["Status"];
                switch (paymentStatus?["StatusName"]?.ToString())
                {
                    case "Error":
                        throw new ApplicationException();

                    case "Completed" when paymentStatus?["ResultCode"] != null:
                        payResponse.Completed = true;
                        payResponse.PayRequestId = paymentStatus?["PayRequestId"]?.ToString();
                        payResponse.Secure3DHtml = null;
                        if (paymentStatus?["ResultCode"]?.ToString() == "990017")
                        {
                            Bill bill = _powerMeasureDbContext.Bills.ToList().Single(x => x.Id == card.billId);
                            bill.isPaid = true;
                            bill.PaymentDate = DateTime.Now;
                            _powerMeasureDbContext.SaveChanges();
                            return payResponse;
                        }
                        throw new ApplicationException($"{paymentStatus?["ResultCode"]}: Payment declined");

                    case "ThreeDSecureRedirectRequired":
                        // payment requires 3D verification
                        JToken? redirectXml = result["Redirect"];
                        if (redirectXml?["UrlParams"] != null)
                        {
                            RestClient client = new RestClient(redirectXml["RedirectUrl"]?.ToString()!);
                            JArray urlParams = JArray.Parse(redirectXml["UrlParams"]?.ToString()!);
                            Dictionary<string, string> urlParamsDictionary = urlParams.Cast<JObject>()
                                .ToDictionary(item => item.GetValue("key")?.ToString(),
                                    item => item.GetValue("value")?.ToString())!;
                            string httpRequest = ToUrlEncodedString(urlParamsDictionary!);

                            RestRequest req = new RestRequest("/payhost/process.trans", Method.Post);
                            req.AddParameter("application/x-www-form-urlencoded", httpRequest,
                                ParameterType.RequestBody);
                            RestResponse res = await client.ExecuteAsync(req);

                            if (!res.IsSuccessful) throw new ApplicationException(res.ErrorMessage);

                            string responseContent = res.Content;
                            payResponse.Completed = false;
                            payResponse.Secure3DHtml = responseContent;
                            payResponse.PayRequestId = urlParamsDictionary["PAY_REQUEST_ID"];

                            return payResponse;

                        }
                        break;
                }
            }
            throw new ApplicationException("Payment request returned no results");
        }

        public async Task<JToken?> GetVaultedCard(string vaultId)
        {
            RestRequest request = new RestRequest("/payhost/process.trans", Method.Post);
            request.AddHeader("Content-Type", "text/xml");
            request.AddHeader("SOAPAction", "SingleVaultRequest");

            // request body
            string body;
            using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/Templates/SingleVaultRequest.xml"))
            {
                body = await reader.ReadToEndAsync();
            }

            body = body.Replace("{PayGateId}", "");
            body = body.Replace("{Password}", "");
            body = body.Replace("{VaultId}", vaultId);
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            RestResponse response = await _client.ExecuteAsync(request);

            string[] map = { "SingleVaultResponse", "LookUpVaultResponse" };
            JToken? result = MapXmlResponseToObject(response.Content, map);
            return result;
        }

        public async Task<JToken?> QueryTransaction(string payRequestId)
        {
            RestRequest request = new RestRequest("/payhost/process.trans", Method.Post);
            request.AddHeader("Content-Type", "text/xml");
            request.AddHeader("SOAPAction", "SingleFollowUpRequest");

            // request body
            string body;
            using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/Templates/SingleFollowUpRequest.xml"))
            {
                body = await reader.ReadToEndAsync();
            }

            body = body.Replace("{PayGateId}", "");
            body = body.Replace("{Password}", "");
            body = body.Replace("{PayRequestId}", payRequestId);
            request.AddParameter("text/xml", body, ParameterType.RequestBody);
            RestResponse response = await _client.ExecuteAsync(request);
            string[] map = { "SingleFollowUpResponse", "QueryResponse" };
            return MapXmlResponseToObject(response.Content, map);
        }

        private static JToken? MapXmlResponseToObject(string xmlContent, string[]? responseKeys)
        {
            XmlDocument xmlResult = new XmlDocument();
            // throws exception if it fails to parse xml
            xmlResult.LoadXml(xmlContent);
            // convert to json
            string result = JsonConvert.SerializeXmlNode(xmlResult);
            // remove prefix tags
            result = Regex.Replace(result, @"\bns2:\b", "");
            // parse as json object
            JObject paymentResponse = JObject.Parse(result);
            // return response
            JToken? response = paymentResponse["SOAP-ENV:Envelope"]?["SOAP-ENV:Body"];
            if (responseKeys != null)
            {
                response = responseKeys.Aggregate(response, (current, t) => current?[t]);
            }
            return response;
        }

        private static string ToUrlEncodedString(Dictionary<string, string?> request)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string key in request.Keys)
            {
                builder.Append("&");
                builder.Append(key);
                builder.Append("=");
                builder.Append(HttpUtility.UrlEncode(request[key]));
            }
            string result = builder.ToString().TrimStart('&');
            return result;
        }

        public async Task<IEnumerable<Bill>> getAllBills(int userId)
        {
            return _powerMeasureDbContext.Bills.Where(x => x.UsersId == userId).OrderBy(x=> x.Id).ToList();
        }
        public async Task<IEnumerable<Bill>> getAllPendingBills(int userId)
        {
            return _powerMeasureDbContext.Bills.Where(x => x.UsersId == userId && x.isPaid == false).ToList();

        }
        public async Task<Bill> addBill(Bill bill)
        {
            Users user = await _powerMeasureDbContext.Users.FindAsync(bill.UsersId);
            user.Bills = new List<Bill>();
            Bill nbill = new Bill();
            nbill.BillAmount = bill.BillAmount;
            nbill.DueDate = bill.DueDate;
            nbill.EnergyCharge = bill.EnergyCharge;
            nbill.ForMonth = bill.ForMonth;
            nbill.isPaid = bill.isPaid;
            nbill.PaymentDate = bill.PaymentDate;
            nbill.Tax = bill.Tax;
            user.Bills.Add(nbill);

            await _powerMeasureDbContext.Bills.AddAsync(nbill);
            await _powerMeasureDbContext.SaveChangesAsync();
            return nbill;
        }
    }
}
