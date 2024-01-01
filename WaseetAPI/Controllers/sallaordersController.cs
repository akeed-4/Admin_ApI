using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaseetAPI.Application;
using WaseetAPI.Application.Salla;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;
using WaseetAPI.Domain.Models.Salla;

namespace WaseetAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class sallaordersController : Controller
    {
        GlobalProcedures global;
        private readonly MainDbContext _ctx;
        public sallaordersController(MainDbContext ctx)
        {
            global = new GlobalProcedures();
            _ctx = ctx;
        }
        [HttpPost]
        public async Task<MessageResponse> Post(SallaListOfOrders new_receive_data)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                foreach (var order in new_receive_data.data)
                {
                    string new_receive_data_string = JsonConvert.SerializeObject(new SallaReceive() { Event = "order.created", merchant = Convert.ToString(new_receive_data.salla_user_id), data = order, created_at = DateTime.Now });
                    status = await new ReceiveProcedures(_ctx).save_receive_data(new_receive_data_string, true);
                    if (status)
                    {
                        message = "success";
                        error_code = 200;
                    }                               
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);
        }
    }
}
