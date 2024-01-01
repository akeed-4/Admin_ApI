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
    [ApiController]
    [Route("[controller]")]
    public class controlController : Controller
    {
        GlobalProcedures global;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        public controlController(IJwtAuthenticationManager jwtAuthenticationManager)
        {
            global = new GlobalProcedures();
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }
        [AllowAnonymous]
        [HttpGet("validate")]
        public MessageResponse validate()
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                string token_value = null;
                if (token != null)
                    token_value = _jwtAuthenticationManager.ValidateJwtToken(token);
                if (token_value != null)
                {
                    status = true;
                    error_code = 200;
                }
                else
                {
                    error_code = 327;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);
        }
        [Authorize]
        [HttpGet("get_types_name")]
        public async Task<VOU_SETTINGListResponse> get_types_name()
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 1;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new InvoiceProcedures(connectionStr).GetTypeNameList();
        }
    }
}
