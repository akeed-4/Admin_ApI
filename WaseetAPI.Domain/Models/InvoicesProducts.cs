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
    public class InvoicesProducts
    { 
        [Key] 
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int? id { get; set; }
        [DataMember]
        public string product_id { get; set; }
        [DataMember]
        public double? quantity { get; set; }
        [DataMember]
        public double? price { get; set; }

        public int? invoice_id { get; set; }
        [ForeignKey("invoice_id")]
        public Invoices invoices { get; set; }
        Products _product_data;
        [ForeignKey("product_id")]
        [DataMember]
        public Products product_data 
        {
            get
            {
                if (_product_data != null)
                    if (_product_data.userswarehouse != null)
                    {
                        var warehouse_user_data = _product_data.userswarehouse.Where(w => w.user_id == invoices.user_id).FirstOrDefault();
                        if (warehouse_user_data != null)
                        {
                            _product_data.price = warehouse_user_data.current_price ?? price;
                            _product_data.available_quantity = warehouse_user_data.available_quantity ?? 0;
                        }
                    };
                return _product_data;
            }
            set
            {
                _product_data = value;
            }
        }

        public double Price { get; set; }

        //public IProducts product_data
        //{ 
        //    get 
        //    {
        //        return products; 
        //    } 
        //}
    }
    //public interface IInvoicesProducts
    //{
    //    public IProducts product_data { get; }
    //    public double? quantity { get; set; }
    //    public double? price { get; set; }
    //}
    public class InvoicesProductsObjectResponse
    {
        public List<InvoicesProducts> data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public InvoicesProductsObjectResponse(List<InvoicesProducts> listOfWarehousProducts, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfWarehousProducts;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
