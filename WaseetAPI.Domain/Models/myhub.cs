using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace WaseetAPI.Domain.Models
{
  
        public class MyHub : Hub
        {
   
            public void OnNewData()
            {
                Clients.All.SendAsync("newData", "This is new data!");
            }
        }

     


    
}
