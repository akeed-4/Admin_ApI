using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace WaseetAPI.Application
{
    public class CustomerProcedures
    {
        private ApplicationDbContext _context;
        private GlobalProcedures global;
        private string _connectionStr;
        public CustomerProcedures(string connectionStr)
        {
            this._connectionStr = connectionStr;
            this._context = new ApplicationDbContext(connectionStr);
            this.global = new GlobalProcedures();
        } 
        public async Task<MessageResponse> CheckCustomerExits(int? user_id, int? customer_id)
        {
            bool status = false;
            int error_code = 400;
            string message = "error";
            try
            {
                Customers customer = await _context.customers.Where(x => ((x.user_id ?? 0) == user_id || (x.user_id ?? 0) == 0) && x.customer_id == customer_id).FirstOrDefaultAsync();
                if (customer != null)
                {
                    error_code = 200;
                    status = true;
                }
                else
                {
                    error_code = 301;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);

        }
        //public CustomersObject GetCustomersObjectFromCustomer(Customers customer)
        //{
        //    List<string> invoice_types = new List<string>() { "001", "002", "003", "004" };
        //    CustomersObject customersObject = new CustomersObject()
        //    {
        //        customer_id = customer.customer_id ?? 0,
        //        customer_name = new Languages()
        //        {
        //            Ar = customer.customer_name,
        //            En = customer.customer_ename
        //        },
        //        customer_image = customer.customer_image,
        //        customer_address = customer.customer_address,
        //        invoices_count = customer.invoices == null ? 0 : customer.invoices.Where(i => i.invoice_acceptance != 2 && invoice_types.Contains(i.invoice_type)).Count(),
        //        receipts_amount = customer.receipts == null ? 0 : customer.receipts.Sum(r => r.receipt_amount).Value
        //    };
        //    return customersObject;
        //}
        public async Task<CustomersResponse> Do(int user_id, string invoice_type)
        {
            List<Customers> customers = new List<Customers>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                UserStoreResponse userStore = await new InvoiceProcedures(_connectionStr).GetUserStore(user_id);
                if (userStore.status)
                {
                    var custList = await _context.customers.Where(x => ((invoice_type == "001" || invoice_type == "003") ? ((x.customer_type ?? 2) == 1 || (x.customer_type ?? 2) == 3) : ((x.customer_type ?? 2) == 2))  && (x.branch == userStore.data.bran))
                    .Include(i => i.invoices)
                    .Include(i => i.receipts)
                    .ToListAsync();
                    //foreach (var row in custList)
                    //{
                    //    //customers.Add(GetCustomersObjectFromCustomer(row));
                    //    customers.Add(row);
                    //}
                    customers = custList;
                    error_code = 200;
                    status = true;
                }
                else
                {
                    error_code = 328;
                }
                
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new CustomersResponse(customers, status, message_laguage, error_code);
        }
        public async Task<CustomersResponse> GetPayments(int user_id)
        {
            List<Customers> customers = new List<Customers>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                UserStoreResponse userStore = await new InvoiceProcedures(_connectionStr).GetUserStore(user_id);
                if (userStore.status)
                {
                    var custList = await _context.customers.Where(x => x.customer_type == 3 && (((x.user_id ?? 0) == user_id) || ((x.user_id ?? 0) == 0)) && (x.branch == userStore.data.bran))
                    .Include(i => i.invoices)
                    .Include(i => i.receipts)
                    .ToListAsync();
                    customers = custList;
                    error_code = 200;
                    status = true;







                }
                else
                {
                    error_code = 328;
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new CustomersResponse(customers, status, message_laguage, error_code);
        }
        
        public async Task<CustomersObjectResponse> CreateCustomer(int user_id, Customers customer)
        {
            bool status = false;
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, "error");            
            Customers customersObject = new Customers();
            try
            {
                UserStoreResponse userStore = await new InvoiceProcedures(_connectionStr).GetUserStore(user_id);
                if (userStore.status)
                {
                    if (customer != null)
                    {
                        int customer_id = await _context.customers.MaxAsync(c => c.customer_id) ?? 0;
                        customer_id += 1;
                        customer.customer_id = customer_id;
                        customer.branch = userStore.data.bran;
                        if (customer.user_id == 1)
                            customer.user_id = user_id;

                        if (customer.customer_name != null)
                        {
                            if (
                                string.IsNullOrEmpty(customer.customer_aname) &&
                                !string.IsNullOrEmpty(customer.customer_name.Ar)
                                )
                                customer.customer_aname = customer.customer_name.Ar;
                            if (
                                string.IsNullOrEmpty(customer.customer_ename) &&
                                !string.IsNullOrEmpty(customer.customer_name.En)
                                )
                                customer.customer_ename = customer.customer_name.En;
                        }
                        
                        int saveReturnValue = 0;
                        await _context.customers.AddAsync(customer);
                        saveReturnValue = await _context.SaveChangesAsync();
                        if (saveReturnValue > 0)
                        {
                            customersObject = await _context.customers
                                .Where(i => i.customer_id == customer.customer_id)
                                .Include(i => i.invoices)
                                .Include(i => i.receipts)
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
                        error_code = 302;
                        message_laguage = global.GetMessageLanguageFromCode(error_code);
                    }
                }
                else
                {
                    error_code = 328;
                }                
            }
            catch (Exception ex)
            {
                message_laguage = global.GetMessageLanguageFromCode(error_code, ex.Message);
            }
            return new CustomersObjectResponse(customersObject, status, message_laguage, error_code);
        }
        public async Task<CustomersObjectResponse> UpdateCustomer(int user_id, Customers customer)
        {
            bool status = false;
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, "error");
            Customers customersObject = new Customers();
            try
            {
                int saveReturnValue = 0;
                Customers updateCustomer = await _context.customers.FindAsync(customer.customer_id);
                //Find out if the customer has invoices 
                Invoices invoice = await _context.invoices.FindAsync(customer.customer_id);
                if (updateCustomer != null || (updateCustomer.user_id == user_id || updateCustomer.user_id == 0))
                {
                    //In case he has invoices we prevent the process of changing the name and keeping the previous name
                    if (invoice != null)
                    {
                        if ( invoice.total_amount != null || (updateCustomer.user_id == user_id || updateCustomer.user_id == 0))
                        {
                            updateCustomer.customer_aname = updateCustomer.customer_aname;
                            updateCustomer.customer_ename = updateCustomer.customer_ename;
                            updateCustomer.customer_image = customer.customer_image;
                            updateCustomer.customer_address = customer.customer_address;
                            updateCustomer.customer_taxid = customer.customer_taxid;
                            updateCustomer.customer_segal = customer.customer_segal;
                            updateCustomer.customer_country = customer.customer_country;
                            updateCustomer.customer_city = customer.customer_city;
                            updateCustomer.customer_state = customer.customer_state;
                            updateCustomer.customer_postcode = customer.customer_postcode;
                            updateCustomer.customer_building = customer.customer_building;
                            updateCustomer.customer_street = customer.customer_street;
                            updateCustomer.customer_mobile = customer.customer_mobile;
                            if (customer.user_id == 1)
                                updateCustomer.user_id = user_id;
                            else
                                updateCustomer.user_id = 0;
                        }
                    }
                    else
                    {
                        updateCustomer.customer_aname = customer.customer_aname;
                        updateCustomer.customer_ename = customer.customer_ename;
                        updateCustomer.customer_image = customer.customer_image;
                        updateCustomer.customer_address = customer.customer_address;
                        updateCustomer.customer_taxid = customer.customer_taxid;
                        updateCustomer.customer_segal = customer.customer_segal;
                        updateCustomer.customer_country = customer.customer_country;
                        updateCustomer.customer_city = customer.customer_city;
                        updateCustomer.customer_state = customer.customer_state;
                        updateCustomer.customer_postcode = customer.customer_postcode;
                        updateCustomer.customer_building = customer.customer_building;
                        updateCustomer.customer_street = customer.customer_street;
                        updateCustomer.customer_mobile = customer.customer_mobile;
                        if (customer.user_id == 1)
                            updateCustomer.user_id = user_id;
                        else
                            updateCustomer.user_id = 0;
                    }
                  
       
                    saveReturnValue = await _context.SaveChangesAsync();
                    if (saveReturnValue > 0)
                    {
                        customersObject = await _context.customers
                            .Where(i => i.customer_id == customer.customer_id)
                            .Include(i => i.invoices)
                            .Include(i => i.receipts)
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
                    error_code = 306;
                    message_laguage = global.GetMessageLanguageFromCode(error_code);
                }
            }
            catch (Exception ex)
            {
                message_laguage = global.GetMessageLanguageFromCode(error_code, ex.Message);
            }
            return new CustomersObjectResponse(customersObject, status, message_laguage, error_code);
        }
        public async Task<MessageResponse> DeleteCustomer(int user_id, int customer_id)
        {
            bool status = false;
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, "error");
            try
            {
                int saveReturnValue = 0;
                Customers DeleteCustomer = await _context.customers.FindAsync(customer_id);
                if (DeleteCustomer != null)
                {

                    var checkToDeleteCustomer = await _context.invoices.AnyAsync(i => i.customer_id == DeleteCustomer.customer_id)
                        || await _context.receipts.AnyAsync(r => r.customer_id == DeleteCustomer.customer_id);
                    if (!checkToDeleteCustomer)
                    {
                        _context.customers.Remove(DeleteCustomer);
                    }                    

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
                    error_code = 306;
                    message_laguage = global.GetMessageLanguageFromCode(error_code);
                }
            }
            catch (Exception ex)
            {
                message_laguage = global.GetMessageLanguageFromCode(error_code, ex.Message);
            }
            return new MessageResponse(status, message_laguage, error_code);
        }
    }
}
