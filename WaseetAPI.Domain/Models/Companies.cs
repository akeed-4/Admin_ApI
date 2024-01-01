using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    public class Companies
    {
        [Key]
        public string company_id { get; set; }
        public string server_name { get; set; }
        public string server_user_name { get; set; }
        public string server_password { get; set; }
        public string db_name { get; set; }
        public int is_own_database { get; set; }
        public ICollection<Users> users { get; set; }
    }
}
