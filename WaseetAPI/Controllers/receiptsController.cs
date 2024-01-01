using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaseetAPI.Application;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;

namespace WaseetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class receiptsController : Controller
    {
        GlobalProcedures global;
        public receiptsController()
        {
            global = new GlobalProcedures();
        }
        [HttpGet("get_receipts")]
        public async Task<ReceiptsResponse> get_receipts()
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReceiptsProcedures(connectionStr).GetReceipts(user_id);
        }
        //return All Invoices
        [HttpGet("Webget_receipts")]
        public async Task<ReceiptsResponse> Webget_receipts()
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReceiptsProcedures(connectionStr).webGetReceipts(user_id);
        }
        [HttpPost("create")]
        public async Task<ReceiptResponse> create(Receipts receipt)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReceiptsProcedures(connectionStr).CreateReceipts(user_id, receipt);
        }
        [HttpPost("update")]
        public async Task<MessageResponse> update(Receipts receipt)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReceiptsProcedures(connectionStr).UpdateReceipts(user_id, receipt);
        }

        [HttpPost("delete")]
        public async Task<MessageResponse> delete(Receipts receipt)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReceiptsProcedures(connectionStr).ChangeReceiptAcceptance(receipt.id ?? 0, 2, user_id, userOnlineType, receipt.receipt_no ?? 0);
        }

        [HttpPost("approve")]
        public async Task<MessageResponse> approve(Receipts receipt)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReceiptsProcedures(connectionStr).ChangeReceiptAcceptance(receipt.id ?? 0, 1, user_id, userOnlineType, receipt.receipt_no ?? 0);
        }
    }
}
