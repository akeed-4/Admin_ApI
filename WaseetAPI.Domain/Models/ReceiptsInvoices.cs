using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    [DataContract]
    public class ReceiptsInvoices
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        [DataMember]
        public int? invoice_id { get; set; }
        public int? receipt_id { get; set; }
        [DataMember]
        public double? paid_amount { get; set; }

        [ForeignKey("receipt_id")]
        public Receipts receipts { get; set; }
        [ForeignKey("invoice_id")]
        public Invoices invoices { get; set; }
        [DataMember]
        public ReceiptsNoInvoicesObject receipt 
        {
            get
            {
                TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                DateTime tstTime = TimeZoneInfo.ConvertTime((receipts.receipt_date ?? DateTime.UtcNow), TimeZoneInfo.Utc, tst);                
                return new ReceiptsNoInvoicesObject()
                {
                    id = receipts.id,
                    receipt_no = receipts.receipt_no,
                    receipt_date = TimeZoneInfo.ConvertTimeToUtc(tstTime, tst),
                    receipt_amount = receipts.receipt_amount
                };
            }
        }
        [DataMember]
        public InvoicesNoReceiptsObject invoice
        {
            get
            {
                TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                DateTime tstTime = TimeZoneInfo.ConvertTime((invoices.invoice_date ?? DateTime.UtcNow), TimeZoneInfo.Utc, tst);

                //if (invoices.products != null)
                //{
                //    foreach (var p in invoice.products)
                //    {
                //        if (p.product_data != null)
                //            if (p.product_data.userswarehouse != null)
                //            {
                //                p.product_data.price = p.product_data.userswarehouse.Where(w => w.user_id == invoices.user_id).FirstOrDefault().current_price;
                //                p.product_data.available_quantity = p.product_data.userswarehouse.Where(w => w.user_id == invoices.user_id).FirstOrDefault().available_quantity;
                //            }
                //    }
                //}

                return new InvoicesNoReceiptsObject()
                {
                    id = invoices.id,
                    invoice_no = invoices.invoice_no,
                    invoice_type = invoices.invoice_type,
                    invoice_name = invoices.invoice_name,
                    invoice_short_name = invoices.invoice_short_name,
                    invoice_date = TimeZoneInfo.ConvertTimeToUtc(tstTime, tst),
                    total_amount = invoices.total_amount,
                    paid_amount = invoices.paid_amount,
                    invoice_acceptance = invoices.invoice_acceptance,
                    tax_amount = invoices.tax_amount,
                    qr_data = invoices.qr_data,
                    customer = invoices.customer,
                 
                    products = invoices.products
                };
            }
        }
    }
    //public interface IReceiptsInvoices
    //{
    //    public ReceiptsNoInvoicesObject receipt { get; }
    //    public double? paid_amount { get; set; }
    //}
    //public class InvoicesReceiptsObject
    //{
    //    public InvoicesNoReceiptsObject invoice { get; set; }
    //    public double? paid_amount { get; set; }
    //}
}
