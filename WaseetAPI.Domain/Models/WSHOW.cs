using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    public class WSHOW
    {
        [Key]
        [Column(Order = 1)]
        public string ftype { get; set; }
        [Key]
        [Column(Order = 2)]
        public string bran { get; set; }
        [Key]
        [Column(Order = 3)]
        public string code { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string tax_id { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string DSEGAL { get; set; }
        public string CANTRY { get; set; }
        public string CITY { get; set; }
        public string LOCALAREA { get; set; }
        public string DBOXID { get; set; }
        public string DBULDINGID { get; set; }
        public string INFO1 { get; set; }
        //public double? WEB_STORE_TYPE { get; set; }           
    }

    public class WSHOWResponse
    {
        public WSHOW data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public WSHOWResponse(WSHOW company_info, bool response_status, Languages response_message, int response_error_code)
        {
            data = company_info;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
