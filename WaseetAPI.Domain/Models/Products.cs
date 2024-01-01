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
    public class Products
    {
        [Key]
        [DataMember]
        public string product_id { get; set; }
        [DataMember]
        public string product_aname { get; set; }
        [DataMember]
        public string product_ename { get; set; }
        [DataMember]
        public string product_image { get; set; }
        [DataMember]
        public string product_description { get; set; }
        //private double _price;
        [DataMember]
        public double? price { get; set; }
        private double _available_quantity;
        [NotMapped]
        [DataMember]
        public double? available_quantity 
        {
            get 
            {
                return _available_quantity; 
            }
            set
            {
                _available_quantity = value ?? 0;
            }
        }
        [DataMember]
        public Languages product_name 
        {
            get
            {
                return
                    new Languages()
                    {
                        Ar = product_aname,
                        En = product_ename
                    };
            }
        }
        public double? tax_per { get; set; }
        public string branch { get; set; }
        [DataMember]
        public int? new_item { get; set; }
        public ICollection<UsersWarehouse> userswarehouse { get; set; }               

    }
    //public interface IProducts
    //{
    //    public string product_id { get; set; }
    //    public Languages product_name { get; }
    //    public string product_image { get; set; }
    //    public string product_description { get; set; }
    //    public double? available_quantity { get; }
    //    public double? price { get; set; }
    //}
    public class ProductsResponse
    {
        public List<Products> data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public ProductsResponse(List<Products> listOfProducts, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfProducts;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
    public class ReturnProductsObject
    {
        public int? id { get; set; }
        public double? invoice_no { get; set; }
        public DateTime? invoice_date { get; set; }
        public ICollection<InvoicesProducts> products { get; set; }
    }

    public class ReturnProductsObjectResponse
    {
        public ReturnProductsObject data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public ReturnProductsObjectResponse(ReturnProductsObject returnProducts, bool response_status, Languages response_message, int response_error_code)
        {
            data = returnProducts;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
    public class ReturnProductsObjectListResponse
    {
        public List<ReturnProductsObject> data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public ReturnProductsObjectListResponse(List<ReturnProductsObject> listOfReturnProducts, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfReturnProducts;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }

    public class HistoryProductsObject
    {
        public Products product_data { get; set; }
        public Invoices invoice { get; set; }
        public double? quantity { get; set; }
    }
    public class HistoryProductsResponse
    {
        public List<HistoryProductsObject> data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public HistoryProductsResponse(List<HistoryProductsObject> listOfHistoryProducts, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfHistoryProducts;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
    public class ProductsObjectResponse
    {
        public Products data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public ProductsObjectResponse(Products product, bool response_status, Languages response_message, int response_error_code)
        {
            data = product;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
