using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CheckoutDTO;
using HCP.Service.DTOs.PaymentDTO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.TemporaryService
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class TemporaryStorage : ITemporaryStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TemporaryStorage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task StoreAsync(ConfirmBookingDTO request, ClaimsPrincipal userClaims)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var key = $"{userClaims.Identity.Name}_booking";
            var jsonData = JsonConvert.SerializeObject(request);
            session.SetString(key, jsonData);
            await Task.CompletedTask;
        }
        public async Task StoreAsync(PaymentBodyDTO request)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var key = $"{request.Id}_bookingPayment";
            var jsonData = JsonConvert.SerializeObject(request);
            session.SetString(key, jsonData);
            await Task.CompletedTask;
        }
        public async Task<PaymentBodyDTO> RetrieveAsync(Guid id)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var key = $"{id}_bookingPayment";
            var jsonData = session.GetString(key);
            if (jsonData != null)
            {
                return JsonConvert.DeserializeObject<PaymentBodyDTO>(jsonData);
            }
            return null;
        }
        public async Task StoreTest(ClaimsPrincipal userClaims, Test huh)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var jsonData = JsonConvert.SerializeObject(huh);
            session.SetString("hehe", jsonData);
            await Task.CompletedTask;

        }
        public async Task<Test> RetrieveTest(ClaimsPrincipal userClaims)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var key = $"{userClaims.Identity.Name}_huh";
            var jsonData = session.GetString("hehe");
            if (jsonData != null)
            {
                return JsonConvert.DeserializeObject<Test>(jsonData);
            }
            return null;

        }
        public async Task<ConfirmBookingDTO> RetrieveAsync(Guid orderId, ClaimsPrincipal userClaims)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var key = $"{userClaims.Identity.Name}_booking";
            var jsonData = session.GetString(key);
            if (jsonData != null)
            {
                return JsonConvert.DeserializeObject<ConfirmBookingDTO>(jsonData);
            }
            return null;
        }
    }
}
