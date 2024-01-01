using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;

namespace WaseetAPI.Application
{
    public class ReceiptsProcedures
    {
        private ApplicationDbContext _context;
        private string _connectionStr;
        private GlobalProcedures global;
        public ReceiptsProcedures(string connectionStr)
        {
            this._context = new ApplicationDbContext(connectionStr);
            this._connectionStr = connectionStr;
            this.global = new GlobalProcedures();
        }

        //private async Task<Receipts> GetReceiptObjectFromReceipt(Receipts receipt)
        //{
        //    List<InvoicesReceiptsObject> listOfInvoices = new List<InvoicesReceiptsObject>();
        //    //ICustomers customersObject = new Customers();           
        //    //List of Receipts Object
        //    if (receipt.receiptsinvoices != null)
        //    {
        //        foreach (var invoice in receipt.receiptsinvoices)
        //        {
        //            InvoicesReceiptsObject invoicesReceipts = new InvoicesReceiptsObject()
        //            {
        //                invoice = await GetInvoiceNoReceiptsObjectFromInvoice(invoice.invoices),
        //                paid_amount = invoice.paid_amount
        //            };
        //            listOfInvoices.Add(invoicesReceipts);
        //        }
        //    }
        //    //Customer Object
        //    //if (receipt.customer != null)
        //    //{
        //    //    //customersObject = new CustomerProcedures(_connectionStr).GetCustomersObjectFromCustomer(receipt.customer);
        //    //    customersObject = receipt.customer;
        //    //}
        //    ReceiptsObject receiptsObject = new ReceiptsObject()
        //    {
        //        id = receipt.id,
        //        receipt_no = receipt.receipt_no,
        //        receipt_date = global.CovertTimeZone(receipt.receipt_date ?? DateTime.UtcNow),
        //        receipt_amount = receipt.receipt_amount,
        //        receipt_acceptance = receipt.receipt_acceptance,
        //        customer = receipt.customer,
        //        invoices = listOfInvoices
        //    };
        //    return receiptsObject;
        //}
        //private async Task<InvoicesNoReceiptsObject> GetInvoiceNoReceiptsObjectFromInvoice(Invoices invoice)
        //{
        //    List<InvoicesProducts> listOfProducts = new List<InvoicesProducts>();
        //    //ICustomers customersObject = new Customers();
        //    //List of Products Object
        //    if (invoice.products != null)
        //    {
        //        foreach (var p in invoice.products)
        //        {
        //            if (p.product_data != null)
        //                if (p.product_data.userswarehouse != null)
        //                {
        //                    p.product_data.price = p.product_data.userswarehouse.Where(w => w.user_id == invoice.user_id).FirstOrDefault().current_price;
        //                    p.product_data.available_quantity = p.product_data.userswarehouse.Where(w => w.user_id == invoice.user_id).FirstOrDefault().available_quantity;
        //                }
        //        }
        //        listOfProducts = invoice.products.Cast<InvoicesProducts>().ToList();
        //        //foreach (var invoicesproducts in invoice.invoicesproducts)
        //        //{
        //        //    //Languages product_name = new Languages()
        //        //    //{
        //        //    //    Ar = product.product_data.product_name,
        //        //    //    En = product.product_data.product_ename
        //        //    //};
        //        //    //UsersWarehouse usersWarehouse = await new InvoiceProcedures(_connectionStr).GetAvailableQuantity(invoice.user_id ?? 0, product.product_data.product_id);
        //        //    //double available_quantity = 0;
        //        //    //double current_price = 0;
        //        //    //if (usersWarehouse != null)
        //        //    //{
        //        //    //    available_quantity = (usersWarehouse.available_quantity ?? 0);
        //        //    //    current_price = (usersWarehouse.current_price ?? 0);
        //        //    //}
        //        //    //else
        //        //    //{
        //        //    //    current_price = (product.product_data.price ?? 0);
        //        //    //}
        //        //    //IProducts product_data = new Products()
        //        //    //{
        //        //    //    product_id = product.product_data.product_id,
        //        //    //    product_image = product.product_data.product_image,
        //        //    //    product_description = product.product_data.product_description,
        //        //    //    available_quantity = available_quantity,
        //        //    //    price = current_price,
        //        //    //    product_name = product_name
        //        //    //};
        //        //    //IInvoicesProducts invoicesProducts = new InvoicesProducts()
        //        //    //{
        //        //    //    product_data = product_data,
        //        //    //    price = product.price,
        //        //    //    quantity = product.quantity
        //        //    //};
        //        //    IInvoicesProducts iinvoicesproducts = invoicesproducts;
        //        //    iinvoicesproducts.product_data.available_quantity = invoicesproducts.products.get_available_quantity(invoice.user_id??0);
        //        //    iinvoicesproducts.product_data.price = invoicesproducts.products.get_price(invoice.user_id ?? 0);
        //        //    listOfProducts.Add(iinvoicesproducts);
        //        //}
        //    }           
        //    string invoice_type = invoice.invoice_type;
        //    if (invoice_type != "001" && invoice_type != "002" && invoice_type != "003" && invoice_type != "004")
        //        invoice_type = null;
        //    //Customer Object
        //    //if (invoice.customer != null)
        //    //{
        //    //    //customersObject = new CustomerProcedures(_connectionStr).GetCustomersObjectFromCustomer(invoice.customer);
        //    //    customersObject = invoice.customer;
        //    //}
        //    VOU_SETTINGResponse vou_SettingObjectResponse = await GetInvoiceNameFromType(invoice_type);
        //    InvoicesNoReceiptsObject invoiceNoReceiptsObject = new InvoicesNoReceiptsObject()
        //    {
        //        id = invoice.id,
        //        invoice_no = invoice.invoice_no,
        //        invoice_date = global.CovertTimeZone(invoice.invoice_date ?? DateTime.UtcNow),
        //        invoice_type = invoice_type,
        //        invoice_name = vou_SettingObjectResponse.data.NAME,
        //        invoice_short_name = vou_SettingObjectResponse.data.SHORT_NAME,
        //        products = listOfProducts,
        //        total_amount = invoice.total_amount,
        //        paid_amount = invoice.paid_amount,
        //        invoice_acceptance = invoice.invoice_acceptance,
        //        tax_amount = invoice.tax_amount,
        //        qr_data = invoice.qr_data,
        //        customer = invoice.customer
        //    };
        //    return invoiceNoReceiptsObject;
        //}
        private async Task<VOU_SETTINGResponse> GetInvoiceNameFromType(string invoice_type)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            VOU_SETTING vou_SettingObject = new VOU_SETTING();
            //Languages invoice_name = new Languages();
            //Languages short_invoice_name = new Languages();
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
                //}
                //else
                {
                    vou_SettingObject = new VOU_SETTING() 
                    { 
                        FTYPE = invoice_type,
                        NAME1 = "",
                        NAME2 = "",
                        SHORT_NAME1 = "",
                        SHORT_NAME2 = ""
                    };
                    //invoice_name.Ar = "";
                    //invoice_name.En = "";
                    //short_invoice_name.Ar = "";
                    //short_invoice_name.En = "";
                }
                //vou_SettingObject = new VOU_SETTINGObject()
                //{
                //    FTYPE = invoice_type,
                //    NAME = invoice_name,
                //    SHORT_NAME = short_invoice_name
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
        //private async Task<List<ReceiptsObject>> GetReceiptsObjectFromReceipts(List<Receipts> listOfReceipts)
        //{
        //    List<ReceiptsObject> listOfReceiptsObject = new List<ReceiptsObject>();
        //    foreach (var receipt in listOfReceipts)
        //    {
        //        ReceiptsObject receiptsObject = await GetReceiptObjectFromReceipt(receipt);
        //        listOfReceiptsObject.Add(receiptsObject);
        //    }
        //    return listOfReceiptsObject;
        //}

        public async Task<ReceiptsResponse> GetReceipts(int user_id)
        {
            List<Receipts> receipts = new List<Receipts>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                receipts = await _context.receipts
                    .Where(x => x.user_id == user_id && x.receipt_status == true && (x.receipt_acceptance ?? 0) != 2)
                    .Include(i => i.invoices)
                    .Include("invoices.invoices")
                    .Include("invoices.invoices.customer")
                    .Include("invoices.invoices.products")
                    .Include("invoices.invoices.products.product_data")
                    .Include("invoices.invoices.products.product_data.userswarehouse")
                    .Include(i => i.customer)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .OrderByDescending(i => i.receipt_date)
                    .ToListAsync();
                //receipts = await GetReceiptsObjectFromReceipts (receiptsList);
                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ReceiptsResponse(receipts, status, message_laguage, error_code);
        }


        public async Task<ReceiptsResponse> webGetReceipts(int user_id)
        {
            List<Receipts> receipts = new List<Receipts>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                receipts = await _context.receipts
                    .Where(x => x.receipt_status == true )
                    .Include(i => i.invoices)
                    //.Include("invoices.invoices")
                    .Include("invoices.invoices.customer")
                    //.Include("invoices.invoices.products")
                    //.Include("invoices.invoices.products.product_data")
                    //.Include("invoices.invoices.products.product_data.userswarehouse")
                    .Include(i => i.customer)
                      .Include(i => i.localUser)
                    ////.Include("customer.invoices")
                    ////.Include("customer.receipts")
                    .OrderByDescending(i => i.receipt_date)
                    .ToListAsync();
                //receipts = await GetReceiptsObjectFromReceipts (receiptsList);
                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ReceiptsResponse(receipts, status, message_laguage, error_code);
        }
        public async Task<ReceiptResponse> CreateReceipts(int user_id, Receipts receipt)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            Receipts receiptsObject = new Receipts();
            try
            {
                double total_amount = 0;
                receipt.id = null;
                receipt.user_id = user_id;
                receipt.receipt_tobaseet = 0;
                receipt.receipt_date = global.CovertTimeZone(receipt.receipt_date ?? DateTime.UtcNow);
                receipt.receipt_no = 0;
                //list of invoices to delete
                List<int> listOfInvoiceIDs = new List<int>();
                List<int> listOfCustomerIDs = new List<int>();
                //loop of invoices of receipts
                foreach (ReceiptsInvoices receiptInvoice in receipt.invoices)
                {
                    //check if invoice exists
                    Invoices invoice = await _context.invoices.FindAsync(receiptInvoice.invoice_id);
                    if (invoice != null)
                    {
                        //add customer to list
                        if (!listOfCustomerIDs.Contains(invoice.customer_id ?? 0))
                            listOfCustomerIDs.Add(invoice.customer_id ?? 0);
                        //get remaining amount for invoice
                        double toPay = ((invoice.total_amount ?? 0) - (invoice.paid_amount ?? 0));
                        //if is credit invoice and remaining amount > 0
                        if (invoice.invoice_type == "002" && toPay > 0)
                        {
                            if (toPay < receiptInvoice.paid_amount)
                                receiptInvoice.paid_amount = toPay;
                            receiptInvoice.id = null;
                            receiptInvoice.receipt_id = null;
                            //sum total amount
                            total_amount = total_amount + (receiptInvoice.paid_amount ?? 0);
                        }
                        else
                            listOfInvoiceIDs.Add(receiptInvoice.invoice_id ?? 0);
                    }
                    else
                        listOfInvoiceIDs.Add(receiptInvoice.invoice_id ?? 0);
                }
                if (listOfCustomerIDs.Count == 1)
                {
                    receipt.customer_id = listOfCustomerIDs[0];
                    MessageResponse customerCheck = await new CustomerProcedures(_connectionStr).CheckCustomerExits(user_id, receipt.customer_id);
                    if (customerCheck.status)
                    {
                        //remove not valid invoices
                        foreach (int invoice_id in listOfInvoiceIDs)
                        {
                            ReceiptsInvoices receiptInvoice = receipt.invoices.Where(i => i.invoice_id == invoice_id).FirstOrDefault();
                            receipt.invoices.Remove(receiptInvoice);
                        }
                        receipt.receipt_amount = total_amount;
                        receipt.receipt_status = true;
                        int saveReturnValue = 0;
                        if (total_amount > 0)
                        {
                            await _context.receipts.AddAsync(receipt);
                            saveReturnValue = await _context.SaveChangesAsync();
                            if (saveReturnValue > 0)
                            {
                                //receiptsObject = await GetReceiptObjectFromReceipt(await _context.receipts
                                //    .Where(i => i.id == receipt.id)
                                //    .Include(i => i.invoices)
                                //    .Include("invoices.invoices")
                                //    .Include("invoices.invoices.customer")
                                //    .Include("invoices.invoices.products")
                                //    .Include("invoices.invoices.products.product_data")
                                //    .Include("invoices.invoices.products.product_data.userswarehouse")
                                //    .Include(i => i.customer)
                                //    //.Include("customers.invoices")
                                //    //.Include("customers.receipts")
                                //    .FirstOrDefaultAsync());
                                receiptsObject = await _context.receipts
                                    .Where(i => i.id == receipt.id)
                                    .Include(i => i.invoices)
                                    .Include("invoices.invoices")
                                    .Include("invoices.invoices.customer")
                                    .Include("invoices.invoices.products")
                                    .Include("invoices.invoices.products.product_data")
                                    .Include("invoices.invoices.products.product_data.userswarehouse")
                                    .Include(i => i.customer)
                                    //.Include("customers.invoices")
                                    //.Include("customers.receipts")
                                    .FirstOrDefaultAsync();
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
                            error_code = 320;
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
                    error_code = 321;
                    message_laguage = global.GetMessageLanguageFromCode(error_code);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            }            
            return new ReceiptResponse(receiptsObject, status, message_laguage, error_code);
        }
        private async Task<MessageResponse> GetLastSerialNo(int user_id, int user_online_type, string invoice_type, double invoice_no)
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
        public async Task<MessageResponse> UpdateReceipts(int user_id, Receipts receipt)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            try
            {
                int saveReturnValue = 0;
                double total_amount = 0;
                Receipts updateReceipt = await _context.receipts.FindAsync(receipt.id);
                if (updateReceipt != null || updateReceipt.user_id == user_id)
                {
                    if (receipt.customer != null)
                    {
                        receipt.customer_id = receipt.customer.customer_id;
                    }
                    MessageResponse customerCheck = new MessageResponse(true, global.GetMessageLanguageFromCode(200), 200);
                    if (receipt.customer_id != null)
                        customerCheck = await new CustomerProcedures(_connectionStr).CheckCustomerExits(receipt.user_id, receipt.customer_id);
                    if (customerCheck.status)
                    {
                        //==updateReceipt.receipt_date = receipt.receipt_date;
                        //==updateReceipt.receipt_no = receipt.receipt_no;

                        //add and update exits products
                        List<int> listOfInvoiceIDs = new List<int>();

                        //loop of invoices of receipts
                        foreach (ReceiptsInvoices receiptInvoice in receipt.invoices)
                        {                           
                            //check if invoice exists
                            Invoices invoice = await _context.invoices.FindAsync(receiptInvoice.invoice_id);
                            if (invoice != null)
                            {
                                ReceiptsInvoices updateReceiptInvoice = await _context.receiptsInvoices.FirstOrDefaultAsync(p => p.receipt_id == receipt.id && p.invoice_id == receiptInvoice.invoice_id);

                                //get remaining amount for invoice
                                double toPay = ((invoice.total_amount ?? 0) - (invoice.paid_amount ?? 0));
                                if (updateReceiptInvoice == null)
                                {
                                    //if is credit invoice and remaining amount > 0
                                    if (invoice.invoice_type == "002" && toPay > 0)
                                    {
                                        if (toPay < receiptInvoice.paid_amount)
                                            receiptInvoice.paid_amount = toPay;

                                        receiptInvoice.id = null;
                                        receiptInvoice.receipt_id = receipt.id;
                                        await _context.receiptsInvoices.AddAsync(receiptInvoice);
                                        //update payed amount in invoice
                                        invoice.paid_amount = (invoice.paid_amount + (receiptInvoice.paid_amount ?? 0));

                                        //sum total amount
                                        total_amount = total_amount + (receiptInvoice.paid_amount ?? 0);
                                        listOfInvoiceIDs.Add(receiptInvoice.invoice_id ?? 0);
                                    }
                                }
                                else
                                {
                                    toPay = toPay + (updateReceiptInvoice.paid_amount ?? 0);
                                    if (invoice.invoice_type == "002" && toPay > 0)
                                    {
                                        if (toPay < receiptInvoice.paid_amount)
                                            receiptInvoice.paid_amount = toPay;
                                        //update payed amount in invoice
                                        invoice.paid_amount = (invoice.paid_amount + (receiptInvoice.paid_amount ?? 0) - (updateReceiptInvoice.paid_amount ?? 0));
                                        updateReceiptInvoice.paid_amount = receiptInvoice.paid_amount;
                                        //other information here
                                        //--
                                        //sum total amount
                                        total_amount = total_amount + (receiptInvoice.paid_amount ?? 0);
                                        listOfInvoiceIDs.Add(receiptInvoice.invoice_id ?? 0);
                                    }
                                }
                            }
                        }
                        if (total_amount > 0)
                        {
                            updateReceipt.receipt_amount = total_amount;
                            //delete not exist products
                            var deleteReceiptInvoice = await _context.receiptsInvoices.Where(p => !listOfInvoiceIDs.Contains(p.invoice_id ?? 0) && p.receipt_id == receipt.id).ToListAsync();
                            foreach (var deleteInvoice in deleteReceiptInvoice)
                            {
                                Invoices invoice = await _context.invoices.FindAsync(deleteInvoice.invoice_id);
                                invoice.paid_amount = invoice.paid_amount - deleteInvoice.paid_amount;
                            }
                            _context.receiptsInvoices.RemoveRange(deleteReceiptInvoice);

                            updateReceipt.receipt_date = global.CovertTimeZone(updateReceipt.receipt_date ?? DateTime.UtcNow);
                            saveReturnValue = await _context.SaveChangesAsync();
                            if (saveReturnValue > 0)
                            {
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
                            error_code = 322;
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
                    error_code = 325;
                    message_laguage = global.GetMessageLanguageFromCode(error_code);
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            }
            return new MessageResponse(status, message_laguage, error_code);
        }

        public async Task<MessageResponse> ChangeReceiptAcceptance(int receipt_id, int receipt_acceptance, int user_id, int user_online_type, double receipt_no)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                if (receipt_id > 0)
                {
                    Receipts receipt = await _context.receipts.Where(r => r.id == receipt_id).Include(i => i.invoices).FirstOrDefaultAsync();
                    if (receipt != null)
                    {
                        if ((receipt.receipt_acceptance ?? 0) == 0)
                        {
                            receipt.receipt_acceptance = receipt_acceptance;
                            bool vCheckInvoicePay = true;
                            if (receipt_acceptance == 1)
                            {
                                //loop of invoices of receipts
                                foreach (ReceiptsInvoices receiptInvoice in receipt.invoices)
                                {
                                    //check if invoice exists
                                    Invoices invoice = await _context.invoices.FindAsync(receiptInvoice.invoice_id);
                                    if (invoice != null)
                                    {
                                        //get remaining amount for invoice
                                        double toPay = ((invoice.total_amount ?? 0) - (invoice.paid_amount ?? 0));
                                        //if is credit invoice and remaining amount > 0
                                        if (invoice.invoice_type == "002" && toPay > 0)
                                        {
                                            if (toPay < receiptInvoice.paid_amount)
                                            {
                                                vCheckInvoicePay = false;
                                                error_code = 320;
                                            }
                                            else
                                                invoice.paid_amount = (invoice.paid_amount + (receiptInvoice.paid_amount ?? 0));
                                        }
                                        else
                                        {
                                            vCheckInvoicePay = false;
                                            error_code = 320;
                                        }
                                    }
                                    else
                                    {
                                        vCheckInvoicePay = false;
                                        error_code = 320;
                                    }
                                }
                                if ((receipt_no > 0) || (user_online_type == 1))
                                {
                                    MessageResponse getLastSerialNo = await GetLastSerialNo(user_id, user_online_type, "055", receipt_no);
                                    if (getLastSerialNo.status)
                                    {
                                        receipt.receipt_no = getLastSerialNo.error_code;                                        
                                    }
                                    else
                                    {
                                        vCheckInvoicePay = false;
                                        error_code = getLastSerialNo.error_code;
                                    }
                                }
                                else
                                {
                                    vCheckInvoicePay = false;
                                    error_code = 329;
                                }
                            }
                            if (vCheckInvoicePay)
                            {
                                int saveReturnValue = await _context.SaveChangesAsync();
                                if (saveReturnValue > 0)
                                {
                                    error_code = 200;
                                    status = true;
                                }
                                else
                                {
                                    error_code = 302;
                                }
                            }
                        }
                        else
                        {
                            if ((receipt.receipt_acceptance ?? 0) == 1)
                                error_code = 323;
                            else if ((receipt.receipt_acceptance ?? 0) == 2)
                                error_code = 324;
                        }
                    }
                    else
                    {
                        error_code = 325;
                    }
                }
                else
                {
                    error_code = 326;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);
        }
    }
}
