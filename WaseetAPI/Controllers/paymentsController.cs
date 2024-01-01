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

namespace WaseetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class paymentsController : Controller
    {
        GlobalProcedures global;
        public paymentsController()
        {
            global = new GlobalProcedures();
        }
        [Route("get_payments/{id}")]
        [HttpGet("get_payments")]
        public async Task<CustomersResponse> Get(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new CustomerProcedures(connectionStr).GetPayments(user_id);
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
            customer.customer_type = 3;
            customer.user_id = 0;
            return await new CustomerProcedures(connectionStr).CreateCustomer(user_id, customer);           
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
            customer.customer_type = 3;
            customer.user_id = 0;
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
