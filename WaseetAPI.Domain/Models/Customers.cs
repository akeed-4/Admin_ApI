using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    [DataContract]
    public class Customers
    {
        [Key]
        [DataMember]
        public int? customer_id { get; set; }
        [DataMember]
        public string customer_aname { get; set; }
        [DataMember]
        public string customer_ename { get; set; }
        [DataMember]
        public string customer_image { get; set; }
        [DataMember]
        public string customer_address { get; set; }
        [DataMember]
        public string customer_taxid { get; set; }
        [DataMember]
        public string customer_segal { get; set; }
        [DataMember]
        public string customer_country { get; set; }
        [DataMember]
        public string customer_city { get; set; }
        [DataMember]
        public string customer_state { get; set; }
        [DataMember]
        public string customer_postcode { get; set; }
        [DataMember]
        public string customer_building { get; set; }
        [DataMember]
        public string customer_street { get; set; }
        [DataMember]
        public string customer_mobile { get; set; }
        [DataMember]
        public int? user_id { get; set; }
        public int? customer_type { get; set; }
        public string branch { get; set; }
        public ICollection<Invoices> invoices { get; set; }
        public ICollection<Receipts> receipts { get; set; }

        [DataMember]
        public Languages customer_name
        {
            get
            {
                return
                        new Languages()
                        {
                            Ar = customer_aname,
                            En = customer_ename
                        };
            }
        }
        [DataMember]
        public int invoices_count
        {
            get
            {
                List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                if (invoices == null)
                    return 0;
                else
                    return invoices.Where(i => i.invoice_acceptance != 2 && invoice_types.Contains(i.invoice_type)).Count();
            }
        }



        //return Balance per customer
        [DataMember]
        public double Balance
        {
            get
            {
                List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                if (invoices == null)
                    return 0;
                else
                    return invoices.Where(i => i.invoice_acceptance != 2 && invoice_types.Contains(i.invoice_type)).Sum(r => r.total_amount).Value;
            }
        }
        [DataMember]
        public DateTime? Invioce_date
        {
            get
            {
                List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                if (invoices == null)
                    return DateTime.UtcNow;
                else
                    return invoices.Where(i => i.invoice_acceptance != 2 && invoice_types.Contains(i.invoice_type)).Select(r => r.invoice_date).FirstOrDefault();
            }
        }
       
        [DataMember]
        public double receipts_amount
        {
            get
            {
                if (receipts == null)
                    return 0;
                else
                    return receipts.Sum(r => r.receipt_amount).Value;
            }
        }
     
    }
    //public interface ICustomers
    //{
    //    public int? customer_id { get; set; }
    //    public Languages customer_name { get; }
    //    public string customer_image { get; set; }
    //    public string customer_address { get; set; }
    //    public int invoices_count { get; }
    //    public double receipts_amount { get; }
    //}
    public class CustomersResponse
    {
        public List<Customers> data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public CustomersResponse(List<Customers> listOfCustomers, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfCustomers;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
    public class CustomersObjectResponse
    {
        public Customers data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public CustomersObjectResponse(Customers customer, bool response_status, Languages response_message, int response_error_code)
        {
            data = customer;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
