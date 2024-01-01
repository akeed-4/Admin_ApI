using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using WaseetAPI.Domain.Models.Salla;
using System.ComponentModel.Design;

namespace WaseetAPI.Application
{
    public class ProductProcedures
    {
        private ApplicationDbContext _context;
        private string _connectionStr;
        private GlobalProcedures global;
        public ProductProcedures(string connectionStr)
        {
            this._context = new ApplicationDbContext(connectionStr);
            this._connectionStr = connectionStr;
            this.global = new GlobalProcedures();
        }
        public async Task<ReturnProductsObjectListResponse> GetReceiveProducts(int user_id, string invoice_type)
        {
            List<ReturnProductsObject> listOfReturnProducts = new List<ReturnProductsObject>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                var invoicesList = await _context.invoices
                    .Where(x => x.user_id == user_id && x.invoice_type == invoice_type && x.invoice_status == true && x.invoice_acceptance == 2)
                    .Include(i => i.products)
                    .Include("products.product_data")
                    .Include("products.product_data.userswarehouse")
                    .ToListAsync();
                listOfReturnProducts = GetReturnProductsListFromInvoice(invoicesList);
                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ReturnProductsObjectListResponse(listOfReturnProducts, status, message_laguage, error_code);
        }
        private List<ReturnProductsObject> GetReturnProductsListFromInvoice(List<Invoices> listOfInvoices)
        {
            List<ReturnProductsObject> listOfReturnProductsObject = new List<ReturnProductsObject>();
            foreach (var invoice in listOfInvoices)
            {
                ReturnProductsObject returnProductsObject = GetReturnProductsFromInvoice(invoice);
                listOfReturnProductsObject.Add(returnProductsObject);
            }
            return listOfReturnProductsObject;
        }
        private ReturnProductsObject GetReturnProductsFromInvoice(Invoices invoice)
        {
            ReturnProductsObject returnProductsObject = new ReturnProductsObject()
            {
                id = invoice.id,
                invoice_no = invoice.invoice_no,
                invoice_date = global.CovertTimeZone(invoice.invoice_date ?? DateTime.UtcNow),
                products = invoice.products
            };
            return returnProductsObject;
        }
        public async Task<HistoryProductsResponse> GetHistoryProducts(int user_id, string product_id)
        {
            List<HistoryProductsObject> historyProducts = new List<HistoryProductsObject>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                var invoicesList = await _context.invoices
                    .Where(x => x.user_id == user_id && x.invoice_acceptance == 1 && (x.invoice_tobaseet != 1) && x.invoice_status == true)
                    .Include(i => i.products.Where(p => product_id == null ? true : p.product_id == product_id))
                    .Include("products.product_data")
                    .Include("products.product_data.userswarehouse")
                    .Include(i => i.customer)
                    //.Include("customer.invoices")
                    //.Include("customer.receipts")
                    .Include(i => i.vou_setting)
                    .ToListAsync();
                //List<VOU_SETTING> invoiceTypesList = await _context.vou_setting
                //    .Where(x => x.FTYPE2 == 0)
                //    .ToListAsync();
                historyProducts = GetHistoryProductsObjectListFromInvoices(invoicesList);
                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new HistoryProductsResponse(historyProducts, status, message_laguage, error_code);
        }
        private List<HistoryProductsObject> GetHistoryProductsObjectListFromInvoices(List<Invoices> listOfInvoices, double current_tax_per = 0)
        {
            List<HistoryProductsObject> listOfProductsObject = new List<HistoryProductsObject>();
            foreach (var invoice in listOfInvoices)
            {
                //Simple Invoice Data
                //InvoicesObject invoicesObject = await GetInvoiceObjectFromInvoice(invoice, current_tax_per);
                //List of Products Object
                if (invoice.products != null)
                {
                    foreach (var product in invoice.products)
                    {
                        //ProductsObject product_data = await GetProductObjectFromProduect(product.product_data, invoice.user_id ?? 0);           
                        double quantity = product.quantity ?? 0;
                        if (invoice.invoice_type == "051" || invoice.invoice_type == "001" || invoice.invoice_type == "002")
                            quantity = quantity * -1;
                        HistoryProductsObject productObject = new HistoryProductsObject()
                        {
                            //product_data = product_data,
                            product_data = product.product_data,
                            invoice = invoice,
                            quantity = quantity
                        };
                        //productObject.product_data.available_quantity = product.products.get_available_quantity(invoice.user_id??0);
                        //productObject.product_data.price = product.products.get_price(invoice.user_id??0);
                        listOfProductsObject.Add(productObject);
                    }
                }
            }
            return listOfProductsObject;
        }
        public async Task<ReturnProductsObjectResponse> CreateProductsReturn(int user_id, List<InvoicesProducts> listOfProducts)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            ReturnProductsObject returnProductsObject = new ReturnProductsObject();
            try
            {
                double invoice_no = 1;

                MessageResponse getLastSerialNo = await new InvoiceProcedures(_connectionStr).GetLastSerialNo(user_id, 1, "051", invoice_no);
                if (getLastSerialNo.status)
                {
                    invoice_no = getLastSerialNo.error_code;
                    foreach (InvoicesProducts invoice_products in listOfProducts)
                    {
                        invoice_products.id = null;
                        invoice_products.invoice_id = null;
                        invoice_products.price = 0;
                        if (invoice_products.quantity == null)
                            invoice_products.quantity = 0;

                        double quantity = invoice_products.quantity ?? 0;
                        quantity = quantity * -1;
                        UsersWarehouse usersWarehouse = await _context.usersWarehouse
                            .Where(w => w.user_id == user_id && w.product_id == invoice_products.product_id)
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
                                user_id = user_id,
                                product_id = invoice_products.product_id,
                                available_quantity = quantity
                            };
                            await _context.usersWarehouse.AddAsync(usersWarehouse);
                        }
                    }
                    Invoices returnInvoice = new Invoices()
                    {
                        id = null,
                        invoice_no = invoice_no,
                        invoice_date = global.CovertTimeZone(DateTime.UtcNow),
                        user_id = user_id,
                        invoice_type = "051",
                        invoice_type2 = 0,
                        invoice_tobaseet = 0,
                        invoice_status = true,
                        invoice_acceptance = 1,
                        customer_id = null,
                        customer = null,
                        receipts = null,
                        total_amount = 0,
                        paid_amount = 0,
                        products = listOfProducts
                    };
                    int saveReturnValue = 0;
                    await _context.invoices.AddAsync(returnInvoice);
                    saveReturnValue = await _context.SaveChangesAsync();
                    if (saveReturnValue > 0)
                    {
                        returnProductsObject = GetReturnProductsFromInvoice(await _context.invoices
                            .Where(i => i.id == returnInvoice.id)
                            .Include(i => i.products)
                            .Include("products.product_data")
                            .Include("products.product_data.userswarehouse")
                            .FirstOrDefaultAsync());
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
                    error_code = 329;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ReturnProductsObjectResponse(returnProductsObject, status, message_laguage, error_code);
        }
        public async Task<MessageResponse> AcceptReceiveProducts(int invoice_id, int invoice_acceptance)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                if (invoice_id > 0)
                {
                    Invoices invoice = await _context.invoices.FindAsync(invoice_id);
                    if (invoice != null)
                    {
                        if (invoice.invoice_acceptance == 2)
                        {
                            invoice.invoice_acceptance = invoice_acceptance;

                            if (invoice_acceptance == 1)
                            {
                                List<InvoicesProducts> listOfProducts = await _context.invoicesProducts
                                    .Where(p => p.invoice_id == invoice_id)
                                    .ToListAsync();
                                foreach (InvoicesProducts product in listOfProducts)
                                {
                                    UsersWarehouse usersWarehouse = await _context.usersWarehouse
                                        .Where(w => w.user_id == invoice.user_id && w.product_id == product.product_id)
                                        .FirstOrDefaultAsync();
                                    if (usersWarehouse != null)
                                    {
                                        usersWarehouse.available_quantity += (product.quantity ?? 0);
                                        usersWarehouse.current_price = (product.price ?? 0);
                                    }
                                    else
                                    {
                                        usersWarehouse = new UsersWarehouse()
                                        {
                                            id = null,
                                            user_id = invoice.user_id,
                                            product_id = product.product_id,
                                            available_quantity = (product.quantity ?? 0),
                                            current_price = (product.price ?? 0)
                                        };
                                        await _context.usersWarehouse.AddAsync(usersWarehouse);
                                    }
                                }
                            }
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
                        else
                        {
                            if (invoice.invoice_acceptance == 0)
                                error_code = 307;
                            else if (invoice.invoice_acceptance == 1)
                                error_code = 308;
                        }
                    }
                    else
                    {
                        error_code = 309;
                    }
                }
                else
                {
                    error_code = 310;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);
        }
        public async Task<ProductsResponse> GetWarehousProducts(int user_id, int is_own_database, string product_id = null, string all_products = null)
        {
            List<Products> productsObject = new List<Products>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                var store_quan = await get_items_stock(user_id, is_own_database, product_id);
                if (all_products != null && all_products != "")
                {
                    productsObject = new List<Products>
                        (from product in await _context.products
                         .Include(p => p.userswarehouse
                         .Where(x =>  product_id == null ? true : x.product_id == product_id))
                         .ToListAsync()
                         join store in store_quan on product.product_id equals store.icode into final_product
                         from f_product in final_product.DefaultIfEmpty()
                         select new Products()
                         {
                             product_id = product.product_id,
                             product_aname = product.product_aname,
                             product_ename = product.product_ename,
                             product_description = f_product.string_serial,
                             product_image = product.product_image,
                             branch = product.branch,
                             tax_per = product.tax_per,
                             available_quantity = product.userswarehouse.Count == 0 ? 0 : product.userswarehouse.FirstOrDefault().available_quantity ?? 0,
                             price = product.userswarehouse.Count == 0 ? 0 : product.userswarehouse.FirstOrDefault().current_price ?? 0,
                             new_item = product.new_item??0
                         });
                }
                else
                {
                    productsObject = new List<Products>
                        (from product in await _context.usersWarehouse
                         .Where(x => product_id == null ? true : x.product_id == product_id)
                         .Include(x => x.product_data)
                         .ToListAsync()
                         join store in store_quan on product.product_id equals store.icode into final_product
                         from f_product in final_product.DefaultIfEmpty()
                         select new Products()
                         {
                             product_id = product.product_id,
                             product_aname = product.product_data.product_aname,
                             product_ename = product.product_data.product_ename,
                             product_description = f_product == null ? "" : f_product.string_serial,
                             product_image = product.product_data.product_image,
                             branch = product.product_data.branch,
                             tax_per = product.product_data.tax_per,
                             available_quantity = product.available_quantity ?? 0,
                             price = product.current_price ?? 0,
                             new_item = product.product_data.new_item??0
                         });
                    //productsObject = GetProductsObjectListFromInvoices(warehouseList);
                }

                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ProductsResponse(productsObject, status, message_laguage, error_code);
        }
        public async Task<ProductsResponse> get_products(int user_id, int is_own_database, int page , int pagesize )
        {
            List<Products> productsObject = new List<Products>();
            bool status = false;
            string message = "error";
            int error_code = 400;
            pagesize = 400;
            try
            {
                var products = await _context.products.ToListAsync();
                productsObject.AddRange(products);
                if (productsObject.Count() < pagesize)
                {
                    // No more pages to retrieve
                    error_code = 200;
                    status = true;
                }
                error_code = 200;
                status = true;

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ProductsResponse(productsObject, status, message_laguage, error_code);
        }
        public async Task<ProductsResponse> GetSalesProducts(int user_id, int is_own_database, string product_id = null, string all_products = null)
        {
            List<Products> productsObject = new List<Products>();
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
                var invoiceProducts = await _context.invoicesProducts.ToListAsync();
                // Group the invoice products by product ID and calculate the total price for each product
                var groupedProducts = invoiceProducts.GroupBy(ip => ip.product_id)
                    .Select(g => new ProductSale
                    {
                        ProductId = g.Key,
                        TotalPrice = (decimal)g.Sum(ip => ip.Price)
                    });
                // Order the grouped products by total price in descending order
                var orderedProducts = groupedProducts.OrderByDescending(p => p.TotalPrice);

                // Get the top 10 most selling products
                var topSellingProducts = orderedProducts.Take(10);

                //productsObject.AddRange((IEnumerable<Products>)topSellingProducts);

                error_code = 200;
                status = true;

                error_code = 200;
                status = true;

             

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new ProductsResponse(productsObject, status, message_laguage, error_code);
        }
        private async Task<List<BATCH>> get_items_stock(int user_id, int is_own_database, string product_id)
        {
            string itm_string = "";
            List<BATCH> list_of_stock = new List<BATCH>();
            try
            {
                var user_store = await _context.userStore.ToListAsync();
                if (user_store != null)
                {
                    List<string> list_of_exists_stores = new List<string>();
                    string branch = "";
                    string us_ftype = "";
                    string us_code = "";
                    string store_name = "";
                    foreach (var us in user_store)
                    {
                        if (us.user_id == user_id)
                        {
                            branch = us.bran;
                            us_ftype = us.ftype;
                            us_code = us.code;
                        }
                        else
                        {
                            list_of_exists_stores.Add(us.bran + us.ftype + us.code);
                        }
                    }
                    if (branch != "")
                    {
                        var wshow = await _context.wshow.Where(w => w.ftype != "0" && w.bran == branch && !list_of_exists_stores.Contains(w.bran + w.ftype + w.code)).ToListAsync();
                        if (wshow != null)
                        {
                            string tbl_obj;
                            string code2;
                            string store_quan_column = "";
                            foreach (var store in wshow)
                            {

                                if (store.bran == branch && store.ftype == us_ftype && store.code == us_code)
                                {
                                    store_name = store.name;
                                }
                                else
                                {
                                    code2 = store.ftype + store.code;
                                    tbl_obj = "s.";
                                    if (store.ftype == "1")
                                        tbl_obj = "w.";
                                    if (store_quan_column != "")
                                        store_quan_column += Environment.NewLine + " + ";
                                    store_quan_column += "'" + store.name + " : ' + CAST ((" + tbl_obj + "IAQ" + code2 + "+" + tbl_obj + "IPQ" + code2 + "-" + tbl_obj + "ISQ" + code2 + "-" + tbl_obj + "IRQ" + code2 + ") as nvarchar(100))";
                                }
                            }
                            if (store_quan_column != "")
                            {
                                if (is_own_database == 1)
                                {
                                    itm_string = "SELECT 0 AS int_serial, (" + store_quan_column + ") AS string_serial,"
                                    + " CAST(0 AS numeric(18, 0)) AS decimal_serial, b.BARCODE AS icode "
                                    + " FROM BARCODE" + branch + " b INNER JOIN ITMW" + branch
                                    + " w ON b.ICODE=w.ICODE INNER JOIN ITMS" + branch + " s ON w.ICODE=s.ICODE "
                                    + " WHERE b.BARCODE in (SELECT product_id FROM Products)";
                                    if (product_id != null)
                                        itm_string = itm_string + " AND b.BARCODE='" + product_id + "'";
                                }
                                else
                                {
                                    itm_string = "SELECT 0 AS int_serial, ('" + store_name + " : ' + CAST ((available_quantity) as nvarchar(100))) AS string_serial,"
                                                    + " CAST(0 AS numeric(18, 0)) AS decimal_serial, product_id AS icode "
                                                    + " FROM UsersWarehouse WHERE user_id = '" + Convert.ToString(user_id) + "' AND product_id in (SELECT product_id FROM Products)";
                                    if (product_id != null)
                                        itm_string = itm_string + " AND product_id='" + product_id + "'";
                                }
                                list_of_stock = await _context.batch.FromSqlRaw(itm_string).ToListAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                itm_string = ex.Message;
            }
            return list_of_stock;
        }
        public async Task<ProductsObjectResponse> CreateProduct(int user_id, Products product)
        {

            bool status = false;
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, "error");
            Products productsObject = new Products();
            try
            {
                //UserStoreResponse userStore = await new InvoiceProcedures(_connectionStr).GetUserStore(user_id);
                bool isSavedProduct = false;
                var userStore = await _context.userStore.ToListAsync<UserStore>();
                string product_id = await _context.products
                    .Where(p => p.product_id.Contains("mItem_" + Convert.ToString(user_id)))
                    .MaxAsync(p => p.product_id) ?? "0";
                string getserial_prefix = "mItem_" + Convert.ToString(user_id);
                if(product_id == "0")
                {
                    product_id = "1";
                }
                else
                {
                    int getserial_start_index = getserial_prefix.Length;
                    int getserial_Length = product_id.Length - getserial_prefix.Length;
                    product_id = product_id.Substring(getserial_start_index, getserial_Length);
                    if (string.IsNullOrEmpty(product_id))
                        product_id = "0";
                    product_id = Convert.ToString(Convert.ToInt64(product_id) + 1);

                }
                
                string new_product_id_zeros = "";
                string new_product_id = "";
                if (product_id.Length<10)
                {
                    for (int i = product_id.Length; i < 10; i++)
                        new_product_id_zeros += "0";
                }
                new_product_id = "mItem_" + Convert.ToString(user_id) + new_product_id_zeros + product_id;
                while (await _context.products.Where(p => p.product_id == new_product_id).FirstOrDefaultAsync() != null)
                {
                    if (Convert.ToString(Convert.ToInt64(product_id) + 1).Length > product_id.Length)
                        new_product_id_zeros.Substring(0, Convert.ToString(Convert.ToInt64(product_id) + 1).Length - product_id.Length);
                    product_id = Convert.ToString(Convert.ToInt64(product_id) + 1);
                    new_product_id = "mItem_" + Convert.ToString(user_id) + new_product_id_zeros + product_id;
                }
                foreach (var us in userStore)
                {
                    UsersWarehouse usersWarehouse = new UsersWarehouse()
                    {
                        id = null,
                        product_id = new_product_id,
                        user_id = us.user_id,
                        available_quantity = 0,
                        current_price = product.price ?? 0
                    };
                    await _context.usersWarehouse.AddAsync(usersWarehouse);
                    if (us.user_id == user_id)
                    {
                        if (product != null)
                        {
                            double tax_per = await _context.products.MaxAsync(p => p.tax_per) ?? 0;
                            product.product_id = new_product_id;
                            product.tax_per = tax_per;
                            product.branch = us.bran;
                            product.available_quantity = 0;
                            product.product_description = "";
                            product.new_item = 1;
                            await _context.products.AddAsync(product);
                            
                            isSavedProduct = true;
                        }
                        else
                        {
                            error_code = 302;
                            message_laguage = global.GetMessageLanguageFromCode(error_code);
                        }
                    }
                }
                if(isSavedProduct)
                {
                    int saveReturnValue = 0;
                    saveReturnValue = await _context.SaveChangesAsync();
                    if (saveReturnValue > 0)
                    {
                        productsObject = new Products
                        {
                            product_id = product.product_id,
                            product_aname = product.product_aname,
                            product_ename = product.product_ename,
                            product_description = "",
                            product_image = product.product_image,
                            branch = product.branch,
                            tax_per = product.tax_per,
                            available_quantity = product.available_quantity ?? 0,
                            price = product.price ?? 0,
                            new_item = product.new_item
                        };
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
                    error_code = 328;
                }                
            }
            catch (Exception ex)
            {
                message_laguage = global.GetMessageLanguageFromCode(error_code, ex.Message);
            }
            return new ProductsObjectResponse(productsObject, status, message_laguage, error_code);
        }
        public async Task<ProductsObjectResponse> UpdateProduct(int user_id, Products product)
        {
            bool status = false;
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, "error");
            Products productsObject = new Products();
            try
            {
                int saveReturnValue = 0;
                Products updateProduct = await _context.products.FindAsync(product.product_id);
                
                if (updateProduct != null)
                {
                    updateProduct.product_aname = product.product_aname;
                    updateProduct.product_ename = product.product_ename;
                    updateProduct.product_image = product.product_image;
                    if (updateProduct.price != product.price)
                    {
                        var usersWarehouse = await _context.usersWarehouse
                            .Where(w => w.product_id == product.product_id)
                            .ToListAsync();
                        usersWarehouse.ForEach(w => w.current_price = product.price);
                        updateProduct.price = product.price;
                    }

                    saveReturnValue = await _context.SaveChangesAsync();
                    if (saveReturnValue > 0)
                    {
                        productsObject = new Products
                        {
                            product_id = updateProduct.product_id,
                            product_aname = updateProduct.product_aname,
                            product_ename = updateProduct.product_ename,
                            product_description = "",
                            product_image = updateProduct.product_image,
                            branch = updateProduct.branch,
                            tax_per = updateProduct.tax_per,
                            available_quantity = updateProduct.available_quantity ?? 0,
                            price = updateProduct.price ?? 0,
                            new_item = updateProduct.new_item??0
                        }; 
                        
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
            return new ProductsObjectResponse(productsObject, status, message_laguage, error_code);
        }
        public async Task<MessageResponse> DeleteProduct(int user_id, string product_id)
        {
            bool status = false;
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, "error");
            Products productsObject = new Products();
            try
            {
                int saveReturnValue = 0;
                Products deleteProduct = await _context.products.FindAsync(product_id);
                if (deleteProduct != null)
                {
                    var checkToDeleteProduct = await _context.invoicesProducts.AnyAsync(i => i.product_id == deleteProduct.product_id);
                    if (!checkToDeleteProduct)
                    {
                        var userWarehouseToDelete = await _context.usersWarehouse.Where(w => w.product_id == deleteProduct.product_id).ToListAsync();
                        _context.usersWarehouse.RemoveRange(userWarehouseToDelete);
                        _context.products.Remove(deleteProduct);
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

    public class ProductSale
    {
        public string ProductId { get; set; }
        public decimal TotalPrice { get; set; }
    }


}
