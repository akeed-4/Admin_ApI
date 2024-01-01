using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WaseetAPI.Domain.Models.Salla
{
    public class SallaWebhookRequests
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public decimal? id { get; set; }
        public int salla_user_id { get; set; }
        public string event_name { get; set; }
        public string event_id { get; set; }
    }
}
