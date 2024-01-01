using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography.Xml;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    [DataContract]
    public class Invoices
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int? id { get; set; }
        [DataMember]
        public double? invoice_no { get; set; }
        [DataMember]
        public string invoice_type { get; set; }
        public Int16? invoice_type2 { get; set; }
        //DateTime _invoice_date;
        [DataMember]
        public DateTime? invoice_date { get; set; }
       
        public bool? invoice_status { get; set; }
        [DataMember]
        public int? invoice_acceptance { get; set; }
        [DataMember]
        public int? invoice_tobaseet { get; set; }
      //return invioce state string
        [DataMember]
        public string? invioce_state
        {
            get
            {
                return invoice_acceptance == 1 ? "معتمدة" : "غير معتمدة";


            }
           
        }
        //return invioce_type  string (naming)
        [DataMember]
        public string? invioce_types
        {
            get
            {
                if(invoice_type == "002")
                {
                    return "بيع اجل";
                }
                if (invoice_type == "001")
                {
                    return "بيع نقدا";
                }
                if (invoice_type == "003")
                {
                    return "ترجيع بيع نقدا";
                }
                else
                {
                    return "ترجيع بيع اجل ";
                }
            }

        }

        [DataMember]
        public double? total_amount { get; set; }
        [DataMember]
        public double? paid_amount { get; set; }

        [DataMember]
        public int? customer_id { get; set; }
        [DataMember]
        public string customer_name { get; set; }
        [DataMember]
        public string customer_tax_id { get; set; }
        [DataMember]
        public int? user_id { get; set; }

 
        
        [DataMember]
        public int? return_id { get; set; }
        [DataMember]
        public double? tax_amount { get; set; }
        [DataMember]
        public string qr_data { get; set; }
        private Customers _customer;
        [ForeignKey("customer_id")]
        [DataMember]
     


        public Customers customer 
        { 
            get
            {
                return _customer??new Customers();
            }
            set
            {
                _customer = value; 
            }
        }
        [ForeignKey("invoice_type, invoice_type2")]
        public VOU_SETTING vou_setting { get; set; }
        [DataMember]
        public ICollection<InvoicesProducts> products { get; set; }
    
      
        [DataMember]
        public ICollection<ReceiptsInvoices> receipts { get; set; }

        [DataMember]
        public Languages invoice_name 
        {
            get {
                if (vou_setting != null)
                    return
                        new Languages()
                        {
                            Ar = vou_setting.NAME1,
                            En = vou_setting.NAME2
                        };
                else
                    return
                        new Languages()
                        {
                            Ar = "",
                            En = ""
                        };
            }
        }

        [DataMember]
        public Languages invoice_short_name
        {
            get
            {
                if (vou_setting != null)
                    return
                        new Languages()
                        {
                            Ar = vou_setting.SHORT_NAME1,
                            En = vou_setting.SHORT_NAME2
                        };
                else
                    return
                        new Languages()
                        {
                            Ar = "",
                            En = ""
                        };
            }
        }

        [DataMember]
        public Languages btc_title
        {
            get
            {
                if (vou_setting != null)
                    return
                        new Languages()
                        {
                            Ar = vou_setting.SMALL_PRINTER_TITLEBTC,
                            En = vou_setting.SMALL_PRINTER_TITLEBTC2
                        };
                else
                    return
                        new Languages()
                        {
                            Ar = "",
                            En = ""
                        };
            }
        }

        [DataMember]
        public Languages btb_title
        {
            get
            {
                if (vou_setting != null)
                    return
                        new Languages()
                        {
                            Ar = vou_setting.SMALL_PRINTER_TITLEBTB,
                            En = vou_setting.SMALL_PRINTER_TITLEBTB2
                        };
                else
                    return
                        new Languages()
                        {
                            Ar = "",
                            En = ""
                        };
            }
        }
        private LocalUser _user;

        [DataMember]
        [ForeignKey("user_id")]
        public LocalUser localUser
        {
            get;
            set;
        }


        [DataMember]
        public double? amount_before_tax 
        {
            get 
            {
                return Math.Round((total_amount ?? 0) - (tax_amount ?? 0), 2);
            }
        }

        [DataMember]
        public double? tax_per
        {
            get
            {
                if (products != null)
                {
                    var product = products.Where(p => (p.product_data == null) ? false : (p.product_data.tax_per??0) > 0).FirstOrDefault();
                    if (product != null)
                        return product.product_data.tax_per;
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }
        //public IEnumerable<InvoicesProducts> Products
        //{
        //    get
        //    {
        //        return this.products;
        //    }

        //}


    }
    //public interface IInvoices
    //{
    //    public int? id { get; set; }
    //    public double? invoice_no { get; set; }
    //    public string invoice_type { get; set; }
    //    public Languages invoice_name { get; }
    //    public Languages invoice_short_name { get; }
    //    public Languages btc_title { get; }
    //    public Languages btb_title { get; }
    //    public DateTime? invoice_date { get; set; }
    //    public double? total_amount { get; set; }
    //    public double? paid_amount { get; set; }
    //    public int? invoice_acceptance { get; set; }
    //    public double? amount_before_tax { get; }
    //    public double? tax_per { get; }
    //    public double? tax_amount { get; set; }
    //    public string qr_data { get; set; }
    //    //public ICustomers customer { get; }
    //    public ICollection<InvoicesProducts> products { get; }
    //    public ICollection<ReceiptsInvoices> receipts { get; }
    //}

    public class InvoicesObjectResponse
    {
        public Invoices data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public InvoicesObjectResponse(Invoices Invoice, bool response_status, Languages response_message, int response_error_code)
        {
            data = Invoice;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }

    public class InvoicesObjectListResponse
    {
        public List<Invoices> data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public InvoicesObjectListResponse(List<Invoices> listOfInvoices, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfInvoices;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }

    public class InvoicesNoReceiptsObject
    {
        public int? id { get; set; }
        public double? invoice_no { get; set; }
        public string invoice_type { get; set; }
        public Languages invoice_name { get; set; }
        public Languages invoice_short_name { get; set; }
        public DateTime? invoice_date { get; set; }
        public double? total_amount { get; set; }
        public double? paid_amount { get; set; }
        public int? invoice_acceptance { get; set; }
        public double? tax_amount { get; set; }
        public string qr_data { get; set; }
        public Customers customer { get; set; }
      
        public LocalUser user { get; set; }

     
        public ICollection<InvoicesProducts> products { get; set; }
      
    }
    //public class SimpleInvoicesObject
    //{
    //    public int? id { get; set; }
    //    public double? invoice_no { get; set; }
    //    public string invoice_type { get; set; }
    //    public Languages invoice_type_name { get; set; }
    //    public DateTime? invoice_date { get; set; }
    //    public CustomersObject customer { get; set; }
    //}
}
