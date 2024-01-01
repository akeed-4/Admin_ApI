using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace WaseetAPI.Domain.Models.Salla
{   
    public class SallaOrders
    {
        public Int64 id { get; set; }
        public Int64 reference_id { get; set; }
        [JsonIgnore]
        public OrderNumbers number1 { get; set; }
        //public Urls urls { get; set; }
        public Date date { get; set; }
        public Status status { get; set; }
        public string payment_method { get; set; }
        [JsonIgnore]
        public Languages payment_method_name { get; set; }
        [JsonIgnore]
        public string payment_method_code { get; set; }
        [JsonIgnore]
        public double payment_method_com_per { get; set; }
        [JsonIgnore]
        public string payment_method_com_code { get; set; }
        [JsonIgnore]
        public double payment_method_com_max { get; set; }
        //public string currency { get; set; }
        //public Amounts amounts { get; set; }
        //public Shipping shipping { get; set; }
        //public bool can_cancel { get; set; }
        //public List<string> shipment_branch { get; set; }
        public Customer customer { get; set; }
        //public Bank bank { get; set; }
        public List<Items> items { get; set; }
        //public List<string> tags { get; set; }
    }
    public class Urls
    {
        public string customer { get; set; }
        public string admin { get; set; }
    }
    public class Date
    {
        public DateTime? date { get; set; }
        public Int64 timezone_type { get; set; }
        public string timezone { get; set; }        
    }
    public class Status
    {
        public Int64 id { get; set; }
        public string name { get; set; }
        public Customized customized { get; set; }
    }
    public class Amounts
    {
        public Amount sub_total { get; set; }
        public Amount shipping_cost { get; set; }
        public Amount cash_on_delivery { get; set; }
        public Tax tax { get; set; }
        public List<string> discounts { get; set; }
        public Amount total { get; set; }
    }
    public class Shipping
    {
        public Int64 id { get; set; }
        public string company { get; set; }
        public Receiver receiver { get; set; }
        public Shipper shipper { get; set; }
        public Address pickup_address { get; set; }
        public Shipment shipment { get; set; }
    }
    public class Customer
    {
        public Int64 id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Int64 mobile { get; set; }
        public string mobile_code { get; set; }
        public string email { get; set; }
        public Urls urls { get; set; }
        public string avatar { get; set; }
        public string gender { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public string location { get; set; }
        public Date updated_at { get; set; }
    }
    public class Bank
    {
        public Int64 id { get; set; }
        public string bank_name { get; set; }
        public string account_name { get; set; }
        public string account_number { get; set; }
        public string iban_number { get; set; }
        public string status { get; set; }
    }
    public class Items
    {
        public Int64 id { get; set; }

        [JsonIgnore]
        public string icode { get; set; }
        [JsonIgnore]
        public string code2 { get; set; }
        
        public string name { get; set; }
        public string sku { get; set; }
        public decimal quantity { get; set; }
        public string currency { get; set; }
        public decimal weight { get; set; }
        public IAmounts amounts { get; set; }
        public string notes { get; set; }
        public SallaProducts products { get; set; }
        public List<string> options { get; set; }
        public List<string> images { get; set; }
        public List<string> codes { get; set; }
        public List<string> files { get; set; }
    }
    public class Customized
    {
        public Int64 id { get; set; }
        public string name { get; set; }
    }
    public class Amount
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
    }
    public class Tax
    {
        public string percent { get; set; }
        public Amount amount { get; set; }
    }
    public class Receiver
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
    public class Shipper
    {
        public string name { get; set; }
        public string company_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
    public class Address
    {
        public string country { get; set; }
        public string city { get; set; }
        public string shipping_address { get; set; }
        public string street_number { get; set; }
        public string block { get; set; }
        public string postal_code { get; set; }
        public Geo geo_coordinates { get; set; }
    }
    public class Shipment
    {
        public string id { get; set; }
        public string pickup_id { get; set; }
        public string tracking_link { get; set; }
    }
    public class IAmounts
    {
        public Amount price_without_tax { get; set; }
        public Amount total_discount { get; set; }
        public Tax tax { get; set; }
        public Amount total { get; set; }
    }
    public class SallaProducts
    {
        public Int64 id { get; set; }
        public string type { get; set; }
        public Promotion promotion { get; set; }
        public string status { get; set; }
        public bool is_available { get; set; }
        public string sku { get; set; }
        public string name { get; set; }
        public Amount price { get; set; }
        public Amount sale_price { get; set; }
        public string currency { get; set; }
        public string url { get; set; }
        public string thumbnail { get; set; }
        public bool has_special_price { get; set; }
        public Amount regular_price { get; set; }
        public string favorite { get; set; }
    }
    public class Geo
    {
        public decimal lat { get; set; }
        public decimal lang { get; set; }
    }
    public class Promotion
    {
        public string title { get; set; }
        public string sub_title { get; set; }
    }
    public class OrderNumbers
    {
        public string code2 { get; set; }
        public float number1 { get; set; }
    }
}
