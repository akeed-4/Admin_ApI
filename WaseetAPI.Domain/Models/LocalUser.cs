using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WaseetAPI.Domain.Models
{

    public class LocalUser
    {
        [Key]
        public int? user_id { get; set; }
        public string user_name { get; set; }

        public virtual ICollection<Invoices> Invoices { get; set; }
        public virtual ICollection<Receipts> Receipts { get; set; }
       
        public virtual ICollection<UserStore> UserStore { get; set; }
        public int invoices_count
        {
            get
            {
                List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                if (Invoices == null)
                    return 0;
                else
                    return Invoices.Where(x => x.invoice_status == true && x.invoice_acceptance != 2 && invoice_types.Contains(x.invoice_type)).Count();
            }
        }
        public class localUserResponse
        {
            public List<LocalUser> data { get; set; }
            public bool status { get; set; }
            public Languages message { get; set; }
            public int error_code { get; set; }
            public localUserResponse(List<LocalUser> listOfProducts, bool response_status, Languages response_message, int response_error_code)
            {
                data = listOfProducts;
                status = response_status;
                message = response_message;
                error_code = response_error_code;
            }
        }

    }
}