using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WaseetAPI.Domain.Models;

namespace WaseetAPI.Domain.TimerFeatures
{

    public class ChatHub : Hub
    {
        public async Task SendMessage( List<Invoices> message)
        {
            await Clients.All.SendAsync("ReceiveMessage",  message);
        }
    }
}
