using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static WaseetAPI.Domain.Models.LocalUser;

namespace WaseetAPI.Application
{
    public class ReportProcedures
    {
        private ApplicationDbContext _context;
        private string _connectionStr;
        private GlobalProcedures global;
        public ReportProcedures(string connectionStr)
        {
            this._context = new ApplicationDbContext(connectionStr);
            this._connectionStr = connectionStr;
            this.global = new GlobalProcedures();
        }
        private ReportsTransactions GetReportsTransactionsFromInvoice(Invoices invoice, ref List<ReportsTransactionsTotal> transactionsTotal)
        {
            //ICustomers customersObject = new Customers();
            string cash_payment_type = "555";
            //Customer Object
            if (invoice.customer != null)
            {
                ////customersObject = new CustomerProcedures(_connectionStr).GetCustomersObjectFromCustomer(invoice.customer);
                //customersObject = invoice.customer;
                if ((invoice.customer.customer_type ?? 2) == 3)
                    cash_payment_type = "666";
            }
            string invoice_type = invoice.invoice_type;
            if (invoice_type != "001" && invoice_type != "002" && invoice_type != "003" && invoice_type != "004")
                return null;
            ReportsTransactions transaction = new ReportsTransactions()
            {
                id = invoice.id,
                no = invoice.invoice_no,
                date = global.CovertTimeZone(invoice.invoice_date ?? DateTime.UtcNow),
                invoice_type = invoice_type,
                invoice_name = invoice.invoice_name,
                invoice_short_name =invoice.invoice_short_name,
                amount = invoice.total_amount,
                //customer = customersObject
                customer = invoice.customer,
                localUser=invoice.localUser,
            };
            transactionsTotal.Find(i => i.invoice_type == invoice_type).total_amount += invoice.total_amount ?? 0;
            if (invoice_type == "001")
                transactionsTotal.Find(i => i.invoice_type == "555").total_amount += invoice.total_amount ?? 0;
            else if (invoice_type == "003")
                transactionsTotal.Find(i => i.invoice_type == cash_payment_type).total_amount -= invoice.total_amount ?? 0;
            return transaction;
        }
        private ReportsTransactions GetReportsTransactionsFromReceipt(Receipts receipt, ref List<ReportsTransactionsTotal> transactionsTotal)
        {
            //ICustomers customersObject = new Customers();
            //Customer Object
            //if (receipt.customer != null)
            //{
            //    //customersObject = new CustomerProcedures(_connectionStr).GetCustomersObjectFromCustomer(receipt.customer);
            //    customersObject = receipt.customer;
            //}
            ReportsTransactions transaction = new ReportsTransactions()
            {
                id = receipt.id,
                no = receipt.receipt_no,
                date = global.CovertTimeZone(receipt.receipt_date ?? DateTime.UtcNow),
                invoice_type = "055",
                invoice_name = new Languages()
                {
                    Ar = "سند قبض",
                    En = "Receipt Voucher"
                },
                invoice_short_name = new Languages()
                {
                    Ar = "قبض",
                    En = "Receipt"
                },
                amount = receipt.receipt_amount,
                //customer = customersObject
                customer = receipt.customer,
                localUser = receipt.localUser,
            };
            transactionsTotal.Find(i => i.invoice_type == "055").total_amount += receipt.receipt_amount ?? 0;
            transactionsTotal.Find(i => i.invoice_type == "555").total_amount += receipt.receipt_amount ?? 0;
            return transaction;
        }



        private ReportsTransactionsObject GetReportsTransactions(List<Invoices> listOfInvoices, List<Receipts> listOfReceipts, List<VOU_SETTING> VouSettingList)
        {
            ReportsTransactionsObject reportsTransactionsObject = new ReportsTransactionsObject();
            List<ReportsTransactions> reportsTransactions = new List<ReportsTransactions>();
            List<ReportsTransactionsTotal> reportsTransactionsTotal = new List<ReportsTransactionsTotal>();

            reportsTransactionsTotal.Add(new ReportsTransactionsTotal()
            {
                invoice_type = "555",
                invoice_name = new Languages()
                {
                    Ar = "المبالغ النقدية",
                    En = "Cash Amount"
                },
                invoice_short_name = new Languages()
                {
                    Ar = "المبالغ النقدية",
                    En = "Cash Amount"
                },
                total_amount = 0
            });
            reportsTransactionsTotal.Add(new ReportsTransactionsTotal()
            {
                invoice_type = "666",
                invoice_name = new Languages()
                {
                    Ar = "طرق الدفع",
                    En = "Payment Methods"
                },
                invoice_short_name = new Languages()
                {
                    Ar = "طرق الدفع",
                    En = "Payment Methods"
                },
                total_amount = 0
            });
            foreach (var vou_setting in VouSettingList)
            {
                reportsTransactionsTotal.Add(new ReportsTransactionsTotal()
                {
                    invoice_type = vou_setting.FTYPE,
                    invoice_name = vou_setting.NAME,
                    invoice_short_name = vou_setting.SHORT_NAME,
                    total_amount = 0
                });
            };
            reportsTransactionsTotal.Add(new ReportsTransactionsTotal()
            {
                invoice_type = "055",
                invoice_name = new Languages()
                {
                    Ar = "سند قبض",
                    En = "Receipt Voucher"
                },
                invoice_short_name = new Languages()
                {
                    Ar = "قبض",
                    En = "Receipt"
                },
                total_amount = 0
            });
            foreach (var invoice in listOfInvoices)
            {
                ReportsTransactions transaction = GetReportsTransactionsFromInvoice(invoice, ref reportsTransactionsTotal);
                if (transaction != null)
                    reportsTransactions.Add(transaction);
            }
            foreach (var receipt in listOfReceipts)
            {
                ReportsTransactions transaction = GetReportsTransactionsFromReceipt(receipt, ref reportsTransactionsTotal);
                if (transaction != null)
                    reportsTransactions.Add(transaction);
            }
            reportsTransactionsObject.transactions = reportsTransactions.OrderByDescending(t => t.date).ToList();
            reportsTransactionsObject.total = reportsTransactionsTotal;

            return reportsTransactionsObject;
        }


        private ReportsTransactionsObjectUser GetReportsTransactionsUser(List<Invoices> listOfInvoices, List<Receipts> listOfReceipts, List<VOU_SETTING> VouSettingList)
        {
            ReportsTransactionsObjectUser reportsTransactionsObject = new ReportsTransactionsObjectUser();
            List<ReportsTransactionsUser> reportsTransactions = new List<ReportsTransactionsUser>();
            List<ReportsTransactionsTotal> reportsTransactionsTotal = new List<ReportsTransactionsTotal>();
            Dictionary<string, decimal> totalSalesPerUser = new Dictionary<string, decimal>();
            reportsTransactionsTotal.Add(new ReportsTransactionsTotal()
            {
                invoice_type = "555",
                invoice_name = new Languages()
                {
                    Ar = "المبالغ النقدية",
                    En = "Cash Amount"
                },
                invoice_short_name = new Languages()
                {
                    Ar = "المبالغ النقدية",
                    En = "Cash Amount"
                },
                total_amount = 0
            });
            reportsTransactionsTotal.Add(new ReportsTransactionsTotal()
            {
                invoice_type = "666",
                invoice_name = new Languages()
                {
                    Ar = "طرق الدفع",
                    En = "Payment Methods"
                },
                invoice_short_name = new Languages()
                {
                    Ar = "طرق الدفع",
                    En = "Payment Methods"
                },
                total_amount = 0
            });
            foreach (var vou_setting in VouSettingList)
            {
                reportsTransactionsTotal.Add(new ReportsTransactionsTotal()
                {
                    invoice_type = vou_setting.FTYPE,
                    invoice_name = vou_setting.NAME,
                    invoice_short_name = vou_setting.SHORT_NAME,
                    total_amount = 0
                });
            };
            reportsTransactionsTotal.Add(new ReportsTransactionsTotal()
            {
                invoice_type = "055",
                invoice_name = new Languages()
                {
                    Ar = "سند قبض",
                    En = "Receipt Voucher"
                },
                invoice_short_name = new Languages()
                {
                    Ar = "قبض",
                    En = "Receipt"
                },
                total_amount = 0
            });
            foreach (var invoice in listOfInvoices)
            {
                string invoice_type = invoice.invoice_type;

                // Check if the user already exists in the list.
                ReportsTransactionsUser transaction = reportsTransactions.FirstOrDefault(x => x.localUser.user_id == invoice.user_id);
               
                if (transaction == null)
                {
                    // The user does not exist in the list, so create a new object.
                    transaction = new ReportsTransactionsUser()
                    {
                        id = invoice.id ,

                        localUser = invoice.localUser,
                        user_name = invoice.localUser.user_name,
                        CashSales = (invoice_type == "001" ? invoice.total_amount : 0),
                        ReturnSaleCash= (invoice_type == "003" ? invoice.total_amount : 0),
                        CreditSales = (invoice_type == "002" ? invoice.total_amount : 0),
                        ReturnSaleCredit = (invoice_type == "004" ? invoice.total_amount : 0),
                        totalRecipt=0,
                        Total_cash= (invoice_type == "001" ? invoice.total_amount : 0) - (invoice_type == "003" ? invoice.total_amount : 0)
                    };
                    reportsTransactions.Add(transaction);
                }
                else
                {
                    // The user already exists in the list, so update the existing object.
                    transaction.CashSales += (invoice_type == "001" ? invoice.total_amount : 0);
                    transaction.ReturnSaleCash += (invoice_type == "003" ? invoice.total_amount : 0);
                    transaction.CreditSales += (invoice_type == "002" ? invoice.total_amount : 0);
                    transaction.ReturnSaleCredit += (invoice_type == "004" ? invoice.total_amount : 0);
                    transaction.Total_cash+=(invoice_type == "001" ? invoice.total_amount : 0) - (invoice_type == "003" ? invoice.total_amount : 0);
                }
            
        }
            foreach (var receipt in listOfReceipts)
            {
                //The user already exists in the list, so update the existing object.
                ReportsTransactionsUser transaction = reportsTransactions.FirstOrDefault(x => x.localUser.user_id == receipt.user_id);
                if (transaction != null)
                {
                    transaction.totalRecipt += (receipt.receipt_amount);
                }
               

            }
                reportsTransactionsObject.transactions = reportsTransactions.ToList();
            reportsTransactionsObject.total = reportsTransactionsTotal;

            return reportsTransactionsObject;
        }




        public async Task<ReportsTransactionsObjectResponse> GetTransaction(int user_id/*, ReportsFilter reportsFilter*/)
        {
            ReportsTransactionsObject reportsTransactionsObject = new ReportsTransactionsObject();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                var invoicesList = await _context.invoices
                    .Where(x =>  invoice_types.Contains(x.invoice_type))
                    .Include(i => i.customer)
                    .Include(i => i.localUser)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .OrderByDescending(i => i.invoice_date)
                    .ToListAsync();
                var receiptsList = await _context.receipts
                    .Include(i => i.customer)
                      .Include(i => i.localUser)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .OrderByDescending(i => i.receipt_date)
                    .ToListAsync();
                var vou_settingList = await _context.vou_setting
                    .Where(s => invoice_types.Contains(s.FTYPE) && s.FTYPE2 == 0)
                    .ToListAsync();
              
                reportsTransactionsObject = GetReportsTransactions(invoicesList, receiptsList, vou_settingList);
                error_code = 200;
                status = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ReportsTransactionsObjectResponse(reportsTransactionsObject, status, message_laguage, error_code);
        }

       
        public async Task<ReportsTransactionsObjectResponse> GetTotalUsers(int  user_Id,DateTime start ,DateTime end/*, ReportsFilter reportsFilter*/)
    {
        ReportsTransactionsObject reportsTransactionsObject = new ReportsTransactionsObject();
        bool status = false;
        string message = "error";
        int error_code = 400;
        try
        {
                if(start ==null&& end == null&& user_Id!=0)
                {
                    List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                    var invoicesList = await _context.invoices
                        .Where(x => invoice_types.Contains(x.invoice_type) && (x.user_id == user_Id))

                        .Include(i => i.customer)
                        //.Include("customer.invoices")
                        .Include(i => i.localUser)
                        //.Include("customer.receipts")
                        .OrderByDescending(i => i.invoice_date)
                        .ToListAsync();
                    var receiptsList = await _context.receipts
                        .Where(x => /*&& ((reportsFilter.from_date == null) ? true : global.CovertTimeZone(x.receipt_date ?? DateTime.UtcNow) >= global.CovertTimeZone(reportsFilter.from_date ?? DateTime.UtcNow)) && ((reportsFilter.to_date == null) ? true : global.CovertTimeZone(x.receipt_date ?? DateTime.UtcNow) <= global.CovertTimeZone(reportsFilter.to_date ?? DateTime.UtcNow))*/(x.user_id == user_Id))
                        .Include(i => i.customer)
                        //.Include("customer.invoices")
                        .Include(i => i.localUser)
                        //.Include("customer.receipts")
                        .OrderByDescending(i => i.receipt_date)
                        .ToListAsync();
                    var vou_settingList = await _context.vou_setting
                        .Where(s => invoice_types.Contains(s.FTYPE) && s.FTYPE2 == 0)
                        .ToListAsync();
                    reportsTransactionsObject = GetReportsTransactions(invoicesList, receiptsList, vou_settingList);
                    error_code = 200;
                    status = true;
                }
                else
                {
                    List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                    var invoicesList = await _context.invoices
                        .Where(x => invoice_types.Contains(x.invoice_type) && (x.user_id == user_Id) &&  (x.invoice_date >= global.CovertTimeZone(start) && x.invoice_date <= global.CovertTimeZone(end)))
                        .Include(i => i.customer)
                        //.Include("customer.invoices")
                        .Include(i => i.localUser)
                        //.Include("customer.receipts")
                        .OrderByDescending(i => i.invoice_date)
                        .ToListAsync();
                    var receiptsList = await _context.receipts
                        .Where(x => (x.user_id == user_Id) && (x.receipt_date >= global.CovertTimeZone(start) && x.receipt_date <= global.CovertTimeZone(end)))
                        .Include(i => i.customer)
                        //.Include("customer.invoices")
                        .Include(i => i.localUser)
                        //.Include("customer.receipts")
                        .OrderByDescending(i => i.receipt_date)
                        .ToListAsync();
                    var vou_settingList = await _context.vou_setting
                        .Where(s => invoice_types.Contains(s.FTYPE) && s.FTYPE2 == 0)
                        .ToListAsync();
                    reportsTransactionsObject = GetReportsTransactions(invoicesList, receiptsList, vou_settingList);
                    error_code = 200;
                    status = true;
                }
           
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }

        Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
        return new ReportsTransactionsObjectResponse(reportsTransactionsObject, status, message_laguage, error_code);
    }

        public async Task<ReportsTransactionsObjectResponse> GetTotalUsersId(int user_Id/*, ReportsFilter reportsFilter*/)
        {
            ReportsTransactionsObject reportsTransactionsObject = new ReportsTransactionsObject();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                if ( user_Id != 0)
                {
                    List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                    var invoicesList = await _context.invoices
                        .Where(x => invoice_types.Contains(x.invoice_type) && (x.user_id == user_Id))

                        .Include(i => i.customer)
                        //.Include("customer.invoices")
                        .Include(i => i.localUser)
                        //.Include("customer.receipts")
                        .OrderByDescending(i => i.invoice_date)
                        .ToListAsync();
                    var receiptsList = await _context.receipts
                        .Where(x => /*&& ((reportsFilter.from_date == null) ? true : global.CovertTimeZone(x.receipt_date ?? DateTime.UtcNow) >= global.CovertTimeZone(reportsFilter.from_date ?? DateTime.UtcNow)) && ((reportsFilter.to_date == null) ? true : global.CovertTimeZone(x.receipt_date ?? DateTime.UtcNow) <= global.CovertTimeZone(reportsFilter.to_date ?? DateTime.UtcNow))*/(x.user_id == user_Id))
                        .Include(i => i.customer)
                        //.Include("customer.invoices")
                        .Include(i => i.localUser)
                        //.Include("customer.receipts")
                        .OrderByDescending(i => i.receipt_date)
                        .ToListAsync();
                    var vou_settingList = await _context.vou_setting
                        .Where(s => invoice_types.Contains(s.FTYPE) && s.FTYPE2 == 0)
                        .ToListAsync();
                    reportsTransactionsObject = GetReportsTransactions(invoicesList, receiptsList, vou_settingList);
                    error_code = 200;
                    status = true;
                }
                else
                {
                    List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                    var invoicesList = await _context.invoices
                        .Where(x => invoice_types.Contains(x.invoice_type)  )
                        .Include(i => i.customer)
                        //.Include("customer.invoices")
                        .Include(i => i.localUser)
                        //.Include("customer.receipts")
                        .OrderByDescending(i => i.invoice_date)
                        .ToListAsync();
                    var receiptsList = await _context.receipts
                        .Where(x => (x.user_id == user_Id) )
                        .Include(i => i.customer)
                        //.Include("customer.invoices")
                        .Include(i => i.localUser)
                        //.Include("customer.receipts")
                        .OrderByDescending(i => i.receipt_date)
                        .ToListAsync();
                    var vou_settingList = await _context.vou_setting
                        .Where(s => invoice_types.Contains(s.FTYPE) && s.FTYPE2 == 0)
                        .ToListAsync();
                    reportsTransactionsObject = GetReportsTransactions(invoicesList, receiptsList, vou_settingList);
                    error_code = 200;
                    status = true;
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ReportsTransactionsObjectResponse(reportsTransactionsObject, status, message_laguage, error_code);
        }
        public async Task<localUserResponse> GetAllUser(int user_id)
        {
            List<LocalUser> productsObject = new List<LocalUser>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                productsObject = await _context.localUser .ToListAsync();
                    //productsObject = GetProductsObjectListFromInvoices(warehouseList);
                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new localUserResponse(productsObject, status, message_laguage, error_code);
        }


        public async Task<ReportsTransactionsObjectResponseUser> GetTransactionUser(int user_id)
        {
            ReportsTransactionsObjectUser reportsTransactionsObject = new ReportsTransactionsObjectUser();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                var invoicesList = await _context.invoices
                    .Where(x => invoice_types.Contains(x.invoice_type))
                    .Include(i => i.customer)
                    .Include(i => i.localUser)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .OrderByDescending(i => i.invoice_date)
                    .ToListAsync();
                var receiptsList = await _context.receipts
                    .Include(i => i.customer)
                      .Include(i => i.localUser)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .OrderByDescending(i => i.receipt_date)
                    .ToListAsync();
                var vou_settingList = await _context.vou_setting
                    .Where(s => invoice_types.Contains(s.FTYPE) && s.FTYPE2 == 0)
                    .ToListAsync();

                reportsTransactionsObject = GetReportsTransactionsUser(invoicesList, receiptsList, vou_settingList);
                error_code = 200;
                status = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ReportsTransactionsObjectResponseUser(reportsTransactionsObject, status, message_laguage, error_code);
        }
        public async Task<ReportsTransactionsObjectResponseUser> GetTransactionUserToday(int user_id)
        {
            ReportsTransactionsObjectUser reportsTransactionsObject = new ReportsTransactionsObjectUser();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                var invoicesList = await _context.invoices
                    .Where(x => invoice_types.Contains(x.invoice_type) && x.invoice_date ==DateTime.UtcNow)
                    .Include(i => i.customer)
                    .Include(i => i.localUser)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .OrderByDescending(i => i.invoice_date)
                    .ToListAsync();
                var receiptsList = await _context.receipts
                      .Where(x => x.receipt_date  == DateTime.UtcNow)
                    .Include(i => i.customer)
                      .Include(i => i.localUser)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .OrderByDescending(i => i.receipt_date)
                    .ToListAsync();
                var vou_settingList = await _context.vou_setting
                    .Where(s => invoice_types.Contains(s.FTYPE) && s.FTYPE2 == 0)
                    .ToListAsync();

                reportsTransactionsObject = GetReportsTransactionsUser(invoicesList, receiptsList, vou_settingList);
                error_code = 200;
                status = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ReportsTransactionsObjectResponseUser(reportsTransactionsObject, status, message_laguage, error_code);
        }
        public async Task<ReportsTransactionsObjectResponseUser> GetTransactionUserdate(int user_id,DateTime start,DateTime end)
        {
            ReportsTransactionsObjectUser reportsTransactionsObject = new ReportsTransactionsObjectUser();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
           {
                List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
                var invoicesList = await _context.invoices
                    .Where(x => invoice_types.Contains(x.invoice_type) && x.invoice_date >= global.CovertTimeZone(start) && x.invoice_date <= global.CovertTimeZone(end)
                  
                     )
                    .Include(i => i.customer)
                    .Include(i => i.localUser)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .OrderByDescending(i => i.invoice_date)
                    .ToListAsync();
                var receiptsList = await _context.receipts
                    .Where(x =>  x.receipt_date >= global.CovertTimeZone(start) && x.receipt_date <= global.CovertTimeZone(end))
                    .Include(i => i.customer)
                      .Include(i => i.localUser)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .OrderByDescending(i => i.receipt_date)
                    .ToListAsync();
                var vou_settingList = await _context.vou_setting
                    .Where(s =>  invoice_types.Contains(s.FTYPE) && s.FTYPE2 == 0)
                    .ToListAsync();

                reportsTransactionsObject = GetReportsTransactionsUser(invoicesList, receiptsList, vou_settingList);
                error_code = 200;
                status = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ReportsTransactionsObjectResponseUser(reportsTransactionsObject, status, message_laguage, error_code);
        }


    }

}

