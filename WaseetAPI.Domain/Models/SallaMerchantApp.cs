using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    public class SallaMerchantApp
    {
        [Key]
        [Column(Order = 1)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public decimal? id { get; set; }
        [Key]
        [Column(Order = 2)]
        public decimal? merchant_id { get; set; }
        public DateTime? created_at { get; set; }
        [Key]
        [Column(Order = 3)]
        public string app_name { get; set; }       
    }
}
