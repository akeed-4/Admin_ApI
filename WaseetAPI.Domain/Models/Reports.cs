using System;
using System.Collections.Generic;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    public class ReportsFilter
    {
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }
    }
    public class ReportsTransactionsObject
    {
        public List<ReportsTransactions> transactions { get; set; }
        public List<ReportsTransactionsTotal> total { get; set; }
    }
    public class ReportsTransactionsObjectUser
    {
        public List<ReportsTransactionsUser> transactions { get; set; }
        public List<ReportsTransactionsTotal> total { get; set; }
    }
    public class ReportsTransactions
    {
        public int? id { get; set; }
        public double? no { get; set; }
        public string invoice_type { get; set; }
        public Languages invoice_name { get; set; }
        public Languages invoice_short_name { get; set; }
        public DateTime? date { get; set; }
        public double? amount { get; set; }
        public Customers customer { get; set; }
        public LocalUser localUser { get; set; }
    }

    public class ReportsTransactionsUser
    {
        public int? id { get; set; }
        public double? User_id { get; set; }
        public string? user_name { get; set; }
        public double? Total_cash { get; set; }
        public double? totalRecipt { get; set; }
        public double? ReturnSaleCredit { get; set; }
        public double? ReturnSaleCash { get; set; }
        
        public double? CashSales { get; set; }
        public double? CreditSales { get; set; }
        public LocalUser localUser { get; set; }
    }
    public class ReportsTransactionsTotal
    {
        public string invoice_type { get; set; }
        public Languages invoice_name { get; set; }
        
        public Languages invoice_short_name { get; set; }
        public double total_amount { get; set; }
    }
    public class ReportsTransactionsObjectResponse
    {
        public ReportsTransactionsObject data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public ReportsTransactionsObjectResponse(ReportsTransactionsObject listOfTranactions, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfTranactions;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }

    }
    public class ReportsTransactionsObjectResponseUser
    {
        public ReportsTransactionsObjectUser data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public ReportsTransactionsObjectResponseUser(ReportsTransactionsObjectUser listOfTranactions, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfTranactions;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }

    }
    public class ReportsTransactionsObjectRespons
    {
        public ReportsTransactionsObject data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public ReportsTransactionsObjectRespons(ReportsTransactionsObject listOfTranactions, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfTranactions;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
