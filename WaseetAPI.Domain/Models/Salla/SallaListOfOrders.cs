using System;
using System.Collections.Generic;
using System.Text;

namespace WaseetAPI.Domain.Models.Salla
{
    public class SallaListOfOrders
    {
        public int salla_user_id { get; set; }
        public List<SallaOrders> data { get; set; }
    }
}
