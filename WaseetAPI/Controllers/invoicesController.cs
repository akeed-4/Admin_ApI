using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


using WaseetAPI.Application;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
using IdentityServer4.Models;
using WaseetAPI.Domain.TimerFeatures;
using Microsoft.AspNet.SignalR.Messaging;
using WaseetAPI.Application.HupProcedure;
using WaseetAPI.Domain.Models.Salla;

namespace WaseetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class invoicesController : Controller
    {
        private readonly IHubContext<ProceduresHub> _hubContext;

        GlobalProcedures global;
        public invoicesController(IHubContext<ProceduresHub> hubContext)
        {
            global = new GlobalProcedures();
            _hubContext = hubContext;
        }
        [Route("get_invoices/{id}")]
        [HttpGet("get_invoices")]
        public async Task<InvoicesObjectListResponse> get_invoices(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
          
            return await new InvoiceProcedures(connectionStr).GetInvoices(user_id, id);
        }
        //return GetAll invoices 
        [HttpGet("Webget_invoices")]
        public async Task<InvoicesObjectListResponse> Webget_invoices(string id)
        {
            
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            var invoices = await new InvoiceProcedures(connectionStr).webGetInvoices(id);
            await _hubContext.Clients.All.SendAsync("receiveinvoiceData", invoices);
            return await new InvoiceProcedures(connectionStr).webGetInvoices(id);
           
        }
        [HttpGet("Post")]
        public async Task <IActionResult> Post(string id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            var invoices = await new InvoiceProcedures(connectionStr).webGetInvoices(id);
            await _hubContext.Clients.All.SendAsync("receiveinvoiceData", invoices);
            return Ok(true);
        }
        public async Task<InvoicesObjectListResponse> GetInvoices(string invoiceType)
        {
            string invoiceTypes = null;
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            // Get the invoices from the database
            var invoices = await new InvoiceProcedures(connectionStr).webGetInvoices(invoiceTypes);
            await _hubContext.Clients.All.SendAsync("receiveCustomerData", invoices);
            // Return the invoices to the client
            return invoices;
        }




        [HttpGet("recheck_taxes")]

        public async Task<MessageResponse> recheck_taxes()
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new InvoiceProcedures(connectionStr).ReCheckTaxes();            
        }
        [HttpPost("create")]
        public async Task<InvoicesObjectResponse> create(Invoices invoice)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            //if (invoice.invoice_type == "001" || invoice.invoice_type == "002" || invoice.invoice_type == "003" || invoice.invoice_type == "004")
           
            var invioce= await new InvoiceProcedures(connectionStr).CreateInvoice(user_id, invoice);
            await _hubContext.Clients.All.SendAsync("receiveCustomerData", invioce);
            return await new InvoiceProcedures(connectionStr).CreateInvoice(user_id, invoice);
            //else
            //return await new InvoiceProcedures(connectionStr).CreateInvoice(user_id, null);
        }
        [HttpPost("update")]
        public async Task<InvoicesObjectResponse> update(Invoices invoice)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new InvoiceProcedures(connectionStr).UpdateInvoice(user_id, invoice);
        }

        [HttpPost("delete")]
        public async Task<InvoicesObjectResponse> delete(Invoices invoice)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new InvoiceProcedures(connectionStr).ChangeInoiveAcceptance(invoice.id ?? 0, userOnlineType, 2, user_id, invoice.invoice_no ?? 0);
        }

        [HttpPost("approve")]
        public async Task<InvoicesObjectResponse> approve(Invoices invoice)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new InvoiceProcedures(connectionStr).ChangeInoiveAcceptance(invoice.id ?? 0, userOnlineType, 1, user_id, invoice.invoice_no ?? 0);
        }
    }

    
}
