using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    public class UserStore
    {
        [Key]
        public int? id { get; set; }
        public int? user_id { get; set; }
        public int? salla_user_id { get; set; }
        public string bran { get; set; }
        public string ftype { get; set; }
        public string code { get; set; }
        public string baseet_user_no { get; set; }
        public string cash_account { get; set; }
        public string sales_account { get; set; }
        public string tax_account { get; set; }

        [ForeignKey("user_id")]
        public LocalUser localUser { get; set;}
    }
    public class UserStoreResponse
    {
        public UserStore data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public UserStoreResponse(UserStore user, bool response_status, Languages response_message, int response_error_code)
        {
            data = user;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
