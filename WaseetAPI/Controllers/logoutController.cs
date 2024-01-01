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
    public class logoutController : Controller
    {
        private readonly MainDbContext _ctx;
        private GlobalProcedures global;
        public logoutController(MainDbContext ctx)
        {
            _ctx = ctx;
            global = new GlobalProcedures();
        }

        //public async Task<MessageResponse> Get(Users user) => await new LoginProcedures(_ctx).LogoutUser(user);

        public async Task<MessageResponse> Get()
        {
            string currentUser = HttpContext.User.Claims.First(c => c.Type == "id").Value;
            int user_id = 0;
            int userOnlineType = 0;
            string connectionStr = "";
            int is_own_database = 1;
            global.getUserInfo(currentUser, ref userOnlineType, ref user_id, ref is_own_database, ref connectionStr);
            return await new LoginProcedures(_ctx).LogoutUser(user_id);
        }
    }
}
