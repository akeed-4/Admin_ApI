using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WaseetAPI.Application;
using WaseetAPI.Database;
using IdentityServer4;
using WaseetAPI.Domain.Models;
using WaseetAPI.Application.HupProcedure;
using Microsoft.AspNetCore.SignalR;

namespace WaseetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class customersController : Controller
    {
        private readonly IHubContext<ProceduresHub> _hubContext;
        GlobalProcedures global;
        public customersController(IHubContext<ProceduresHub> hubContext)
        {
            _hubContext = hubContext;
            global = new GlobalProcedures();
        }
        [Route("get_customers/{id}")]
        [HttpGet("get_customers")]
        public async Task<CustomersResponse> Get(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            var customer = await new CustomerProcedures(connectionStr).Do(user_id, id);
            await _hubContext.Clients.All.SendAsync("receiveCustomerData", customer);
            return await new CustomerProcedures(connectionStr).Do(user_id, id);
        }

        //return all customer

        [HttpGet("WebGet")]
        public async Task<CustomersResponse> WebGet(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new CustomerProcedures(connectionStr).Do(user_id, id);
        }
        [HttpPost("create")]
        public async Task<CustomersObjectResponse> create(Customers customer)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            //if (invoice.invoice_type == "001" || invoice.invoice_type == "002" || invoice.invoice_type == "003" || invoice.invoice_type == "004")

            customer.customer_type = 2;
            return await new CustomerProcedures(connectionStr).CreateCustomer(user_id, customer);
            //else
            //return await new InvoiceProcedures(connectionStr).CreateInvoice(user_id, null);
        }
        [HttpPost("update")]
        public async Task<CustomersObjectResponse> update(Customers customer)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new CustomerProcedures(connectionStr).UpdateCustomer(user_id, customer);
        }
        [Route("delete/{id}")]
        [HttpPost("delete")]
        public async Task<MessageResponse> delete(int id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new CustomerProcedures(connectionStr).DeleteCustomer(user_id, id);
        }
    }
}
