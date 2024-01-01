using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    [DataContract]
    public class Receipts
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int? id { get; set; }
        [DataMember]
        public double? receipt_no { get; set; }
        [DataMember]
        public double? receipt_amount { get; set; }
        DateTime _receipt_date;
        [DataMember]
        public DateTime? receipt_date
        {
            get
            {
                return _receipt_date;
            }
            set
            {
                TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                DateTime tstTime = TimeZoneInfo.ConvertTime(value ?? DateTime.UtcNow, TimeZoneInfo.Utc, tst);
                _receipt_date = TimeZoneInfo.ConvertTimeToUtc(tstTime, tst);
            }
        }
        public bool? receipt_status { get; set; }
        [DataMember]
        public int? customer_id { get; set; }
        [DataMember]
        public int? user_id { get; set; }
        [DataMember]
        public int? receipt_acceptance { get; set; }
        //return reciept state
        [DataMember]
        public string? receipt_state
        {
            get
            {
                return receipt_acceptance == 1  ? "معتمد" : "غير معتمد";


            }

        }
        public int? receipt_tobaseet { get; set; }
        [ForeignKey("customer_id")]
        [DataMember]
        public Customers customer { get; set; }
        [DataMember]
        public ICollection<ReceiptsInvoices> invoices { get; set; }



        private LocalUser _user;
        [DataMember]
        [ForeignKey("user_id")]
    
        public LocalUser localUser
        {
            get;
            set;
        }
    }

    //public class ReceiptsObject
    //{
    //    public int? id { get; set; }
    //    public double? receipt_no { get; set; }
    //    public double? receipt_amount { get; set; }
    //    public DateTime? receipt_date { get; set; }
    //    public int? receipt_acceptance { get; set; }
    //    public Customers customer { get; set; }
    //    public ICollection<InvoicesReceiptsObject> invoices { get; set; }
    //}
    public class ReceiptsNoInvoicesObject
    {
        public int? id { get; set; }
        public double? receipt_no { get; set; }
        public double? receipt_amount { get; set; }
        public DateTime? receipt_date { get; set; }
    }
    public class ReceiptResponse
    {
        public Receipts data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public ReceiptResponse(Receipts receipt, bool response_status, Languages response_message, int response_error_code)
        {
            data = receipt;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
    public class ReceiptsResponse
    {
        public List<Receipts> data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public ReceiptsResponse(List<Receipts> listOfReceipts, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfReceipts;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
