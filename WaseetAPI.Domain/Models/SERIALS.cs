using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    [DataContract]
    public class SERIALS
    {
        [Key]
        [Column(Order = 1)]
        public string BTYPE { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BRAN { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CODE { get; set; }
        [Key]
        [Column(Order = 4)]
        public string FTYPE { get; set; }
        [Key]
        [Column(Order = 5)]
        public int? FTYPE2 { get; set; }
        public int? SERIAL { get; set; }
        //public int? FYEAR { get; set; }
        //public int? FMONTH { get; set; }

        [NotMapped]
        [DataMember]
        public string invoice_type 
        {
            get 
            {
                return FTYPE; 
            }
        }
        [NotMapped]
        [DataMember]
        public double user_last_serial 
        {
            get 
            {
                return Convert.ToDouble(SERIAL??0);
            }
        }
    }
    //public class SERIALSObject
    //{
    //    public string invoice_type { get; set; }
    //    public double user_last_serial { get; set; }
    //}
    public class SERIALSRespose
    {
        public List<SERIALS> list_of_serials { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public SERIALSRespose(List<SERIALS> listOfSerials, bool response_status, Languages response_message, int response_error_code)
        {
            list_of_serials = listOfSerials;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
