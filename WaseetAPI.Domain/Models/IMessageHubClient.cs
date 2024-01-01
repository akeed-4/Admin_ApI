using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WaseetAPI.Domain.Models
{
    public interface IMessageHubClient
    {
        Task SendOffersToUser(List<Invoices> message);
    }
}
