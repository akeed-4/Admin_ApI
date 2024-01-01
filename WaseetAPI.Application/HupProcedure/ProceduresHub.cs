using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;
using WaseetAPI.Domain.Models.Salla;


namespace WaseetAPI.Application.HupProcedure
{
    [Authorize]
    public class ProceduresHub : Microsoft.AspNetCore.SignalR.Hub
    {
        
        public async Task SendCustomerData (Customer customer)
        {
            // Send the customer data to all connected clients.
            await Clients.All.SendAsync("receiveCustomerData", customer);

        }

        public async Task SendProductData(Products product)
        {
            // Send the product data to all connected clients.
            await Clients.All.SendAsync("receiveProductData", product);
        }
        public async Task SendInvoiceData(Invoices invoices)
        {
           
            // Send the produinvoicesct data to all connected clients.
            await Clients.All.SendAsync("receiveinvoiceData", invoices);
        }
    }

}


