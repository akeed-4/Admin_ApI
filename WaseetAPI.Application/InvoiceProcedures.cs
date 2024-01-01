using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using WaseetAPI.Domain.TimerFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNet.SignalR.Messaging;

namespace WaseetAPI.Application
{
    public class InvoiceProcedures
    {
        private ApplicationDbContext _context;
        private string _connectionStr;
        private GlobalProcedures global;
        public InvoiceProcedures(string connectionStr)
        {
            this._context = new ApplicationDbContext(connectionStr);
            this._connectionStr = connectionStr;
            this.global = new GlobalProcedures();
        }
        public async Task<VOU_SETTINGListResponse> GetTypeNameList()
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            List<VOU_SETTING> listOfVOU_SETTING = new List<VOU_SETTING>();
            try
            {
                listOfVOU_SETTING = await _context.vou_setting.ToListAsync();
                //listOfVOU_SETTING = GetVOU_SETTINGObjectListFromVOU_SETTING(vou_setting);
                error_code = 200;
                status = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new VOU_SETTINGListResponse(listOfVOU_SETTING, status, message_laguage, error_code);
        }
        //private VOU_SETTING GetVOU_SETTINGObjectFromVOU_SETTING(VOU_SETTING vou_setting)
        //{
        //    return new VOU_SETTING()
        //    {
        //        FTYPE = vou_setting.FTYPE,
        //        NAME = new Languages()
        //        {
        //            Ar = vou_setting.NAME1,
        //            En = vou_setting.NAME2
        //        },
        //        SHORT_NAME = new Languages()
        //        {
        //            Ar = vou_setting.SHORT_NAME1,
        //            En = vou_setting.SHORT_NAME2
        //        }
        //    };
        //}

        //private List<VOU_SETTING> GetVOU_SETTINGObjectListFromVOU_SETTING(List<VOU_SETTING> listOfVOU_SETTING)
        //{
        //    List<VOU_SETTING> listOfVOU_SETTINGObject = new List<VOU_SETTING>();
        //    foreach (var vou_setting in listOfVOU_SETTING)
        //    {
        //        VOU_SETTING vou_settingObject = GetVOU_SETTINGObjectFromVOU_SETTING(vou_setting);
        //        listOfVOU_SETTINGObject.Add(vou_settingObject);
        //    }
        //    return listOfVOU_SETTINGObject;
        //}
        private async Task<VOU_SETTINGResponse> GetInvoiceNameFromType(string invoice_type)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            VOU_SETTING vou_SettingObject = new VOU_SETTING();
            //Languages invoice_name = new Languages();
            //Languages short_invoice_name = new Languages();
            //Languages btc_title = new Languages();
            //Languages btb_title = new Languages();
            try
            {
                vou_SettingObject = await _context.vou_setting
                    .Where(v => v.FTYPE == invoice_type && v.FTYPE2 == 0)
                    .FirstOrDefaultAsync();
                if (vou_SettingObject == null)
                //{
                //    invoice_name.Ar = vou_setting.NAME1;
                //    invoice_name.En = vou_setting.NAME2;
                //    short_invoice_name.Ar = vou_setting.SHORT_NAME1;
                //    short_invoice_name.En = vou_setting.SHORT_NAME2;
                //    btc_title.Ar = vou_setting.SMALL_PRINTER_TITLEBTC;
                //    btc_title.En = vou_setting.SMALL_PRINTER_TITLEBTC2;
                //    btb_title.Ar = vou_setting.SMALL_PRINTER_TITLEBTB;
                //    btb_title.En = vou_setting.SMALL_PRINTER_TITLEBTB2;
                //}
                //else
                {
                    vou_SettingObject = new VOU_SETTING()
                    {
                        FTYPE = invoice_type,                        
                        NAME1 = "",
                        NAME2 = "",
                        SHORT_NAME1 = "",
                        SHORT_NAME2 = "",
                        SMALL_PRINTER_TITLEBTC = "فاتورة ضريبية مبسطة",
                        SMALL_PRINTER_TITLEBTC2 = "Simplified Tax Invoice",
                        SMALL_PRINTER_TITLEBTB = "فاتورة ضريبية",
                        SMALL_PRINTER_TITLEBTB2 = "Tax Invoice"
                    };
                    //invoice_name.Ar = "";
                    //invoice_name.En = "";
                    //short_invoice_name.Ar = "";
                    //short_invoice_name.En = "";
                    //btc_title.Ar = "فاتورة ضريبية مبسطة";
                    //btc_title.En = "Simplified Tax Invoice";
                    //btb_title.Ar = "فاتورة ضريبية";
                    //btb_title.En = "Tax Invoice";
                }
                //vou_SettingObject = new VOU_SETTINGObject()
                //{
                //    FTYPE = invoice_type,
                //    NAME = invoice_name,
                //    SHORT_NAME = short_invoice_name,
                //    BTC_TITLE = btc_title,
                //    BTB_TITLE = btb_title
                //};
                error_code = 200;
                status = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new VOU_SETTINGResponse(vou_SettingObject, status, message_laguage, error_code);
        }
        public async Task<SERIALSRespose> GetInvoicesSerial(int user_id)
        {
            //string cashStr = "001"; string creditStr = "002";
            //string ret_cashStr = "003"; string ret_creditStr = "004";
            //string receiptStr = "055";
            List<SERIALS> serials = new List<SERIALS>();
            //serials.Add(new SERIALS() { invoice_type = cashStr, user_last_serial = 1 });
            //serials.Add(new SERIALS() { invoice_type = creditStr, user_last_serial = 1 });
            //serials.Add(new SERIALS() { invoice_type = ret_cashStr, user_last_serial = 1 });
            //serials.Add(new SERIALS() { invoice_type = ret_creditStr, user_last_serial = 1 });
            //serials.Add(new SERIALS() { invoice_type = receiptStr, user_last_serial = 1 });
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                UserStoreResponse userStore = await GetUserStore(user_id);
                if (userStore.status)
                {
                    serials = await _context.serials
                    .Where(x => x.BRAN == userStore.data.bran && x.BTYPE == userStore.data.ftype && x.CODE == userStore.data.code && x.FTYPE2 == 0)
                    .ToListAsync();
                    //string invoice_type = "";
                    //foreach (var row in serialsList)
                    //{
                    //    invoice_type = "";
                    //    switch (row.FTYPE)
                    //    {
                    //        case "001":
                    //            invoice_type = cashStr;
                    //            break;
                    //        case "002":
                    //            invoice_type = creditStr;
                    //            break;
                    //        case "003":
                    //            invoice_type = ret_cashStr;
                    //            break;
                    //        case "004":
                    //            invoice_type = ret_creditStr;
                    //            break;
                    //        case "055":
                    //            invoice_type = receiptStr;
                    //            break;
                    //    }
                    //    if (invoice_type != "")
                    //        serials.Find(i => i.invoice_type == invoice_type).user_last_serial = row.SERIAL ?? 1;
                    //}
                    if (!serials.Any(s => s.FTYPE == "001"))
                        serials.Add(new SERIALS() { FTYPE = "001", SERIAL = 1 });
                    if (!serials.Any(s => s.FTYPE == "002"))
                        serials.Add(new SERIALS() { FTYPE = "002", SERIAL = 1 });
                    if (!serials.Any(s => s.FTYPE == "003"))
                        serials.Add(new SERIALS() { FTYPE = "003", SERIAL = 1 });
                    if (!serials.Any(s => s.FTYPE == "004"))
                        serials.Add(new SERIALS() { FTYPE = "004", SERIAL = 1 });
                    if (!serials.Any(s => s.FTYPE == "055"))
                        serials.Add(new SERIALS() { FTYPE = "055", SERIAL = 1 });
                    if (!serials.Any(s => s.FTYPE == "204"))
                        serials.Add(new SERIALS() { FTYPE = "204", SERIAL = 1 });
                    if (!serials.Any(s => s.FTYPE == "113"))
                        serials.Add(new SERIALS() { FTYPE = "113", SERIAL = 1 });
                    error_code = 200;
                    status = true;
                }
                else
                {
                    error_code = userStore.error_code;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new SERIALSRespose(serials, status, message_laguage, error_code);
        }
        public async Task<MobilePermissionsRespose> GetUserPermissions(int user_id)
        {
            List<MobilePermissions> permissions = new List<MobilePermissions>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                permissions = await _context.mobilepermissions
                   .Where(x => x.user_id == user_id)
                   .ToListAsync();
                error_code = 200;
                status = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MobilePermissionsRespose(permissions, status, message_laguage, error_code);
        }
        
        public async Task<WSHOWResponse> GetCompanyInfo(int user_id)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            WSHOW wshow = new WSHOW();
            try
            {
                UserStore userStore = await _context.userStore
                    .Where(u => u.user_id == user_id)
                    .FirstOrDefaultAsync();
                if (userStore != null)
                {
                    var company_info = await _context.wshow
                        .Where(x => x.ftype == "0" && x.bran == userStore.bran && x.code == userStore.bran)
                        .FirstOrDefaultAsync();
                    if (company_info != null)
                    {
                        wshow.name = company_info.name;
                        wshow.tax_id = company_info.tax_id;
                        wshow.address = company_info.address;
                        wshow.tel = company_info.tel;
                        wshow.fax = company_info.fax;
                        wshow.DSEGAL = company_info.DSEGAL;
                        wshow.CANTRY = company_info.CANTRY;
                        wshow.CITY = company_info.CITY;
                        wshow.LOCALAREA = company_info.LOCALAREA;
                        wshow.DBOXID = company_info.DBOXID;
                        wshow.DBULDINGID = company_info.DBULDINGID;
                        wshow.INFO1 = company_info.INFO1;
                        error_code = 200;
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new WSHOWResponse(wshow, status, message_laguage, error_code);
        }
        public async Task<UserStoreResponse> GetUserStore(int user_id)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            UserStore userStoreRespose = new UserStore();
            try
            {
                UserStore userStore = await _context.userStore
                    .Where(u => u.user_id == user_id)
                    .FirstOrDefaultAsync();
                if (userStore != null)
                {
                    userStoreRespose = new UserStore()
                    {
                        id = userStore.id,
                        user_id = userStore.user_id,
                        bran = userStore.bran,
                        code = userStore.code,
                        ftype = userStore.ftype
                    };
                    error_code = 200;
                    status = true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new UserStoreResponse(userStoreRespose, status, message_laguage, error_code);
        }

        //private async Task<ProductsObject> GetProductObjectFromProduect(Products product, int user_id, bool calc_available_quantity = true, double available_quantity = 0, double current_price = 0)
        //{
        //    if (calc_available_quantity)
        //    {
        //        UsersWarehouse usersWarehouse = await GetAvailableQuantity(user_id, product.product_id);
        //        if (usersWarehouse != null)
        //        {
        //            available_quantity = (usersWarehouse.available_quantity ?? 0);
        //            current_price = (usersWarehouse.current_price ?? 0);
        //        }
        //    }

        //    ProductsObject productsObject = new ProductsObject()
        //    {
        //        product_id = product.product_id,
        //        product_image = product.product_image,
        //        product_description = product.product_description,
        //        available_quantity = available_quantity,
        //        price = current_price,
        //        product_name = new Languages()
        //        {
        //            Ar = product.product_name,
        //            En = product.product_ename
        //        }
        //    };
        //    return productsObject;
        //}
       
        //public async Task<UsersWarehouse> GetAvailableQuantity(int user_id, string product_id)
        //{
        //    return await _context.usersWarehouse.Where(p => p.user_id == user_id && p.product_id == product_id).FirstOrDefaultAsync();
        //}

        
        //private void GetInvoiceObjectFromInvoice(ref Invoices invoice)
        //{
        //    if (invoice.products != null)
        //    {
        //        int user_id = invoice.user_id??0;
        //        foreach (var p in invoice.products)
        //        {
        //            if (p.product_data != null)
        //                if (p.product_data.userswarehouse != null)
        //                {
        //                    p.product_data.price = p.product_data.userswarehouse.Where(w => w.user_id == user_id).FirstOrDefault().current_price;
        //                    p.product_data.available_quantity = p.product_data.userswarehouse.Where(w => w.user_id == user_id).FirstOrDefault().available_quantity;
        //                }
        //        }
        //    }
        //}
        //private async Task<InvoicesObject> GetInvoiceObjectFromInvoice(Invoices invoice,double current_tax_per = 0)
        //{
        //    List<InvoicesProductsObject> listOfProducts = new List<InvoicesProductsObject>();
        //    List<ReceiptsInvoicesObject> listOfReceipts = new List<ReceiptsInvoicesObject>();
        //    CustomersObject customersObject = new CustomersObject();

        //    //List of Products Object
        //    if (invoice.products != null)
        //    {
        //        foreach (var product in invoice.products)
        //        {
        //            ProductsObject product_data = await GetProductObjectFromProduect(product.product_data, invoice.user_id ?? 0);                    
        //            InvoicesProductsObject invoicesProducts = new InvoicesProductsObject()
        //            {
        //                product_data = product_data,
        //                price = product.price,
        //                quantity = product.quantity
        //            };
        //            listOfProducts.Add(invoicesProducts);
                        
        //            if (current_tax_per == 0 && (product.product_data.tax_per ?? 0) != 0)                            
        //                current_tax_per = (product.product_data.tax_per ?? 0);
        //        }
        //    }            
        //    //List of Receipts Object
        //    if (invoice.receipts != null)
        //    {
        //        foreach (var receipt in invoice.receipts)
        //        {
        //            ReceiptsInvoicesObject receiptsInvoices = new ReceiptsInvoicesObject()
        //            {
        //                receipt = GetReceiptNoInvoicesObjectFromReceipt(receipt.receipts),
        //                paid_amount = receipt.paid_amount
        //            };
        //            listOfReceipts.Add(receiptsInvoices);
        //        }
        //    }
        //    //Customer Object
        //    if (invoice.customer != null)
        //    {
        //        customersObject = new CustomerProcedures(_connectionStr).GetCustomersObjectFromCustomer(invoice.customer);                
        //    }
        //    string invoice_type = invoice.invoice_type;
        //    if (invoice_type != "001" && invoice_type != "002" && invoice_type != "003" && invoice_type != "004")
        //        invoice_type = null;
        //    //VOU_SETTINGResponse vou_SettingObjectResponse = await GetInvoiceNameFromType(invoice_type);
        //    double amount_before_tax = (invoice.total_amount ?? 0) - (invoice.tax_amount ?? 0);
        //    amount_before_tax = Math.Round(amount_before_tax, 2);
        //    InvoicesObject invoiceObject = new InvoicesObject()
        //    {
        //        id = invoice.id,
        //        invoice_no = invoice.invoice_no,
        //        invoice_date = global.CovertTimeZone(invoice.invoice_date ?? DateTime.UtcNow),
        //        invoice_type = invoice_type,
        //        invoice_name = new Languages()
        //        {
        //            Ar = invoice.vou_setting.NAME1,
        //            En = invoice.vou_setting.NAME2
        //        },
        //        //invoice.invoice_name.NAME1
        //        invoice_short_name = new Languages()
        //        {
        //            Ar = invoice.vou_setting.SHORT_NAME1,
        //            En = invoice.vou_setting.SHORT_NAME2
        //        },
        //        //vou_SettingObjectResponse.data.SHORT_NAME,
        //        btc_title = new Languages()
        //        {
        //            Ar = invoice.vou_setting.SMALL_PRINTER_TITLEBTC,
        //            En = invoice.vou_setting.SMALL_PRINTER_TITLEBTC2
        //        },
        //        //vou_SettingObjectResponse.data.BTC_TITLE,
        //        btb_title = new Languages()
        //        {
        //            Ar = invoice.vou_setting.SMALL_PRINTER_TITLEBTB,
        //            En = invoice.vou_setting.SMALL_PRINTER_TITLEBTB2
        //        },
        //        //vou_SettingObjectResponse.data.BTB_TITLE,
        //        customer = customersObject,
        //        products = listOfProducts,
        //        total_amount = invoice.total_amount,
        //        paid_amount = invoice.paid_amount,
        //        invoice_acceptance = invoice.invoice_acceptance,
        //        amount_before_tax = amount_before_tax,
        //        tax_per = current_tax_per,
        //        tax_amount = invoice.tax_amount,
        //        qr_data = invoice.qr_data,
        //        receipts = listOfReceipts
        //    };
        //    return invoiceObject;
        //}
        private ReceiptsNoInvoicesObject GetReceiptNoInvoicesObjectFromReceipt(Receipts receipt)
        {
            ReceiptsNoInvoicesObject receiptsNoInvoicesObject = new ReceiptsNoInvoicesObject()
            {
                id = receipt.id,
                receipt_no = receipt.receipt_no,
                receipt_date = global.CovertTimeZone(receipt.receipt_date ?? DateTime.UtcNow),
                receipt_amount = receipt.receipt_amount
            };
            return receiptsNoInvoicesObject;
        }
        //private void GetInvoicesObjectFromInvoices(ref List<Invoices> listOfInvoices)
        //{
        //    foreach (var invoice in listOfInvoices)
        //    {
        //        if (invoice.products != null)
        //        {
        //            foreach (var p in invoice.products)
        //            {
        //                if (p.product_data != null)
        //                    if (p.product_data.userswarehouse != null)
        //                    {
        //                        p.product_data.price = p.product_data.userswarehouse.Where(w => w.user_id == invoice.user_id).FirstOrDefault().current_price;
        //                        p.product_data.available_quantity = p.product_data.userswarehouse.Where(w => w.user_id == invoice.user_id).FirstOrDefault().available_quantity;
        //                    }
        //            }
        //        }
        //    }

        //    //List<IInvoices> listOfInvoicesObject = new List<IInvoices>();
        //    //listOfInvoicesObject = listOfInvoices.Cast<IInvoices>().ToList();
        //    ////foreach (var invoice in listOfInvoices)
        //    ////{
        //    ////    InvoicesObject invoiceObject = await GetInvoiceObjectFromInvoice(invoice, current_tax_per);
        //    ////    listOfInvoicesObject.Add(invoiceObject);
        //    ////}
        //    //return listOfInvoicesObject;
        //}
        private List<Products> GetProductsObjectListFromInvoices(List<UsersWarehouse> listOfWarehouse)
        {
            List<Products> listOfProductsObject = new List<Products>();
            foreach (var product in listOfWarehouse)
            {
                product.product_data.available_quantity = product.available_quantity;
                product.product_data.price = product.current_price;
                listOfProductsObject.Add(product.product_data);
            }
            //foreach (var product in listOfWarehouse)
            //{
            //    //ProductsObject product_data = await GetProductObjectFromProduect(product.product_data, 0, false, product.available_quantity ?? 0, product.current_price ?? 0);
            //    IProducts product_data = product.product_data;
            //    product_data.available_quantity = product.available_quantity;
            //    product_data.price = product.current_price;
            //    listOfProductsObject.Add(product_data);
            //}
            return listOfProductsObject;
        }                                  

        public async Task<InvoicesObjectListResponse> GetInvoices(int user_id, string invoice_type)
        {
            List<Invoices> invoices = new List<Invoices>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                List<string> invoice_types = new List<string>();
                if (invoice_type == null || invoice_type == "")
                {
                    invoice_types.Add("001");
                    invoice_types.Add("002");
                    invoice_types.Add("003");
                    invoice_types.Add("004");
                }
                else
                {
                    int found = 0;
                    found = invoice_type.IndexOf(",");
                    while (found > 0)
                    {
                        invoice_types.Add(invoice_type.Substring(0, found));
                        invoice_type = invoice_type.Substring(found + 1);
                        found = invoice_type.IndexOf(",");
                    }
                      
                    if (invoice_type != null && invoice_type != "")
                        invoice_types.Add(invoice_type);
                }
                
                invoices = await _context.invoices
                    .Where(x => invoice_types.Contains(x.invoice_type) && x.invoice_acceptance != 2 && x.invoice_status == true && ((x.invoice_tobaseet != 1) || (x.invoice_type == "002" && x.paid_amount < x.total_amount)))
                    .Include(i => i.products)
                    .Include("products.product_data")
                    .Include("products.product_data.userswarehouse")                    
                    .Include(i => i.customer)       
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .Include(i => i.receipts)
                    .Include("receipts.receipts")
                    .Include(i => i.vou_setting)
                    .OrderByDescending(i => i.invoice_date)
                    .ToListAsync();
                //GetInvoicesObjectFromInvoices(ref invoices);
                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
           
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new InvoicesObjectListResponse(invoices, status, message_laguage, error_code);
        }

        public async Task<InvoicesObjectListResponse> webGetInvoices( string invoice_type)
        {
         
            List<Invoices> invoices = new List<Invoices>();
         
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                List<string> invoice_types = new List<string>();
                if (invoice_type == null || invoice_type == "")
                {
                    invoice_types.Add("001");
                    invoice_types.Add("002");
                    invoice_types.Add("003");
                    invoice_types.Add("004");
                }
                else
                {
                    int found = 0;
                    found = invoice_type.IndexOf(",");
                    while (found > 0)
                    {
                        invoice_types.Add(invoice_type.Substring(0, found));
                        invoice_type = invoice_type.Substring(found + 1);
                        found = invoice_type.IndexOf(",");
                    }

                    if (invoice_type != null && invoice_type != "")
                        invoice_types.Add(invoice_type);
                }

                invoices = await _context.invoices
                    .Where(x => invoice_types.Contains(x.invoice_type))
                    //.Include(i => i.products)
                    //.Include("products.product_data")
                    //.Include("products.product_data.userswarehouse")
                    .Include(i => i.customer)
                    .Include(i => i.localUser)
                    //.Include(i => i.Users)
                    //.Include(i => i.receipts)
                    //.Include("receipts.receipts")
                    //.Include(i => i.vou_setting)
                    .OrderByDescending(i => i.invoice_date)
                    .ToListAsync();
            
                //GetInvoicesObjectFromInvoices(ref invoices);
                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new InvoicesObjectListResponse(invoices, status, message_laguage, error_code);
        }

        public async Task<MessageResponse> ReCheckTaxes()
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                List<string> invoice_types = new List<string>();
                invoice_types.Add("001");
                invoice_types.Add("002");
                invoice_types.Add("003");
                invoice_types.Add("004");
                List<string> invioce_state = new List<string>();

                var invoicesList = await _context.invoices
                    .Where(x => invoice_types.Contains(x.invoice_type) && x.invoice_acceptance != 2 && x.invoice_status == true)
                    .Include(i => i.products)
                    .Include("products.product_data")
                    .Include("products.product_data.userswarehouse")
                    .Include(i => i.customer)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .Include(i => i.receipts)
                    .Include("receipts.receipts")
                    .Include(i => i.vou_setting)
                    .OrderByDescending(i => i.invoice_date)
                    .ToListAsync();
                foreach (var invoice in invoicesList)
                {
                    if ((invoice.tax_amount ?? 0) == 0 || invoice.qr_data == null)
                    {
                        Invoices updateinvoice = await _context.invoices
                            .Where(i => i.id == invoice.id)
                            .FirstOrDefaultAsync();
                        if ((invoice.tax_amount ?? 0) == 0)
                        {
                            double ss = (invoice.total_amount ?? 0) / 1.15;
                            ss = (invoice.total_amount ?? 0) - ss;
                            updateinvoice.tax_amount = ss;
                        }
                        if (invoice.qr_data == null)
                            updateinvoice.qr_data = await getqr_data(invoice);
                        await _context.SaveChangesAsync();
                    }                        
                }
                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);            
        }
        public async Task<InvoicesObjectResponse> CreateInvoice(int user_id, Invoices invoice)
        {
            bool status = false;
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, "error");
            //InvoicesObject invoicesObject = new InvoicesObject();
            Invoices invoicesObject = new Invoices();
            try
            {
                if (invoice != null)
                {
                    double total_amount = 0;
                    double tax_amount = 0;
                    double curr_tax_amount = 0;
                    invoice.user_id = user_id;
                    invoice.invoice_status = true;
                    invoice.invoice_type2 = 0;
                    invoice.invoice_tobaseet = 0;
                    invoice.invoice_acceptance = 0;
                    DateTime current_invoice_date = invoice.invoice_date ?? DateTime.UtcNow;
                    invoice.invoice_date = global.CovertTimeZone(current_invoice_date);
                    invoice.invoice_no = 0;
                    invoice.customer = null;
                    //if (invoice.customer != null)
                    //{
                    //    invoice.customer_id = invoice.customer.customer_id;
                    //    invoice.customers = null;
                    //}

                    if ((invoice.invoice_type == "003" || invoice.invoice_type == "004")) 
                    {
                        if (invoice.return_id == null && invoice.id != null)
                            invoice.return_id = invoice.id;
                        invoice.id = null;
                        MessageResponse checkReturn = await CheckReturnInvoice(invoice);
                        if(checkReturn.status)
                        {
                            if (checkReturn.error_code != 0)
                                invoice.customer_id = checkReturn.error_code;
                        }
                        else
                        {
                            status = checkReturn.status;
                            message_laguage = checkReturn.message;
                            error_code = checkReturn.error_code;
                            return new InvoicesObjectResponse(invoicesObject, status, message_laguage, error_code);
                        }
                    }
                    else
                    {
                        invoice.id = null;
                        invoice.return_id = null;
                    }
                    if (invoice.receipts != null)
                    {
                        invoice.receipts = null;
                    }
                    MessageResponse customerCheck = await new CustomerProcedures(_connectionStr).CheckCustomerExits(user_id, invoice.customer_id);
                    if ((!customerCheck.status) && (invoice.invoice_type == "113"))
                    {
                        invoice.customer_id = null;
                        customerCheck.status = true;
                    }
                    if (customerCheck.status)
                    {
                        double curr_amount_before_tax = 0;
                        double curr_amount_after_tax = 0;
                        double curr_tax_per = 0;
                        double tax_per = 0;
                        foreach (InvoicesProducts invoice_products in invoice.products)
                        {
                            invoice_products.id = null;
                            invoice_products.invoice_id = null;
                            if (invoice_products.price == null)
                                invoice_products.price = 0;
                            if (invoice_products.quantity == null)
                                invoice_products.quantity = 0;
                            curr_amount_after_tax = (invoice_products.price ?? 0) * (invoice_products.quantity ?? 0);
                            total_amount = total_amount + curr_amount_after_tax;
                            Products product_info = await _context.products.Where(i => i.product_id == invoice_products.product_id).FirstOrDefaultAsync();
                            curr_tax_per = (product_info.tax_per ?? 0) / 100;
                            curr_tax_per += 1;
                            curr_amount_before_tax = curr_amount_after_tax / curr_tax_per;
                            curr_tax_amount = curr_amount_after_tax - curr_amount_before_tax;
                            curr_tax_amount = Math.Round(curr_tax_amount, 2);
                            tax_amount = tax_amount + curr_tax_amount;
                            if ((product_info.tax_per ?? 0) > tax_per)
                                tax_per = (product_info.tax_per ?? 0);
                        }
                        invoice.total_amount = total_amount;
                        invoice.tax_amount = tax_amount;
                        invoice.qr_data = await getqr_data(invoice);
                        if (invoice.invoice_type == "001" || invoice.invoice_type == "003")
                            invoice.paid_amount = total_amount;
                        else
                            invoice.paid_amount = 0;

                        int saveReturnValue = 0;
                        await _context.invoices.AddAsync(invoice);
                        saveReturnValue = await _context.SaveChangesAsync();
                     
                        if (saveReturnValue > 0)
                        {
                            //invoicesObject = await GetInvoiceObjectFromInvoice(await _context.invoices
                            //    .Where(i => i.id == invoice.id)
                            //    .Include(i => i.products)
                            //    .Include("invoicesproducts.products")
                            //    .Include(i => i.customer)
                            //    .Include("customer.invoices")
                            //    .Include("customer.receipts")
                            //    .Include(i => i.receipts)
                            //    .Include("receipts.receipts")
                            //    .Include(i => i.vou_setting)
                            //    .FirstOrDefaultAsync(), tax_per);

                            invoicesObject = await _context.invoices
                                .Where(i => i.id == invoice.id)
                                .Include(i => i.products)
                                .Include("products.product_data")
                                .Include("products.product_data.userswarehouse")
                                .Include(i => i.customer)
                                //.Include("customer.invoices")
                                //.Include("customer.receipts")
                                .Include(i => i.receipts)
                                .Include("receipts.receipts")
                                .Include(i => i.vou_setting)
                                .FirstOrDefaultAsync();
                            invoicesObject.invoice_date = current_invoice_date;
                            //GetInvoiceObjectFromInvoice(ref invoicesObject);
                            error_code = 200;
                            message_laguage = global.GetMessageLanguageFromCode(error_code);
                            status = true;
                        }
                        else
                        {
                            error_code = 302;
                            message_laguage = global.GetMessageLanguageFromCode(error_code);
                        }
                    }
                    else
                    {
                        message_laguage = customerCheck.message;
                        error_code = customerCheck.error_code;
                    }                    
                }
                else
                {
                    error_code = 303;
                    message_laguage = global.GetMessageLanguageFromCode(error_code);
                }
                
            }
            catch (Exception ex)
            {
                message_laguage = global.GetMessageLanguageFromCode(error_code, ex.Message);
            }
            return new InvoicesObjectResponse(invoicesObject, status, message_laguage, error_code);
        }
        public async Task<MessageResponse> GetLastSerialNo(int user_id, int user_online_type, string invoice_type, double invoice_no)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                UserStoreResponse userStore = await GetUserStore(user_id);
                if (userStore.status)
                {
                    SERIALS invoiceSerial = await _context.serials
                                        .Where(x => x.BRAN == userStore.data.bran && x.BTYPE == userStore.data.ftype && x.CODE == userStore.data.code && x.FTYPE == invoice_type)
                                        .FirstOrDefaultAsync();
                    if (invoiceSerial != null)
                    {
                        if (user_online_type == 1)
                            invoice_no = (invoiceSerial.SERIAL ?? 0) + 1;
                        if (invoice_no > (invoiceSerial.SERIAL ?? 0))
                                invoiceSerial.SERIAL = Convert.ToInt32(invoice_no);
                    }
                    else
                    {
                        if (user_online_type == 1)
                            invoice_no = 1;
                        invoiceSerial = new SERIALS()
                        {
                            BTYPE = userStore.data.ftype,
                            BRAN = userStore.data.bran,
                            CODE = userStore.data.code,
                            FTYPE = invoice_type,
                            FTYPE2 = 0,
                            SERIAL = Convert.ToInt32(invoice_no)
                        };
                        await _context.serials.AddAsync(invoiceSerial);
                    }
                    error_code = Convert.ToInt32(invoice_no);
                    status = true;
                }
                else
                {
                    error_code = 329;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);
        }
        private async Task<MessageResponse> CheckReturnInvoice(Invoices invoice)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                Invoices checkInvoice = await _context.invoices.Where(i => i.id == invoice.return_id).Include(i => i.products).FirstOrDefaultAsync();
                if (checkInvoice != null)
                {
                    if ((checkInvoice.invoice_type == "001" && invoice.invoice_type == "003") || (checkInvoice.invoice_type == "002" && invoice.invoice_type == "004"))
                    {

                        foreach (InvoicesProducts product in invoice.products)
                        {                            
                            double oldReturnQuantity = await _context.invoicesProducts
                                .Where(p => p.product_id == product.product_id && p.invoices.return_id == invoice.return_id && p.invoice_id != invoice.id && p.invoices.invoice_acceptance!=2)
                                .SumAsync(p => p.quantity ?? 0);
                            oldReturnQuantity += (product.quantity ?? 0);
                            if (checkInvoice.products.Any(p => p.product_id == product.product_id && (p.quantity ?? 0) >= (oldReturnQuantity)))
                            {
                                status = true;
                                error_code = checkInvoice.customer_id ?? 0;
                            }
                            else
                            {
                                status = false;
                                error_code = 311;
                                break;
                            }
                        }                      
                    }
                    else
                    {
                        error_code = 303;
                    }
                }
                else
                {
                    error_code = 306;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);

        }        

        public async Task<InvoicesObjectResponse> UpdateInvoice(int user_id, Invoices invoice)
        {
            bool status = false;
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, "error");
            //InvoicesObject invoicesObject = new InvoicesObject();
            Invoices invoicesObject = new Invoices();
            try
            {
                int saveReturnValue = 0;
                double total_amount = 0;
                double tax_amount = 0;
                double curr_tax_amount = 0;
                Invoices updateInvoice = await _context.invoices.FindAsync(invoice.id);
                if (updateInvoice != null || user_id == updateInvoice.user_id)
                {
                    if(updateInvoice.invoice_acceptance == 0)
                    {
                        if ((updateInvoice.invoice_type == "003") || (updateInvoice.invoice_type == "004"))
                        {
                            invoice.return_id = updateInvoice.return_id;
                            MessageResponse checkReturn = await CheckReturnInvoice(invoice);
                            if (!checkReturn.status)
                            {
                                status = checkReturn.status;
                                message_laguage = checkReturn.message;
                                error_code = checkReturn.error_code;
                                return new InvoicesObjectResponse(invoicesObject, status, message_laguage, error_code);
                            }
                        }
                        //if (invoice.customer != null)
                        //{
                        //    if(invoice.customer.customer_id != null)
                        //        updateInvoice.customer_id = invoice.customer.customer_id;
                        //}
                        else
                        {
                            if (invoice.customer_id != null && invoice.customer_id != 0)
                                updateInvoice.customer_id = invoice.customer_id;
                        }
                        //==if (invoice.invoice_date != null)
                        //==    updateInvoice.invoice_date = invoice.invoice_date;
                        //==if (invoice.invoice_no != null)
                        //==    updateInvoice.invoice_no = invoice.invoice_no;
                        //add and update exits products
                        double curr_amount_after_tax = 0;
                        double curr_tax_per = 0;
                        double curr_amount_before_tax = 0;
                        double tax_per = 0;
                        List<string> listOfProductIDs = new List<string>();
                        foreach (InvoicesProducts product in invoice.products)
                        {
                            Products product_info = await _context.products.Where(i => i.product_id == product.product_id).FirstOrDefaultAsync();
                            if (product_info != null)
                            {
                                listOfProductIDs.Add(product.product_id);
                                double price = 0;
                                double quantity = 0;
                                InvoicesProducts updateProcduct = await _context.invoicesProducts.FirstOrDefaultAsync(p => p.invoice_id == invoice.id && p.product_id == product.product_id);
                                if (updateProcduct == null)
                                {
                                    product.id = null;
                                    product.invoice_id = invoice.id;
                                    await _context.invoicesProducts.AddAsync(product);
                                    price = product.price ?? 0;
                                    quantity = product.quantity ?? 0;
                                }
                                else
                                {
                                    if (product.price != null)
                                        updateProcduct.price = product.price;
                                    if (product.quantity != null)
                                        updateProcduct.quantity = product.quantity;
                                    price = updateProcduct.price ?? 0;
                                    quantity = updateProcduct.quantity ?? 0;
                                }

                                curr_amount_after_tax = price * quantity;
                                total_amount = total_amount + curr_amount_after_tax;
                                curr_tax_per = (product_info.tax_per ?? 0) / 100;
                                curr_tax_per += 1;
                                curr_amount_before_tax = curr_amount_after_tax / curr_tax_per;
                                curr_tax_amount = curr_amount_after_tax - curr_amount_before_tax;
                                curr_tax_amount = Math.Round(curr_tax_amount, 2);
                                tax_amount = tax_amount + curr_tax_amount;
                                if ((product_info.tax_per ?? 0) > tax_per)
                                    tax_per = (product_info.tax_per ?? 0);
                            }
                        }
                        updateInvoice.total_amount = total_amount;
                        updateInvoice.tax_amount = tax_amount;
                        updateInvoice.qr_data = await getqr_data(updateInvoice);
                        if (updateInvoice.invoice_type == "001" || updateInvoice.invoice_type == "003")
                            updateInvoice.paid_amount = total_amount;
                        //delete not exist products
                        var deleteProducts = await _context.invoicesProducts.Where(p => !listOfProductIDs.Contains(p.product_id) && p.invoice_id == invoice.id).ToListAsync();
                        _context.invoicesProducts.RemoveRange(deleteProducts);

                        updateInvoice.invoice_date = global.CovertTimeZone(invoice.invoice_date ?? DateTime.UtcNow);
                        updateInvoice.customer_name = invoice.customer_name;
                        updateInvoice.customer_tax_id = invoice.customer_tax_id;
                        saveReturnValue = await _context.SaveChangesAsync();
                        if (saveReturnValue > 0)
                        {
                            //invoicesObject = await GetInvoiceObjectFromInvoice(await _context.invoices
                            //       .Where(i => i.id == invoice.id)
                            //       .Include(i => i.products)
                            //       .Include("invoicesproducts.products")
                            //       .Include(i => i.customer)
                            //       .Include("customer.invoices")
                            //       .Include("customer.receipts")
                            //       .Include(i => i.receipts)
                            //       .Include("receipts.receipts")
                            //       .Include(i => i.vou_setting)
                            //       .FirstOrDefaultAsync(), tax_per);

                            invoicesObject = await _context.invoices
                                   .Where(i => i.id == invoice.id)
                                   .Include(i => i.products)
                                   .Include("products.product_data")
                                   .Include("products.product_data.userswarehouse")
                                   .Include(i => i.customer)
                                   //.Include("customer.invoices")
                                   //.Include("customer.receipts")
                                   .Include(i => i.receipts)
                                   .Include("receipts.receipts")
                                   .Include(i => i.vou_setting)
                                   .FirstOrDefaultAsync();
                            //GetInvoiceObjectFromInvoice(ref invoicesObject);
                            error_code = 200; 
                            message_laguage = global.GetMessageLanguageFromCode(error_code);
                            status = true;
                        }
                        else
                        {
                            error_code = 302;
                            message_laguage = global.GetMessageLanguageFromCode(error_code);
                        }
                    }
                    else
                    {
                        if (updateInvoice.invoice_acceptance == 1)
                            error_code = 312;
                        else if (updateInvoice.invoice_acceptance == 2)
                            error_code = 305;
                        message_laguage = global.GetMessageLanguageFromCode(error_code);
                    }
                }
                else
                {
                    error_code = 306;
                    message_laguage = global.GetMessageLanguageFromCode(error_code);
                }
            }
            catch (Exception ex)
            {
                message_laguage = global.GetMessageLanguageFromCode(error_code, ex.Message);
            }
            return new InvoicesObjectResponse(invoicesObject, status, message_laguage, error_code);
        }
        public async Task<InvoicesObjectResponse> ChangeInoiveAcceptance(int invoice_id, int user_online_type, int invoice_acceptance, int user_id, double invoice_no)
        {
            bool status = false;
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, "error");
            Invoices invoicesObject = new Invoices();
            string message = "";
            try 
            {
                if (invoice_id > 0)
                {
                    Invoices invoice = await _context.invoices.FindAsync(invoice_id);
                    if (invoice != null)
                    { 
                        if (invoice.invoice_acceptance == 0)
                        {
                            invoice.invoice_acceptance = invoice_acceptance;
                            if (invoice_acceptance == 1)
                            {
                                if (invoice.invoice_type == "004")
                                {
                                    Invoices MainInvoice = await _context.invoices.Where(i => i.id == invoice.return_id).FirstOrDefaultAsync();
                                    if(MainInvoice != null)
                                    {
                                        MainInvoice.paid_amount = (MainInvoice.paid_amount + (invoice.total_amount ?? 0));
                                    }                                    
                                }
                                List<InvoicesProducts> listOfProducts = await _context.invoicesProducts
                                    .Where(p => p.invoice_id == invoice_id)
                                    .Include(p => p.product_data)
                                    .ToListAsync();
                                double quantity = 0;
                                UsersWarehouse usersWarehouse = new UsersWarehouse();
                                foreach (InvoicesProducts product in listOfProducts)
                                {
                                    quantity = product.quantity ?? 0;
                                    if (invoice.invoice_type == "001" || invoice.invoice_type == "002")
                                        quantity = quantity * -1;
                                    usersWarehouse = await _context.usersWarehouse
                                        .Where(w => w.user_id == invoice.user_id && w.product_id == product.product_id)
                                        .FirstOrDefaultAsync();
                                    if (usersWarehouse != null)
                                    {
                                        usersWarehouse.available_quantity += quantity;
                                    }
                                    else
                                    {
                                        usersWarehouse = new UsersWarehouse()
                                        {
                                            id = null,
                                            user_id = invoice.user_id,
                                            product_id = product.product_id,
                                            available_quantity = quantity
                                        };
                                        await _context.usersWarehouse.AddAsync(usersWarehouse);
                                    }
                                    if (((usersWarehouse.available_quantity ?? 0) < 0) && ((product.product_data.new_item ?? 0) == 0))
                                    {
                                        error_code = 311;
                                        message_laguage = global.GetMessageLanguageFromCode(error_code);
                                        return new InvoicesObjectResponse(invoicesObject, status, message_laguage, error_code);
                                    }
                                }
                                if ((invoice_no > 0) || (user_online_type == 1))
                                {
                                    MessageResponse getLastSerialNo = await GetLastSerialNo(user_id, user_online_type, invoice.invoice_type, invoice_no);
                                    if (getLastSerialNo.status)
                                    {
                                        invoice.invoice_no = getLastSerialNo.error_code;                                        
                                    }
                                    else
                                    {
                                        error_code = getLastSerialNo.error_code;
                                    }
                                }
                                else
                                {
                                    error_code = 329;
                                }
                            }
                            int saveReturnValue = await _context.SaveChangesAsync();
                            if (saveReturnValue > 0)
                            {
                                invoicesObject = await _context.invoices
                                  .Where(i => i.id == invoice.id)
                                  .Include(i => i.products)
                                  .Include("products.product_data")
                                  .Include("products.product_data.userswarehouse")
                                  .Include(i => i.customer)
                                  //.Include("customer.invoices")
                                  //.Include("customer.receipts")
                                  .Include(i => i.receipts)
                                  .Include("receipts.receipts")
                                  .Include(i => i.vou_setting)
                                  .FirstOrDefaultAsync();
                                error_code = 200;
                                status = true;
                            }
                            else
                            {
                                error_code = 302;
                            }
                        }
                        else
                        {
                            if (invoice.invoice_acceptance == 1)
                                error_code = 312;
                            else if (invoice.invoice_acceptance == 2)
                                error_code = 305;
                        }                        
                    }
                    else
                    {
                        error_code = 306;
                    }
                }
                else
                {
                    error_code = 313;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new InvoicesObjectResponse(invoicesObject, status, message_laguage, error_code);
        }

        private async Task<string> getqr_data(Invoices invoice)
        {
            string responseQR_DATA = null;
            string company_name = null;
            string company_tax_id = null;
            DateTime? invoice_datetime = null;
            double? invoice_amount = null;
            double? invoice_tax_amount = null;
            try
            {
                WSHOWResponse company_info = await GetCompanyInfo(invoice.user_id ?? 0);
                if (company_info.status)
                {
                    company_name = company_info.data.name;
                    company_tax_id = company_info.data.tax_id;
                    invoice_datetime = global.CovertTimeZone(invoice.invoice_date ?? DateTime.UtcNow);
                    invoice_amount = invoice.total_amount;
                    invoice_tax_amount = invoice.tax_amount;
                    if (company_name != null && company_tax_id != null && invoice_datetime != null && invoice_amount != null && invoice_tax_amount != null)
                    {
                        responseQR_DATA = GetBase64QR(company_name, company_tax_id, invoice_datetime, invoice_amount, invoice_tax_amount);
                    }
                }
            }
            catch 
            {
            }
            return responseQR_DATA;
        }

        private static string GetBase64QR(string company_name,string company_tax_id,DateTime? invoice_datetime, double? invoice_amount, double? invoice_tax_amount)
        {
            try
            {
                string hexString = GetHexFromData(company_name, company_tax_id, invoice_datetime ?? DateTime.Now, invoice_amount ?? 0, invoice_tax_amount ?? 0);
                var base64StringArr = ConvertFromStringToHex(hexString);
                string data = System.Convert.ToBase64String(base64StringArr);
                if (data.Length <= 500)
                    return data;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        private static string GetHexFromData(string company_name, string company_tax_id, DateTime invoice_datetime, double invoice_amount, double invoice_tax_amount)
        {
            string date_time = Convert.ToString(invoice_datetime);
            string amount = Convert.ToString(invoice_amount);
            string tax_amount = Convert.ToString(invoice_tax_amount);
            try
            {
                date_time = invoice_datetime.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
            }
            catch
            {
                try
                {
                    date_time = invoice_datetime.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
                }
                catch
                {
                }
            }

            string vHexText = "01" + GetHexFromLengthString(company_name) + GetHexFromString(company_name) +
                       "02" + GetHexFromLengthString(company_tax_id, true) + GetHexFromString(company_tax_id) +
                       "03" + GetHexFromLengthString(date_time) + GetHexFromString(date_time) +
                       "04" + GetHexFromLengthString(amount) + GetHexFromString(amount) +
                       "05" + GetHexFromLengthString(tax_amount) + GetHexFromString(tax_amount);
            return vHexText;
        }
        
        private static string GetHexFromString(string stringValue)
        {
            byte[] bystring = Encoding.UTF8.GetBytes(stringValue);
            return BitConverter.ToString(bystring).Replace("-", "").ToLower();
        }
        private static string GetHexFromLengthString(string stringValue, bool isCapital = false)
        {
            int lnstring = Encoding.UTF8.GetByteCount(stringValue);
            string vCase = "x2";
            if (isCapital)
                vCase = "X2";
            string returnhex = lnstring.ToString(vCase);
            return returnhex;
        }

        private static byte[] ConvertFromStringToHex(string inputHex)
        {
            inputHex = inputHex.Replace("-", "");

            byte[] resultantArray = new byte[inputHex.Length / 2];
            for (int i = 0; i < resultantArray.Length; i++)
            {
                resultantArray[i] = Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }

        public static implicit operator InvoiceProcedures(InvoicesProducts v)
        {
            throw new NotImplementedException();
        }
    }
}
