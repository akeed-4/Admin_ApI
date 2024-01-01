using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WaseetAPI.Application;
using WaseetAPI.Domain.Models;
using static WaseetAPI.Domain.Models.LocalUser;

namespace WaseetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class reportsController : Controller
    {
   
        GlobalProcedures global;
        public reportsController()
        {
          
            global = new GlobalProcedures();
        }
        [HttpGet("get_transaction")]
        public async Task<ReportsTransactionsObjectResponse> get_transaction(/*ReportsFilter reportsFilter*/)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReportProcedures(connectionStr).GetTransaction(user_id/*, reportsFilter*/);
        }

        [HttpGet("Webget_transaction")]
        public async Task<ReportsTransactionsObjectResponse> Webget_transaction(/*ReportsFilter reportsFilter*/)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReportProcedures(connectionStr).GetTransaction(user_id/*, reportsFilter*/);
        }
        

       [HttpGet("GetAllUsers")]
        public async Task<localUserResponse> GetAllUsers(/*ReportsFilter reportsFilter*/)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReportProcedures(connectionStr).GetAllUser(user_id/*, reportsFilter*/);
        }
        [HttpGet("GetTotalUser/{user_Id}/{start}/{end}")]
        public async Task<ReportsTransactionsObjectResponse> GetTotalUser(int user_Id,DateTime start,DateTime end)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReportProcedures(connectionStr).GetTotalUsers( user_Id,start,end);
        }
        [HttpGet("GetTotalUserId/{user_Id}")]
        public async Task<ReportsTransactionsObjectResponse> GetTotalUserId(int user_Id)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReportProcedures(connectionStr).GetTotalUsersId(user_Id);
        }
        [HttpGet("Webget_transactionUser")]
        public async Task<ReportsTransactionsObjectResponseUser> Webget_transactionUser(int user_Id/*ReportsFilter reportsFilter*/)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReportProcedures(connectionStr).GetTransactionUser(user_Id );
        }

        [HttpGet("Webget_transactionUsertoday")]
        public async Task<ReportsTransactionsObjectResponseUser> Webget_transactionUsertoday(int user_Id/*ReportsFilter reportsFilter*/)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReportProcedures(connectionStr).GetTransactionUserToday(user_Id);
        }
        [HttpGet("Webget_transactionUserDate/{start}/{end}")]
        public async Task<ReportsTransactionsObjectResponseUser> Webget_transactionUserDate(int user_Id,DateTime start,DateTime end/*ReportsFilter reportsFilter*/)
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new ReportProcedures(connectionStr).GetTransactionUserdate(user_Id,start,end);
        }
       
    }
}
