using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    public class UsersWarehouse
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        public int? user_id { get; set; }
        public string product_id { get; set; }
        public double? available_quantity { get; set; }
        public double? current_price { get; set; }
        //Products _product_data;
        [ForeignKey("product_id")]
        public Products product_data { get; set; }
    }
}
