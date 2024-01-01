using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;
using WaseetAPI.Domain.Models.Salla;

namespace WaseetAPI.Application.Salla
{
    public class ReceiveProcedures
    {
        private MainDbContext _maincontext;
        private ApplicationDbContext _applicationcontext;
        private GlobalProcedures global;
        private int user_online_type;
        SallaOrders order;
        UserStore user_store;
        string _receive_data;
        List<OrderNumbers> list_of_locations;
        public ReceiveProcedures(MainDbContext context)
        {
            _maincontext = context;
            global = new GlobalProcedures();
            order = new SallaOrders();
            user_store = new UserStore();
            list_of_locations = new List<OrderNumbers>();
        }
        public async Task<bool> save_receive_data(string receive_data, bool by_salla_user_id = false)
        {
            bool status = false;
            _receive_data = receive_data;
            try
            {
                dynamic eventObj = JsonConvert.DeserializeObject(receive_data);
                if (Convert.ToString(eventObj.@event) == "app.installed")
                {
                    savemerchantapp(Convert.ToDecimal(eventObj.merchant), Convert.ToDateTime(eventObj.created_at), Convert.ToString(eventObj.data.app_name));
                }
                else
                {
                    SallaReceive new_receive_data = JsonConvert.DeserializeObject<SallaReceive>(receive_data);                  
                    UsersConnection sallauser = await CheckAvailableUser(new_receive_data.merchant, by_salla_user_id);
                    if (sallauser.status)
                    {
                        _applicationcontext = new ApplicationDbContext(sallauser.connection);
                        user_online_type = sallauser.user_data.user_online_type ?? 1;
                        var webhook_request = await _maincontext.sallawebhookrequests.FirstOrDefaultAsync(r => r.salla_user_id == (sallauser.user_data.user_id ?? 0) && r.event_name == new_receive_data.Event && r.event_id == Convert.ToString(new_receive_data.data.id));
                        if (webhook_request == null)
                        {
                            webhook_request = new SallaWebhookRequests()
                            {
                                salla_user_id = sallauser.user_data.user_id ?? 0,
                                event_name = new_receive_data.Event,
                                event_id = Convert.ToString(new_receive_data.data.id)
                            };
                            await _maincontext.sallawebhookrequests.AddAsync(webhook_request);
                            await _maincontext.SaveChangesAsync();
                            status = await DoReceive(new_receive_data, sallauser.user_data.user_id ?? 0);
                            _maincontext.sallawebhookrequests.Remove(webhook_request);
                            await _maincontext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        await savelog(sallauser.message.Ar, 2);
                        await savelog(_receive_data, 1);
                    }
                }                
            }
            catch (Exception ex)
            {
                await savelog(ex.Message, 2);
                await savelog(_receive_data, 1);
            }
            return status;
        }
        private async Task<UsersConnection> CheckAvailableUser(string username, bool by_salla_user_id = false)
        {
            int user_id = 0;
            string user_name = "";
            string user_image = "";
            string user_address = "";
            int user_online_type = 1;
            bool status = false;
            string message = "error";
            int error_code = 400;
            string connectionStr = "";
            int is_own_database = 1;
            try
            {
                SallaUsers checkUser = await _maincontext.sallausers
                    .Where(x => (!by_salla_user_id && x.user_name == username) || (by_salla_user_id && x.user_id == Convert.ToInt32(username)))
                    .Include(x => x.company)
                    .FirstOrDefaultAsync();
                if (checkUser != null)
                {
                    if ((checkUser.activation_date ?? DateTime.Now.AddDays(-1)).Date >= DateTime.Now.Date)
                    {
                        if (checkUser.company.server_name != null && checkUser.company.server_name != ""
                                && checkUser.company.db_name != null && checkUser.company.db_name != ""
                                && checkUser.company.server_user_name != null && checkUser.company.server_user_name != ""
                                && checkUser.company.server_password != null && checkUser.company.server_password != "")
                        {
                            user_id = checkUser.user_id ?? 0;
                            user_name = checkUser.user_name;
                            user_online_type = checkUser.user_online_type ?? 1;
                            string decrpassword = global.Decrypt(checkUser.company.server_password);
                            connectionStr = "Data Source=" + checkUser.company.server_name + ";Initial Catalog=" + checkUser.company.db_name + ";User Id=" + checkUser.company.server_user_name + ";Password=" + decrpassword + ";";
                            is_own_database = checkUser.company.is_own_database;
                            status = true;
                            error_code = 200;
                        }
                        else
                        {
                            error_code = 315;
                        }
                    }
                    else
                    {
                        error_code = 317;
                    }
                }
                else
                {
                    error_code = 318;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new UsersConnection(user_id, user_name, user_image, user_address, user_online_type, is_own_database, connectionStr, status, message_laguage, error_code);
        }
        private async Task<bool> DoReceive(SallaReceive salla_receive, int salla_user_id)
        {
            bool status = false;
            try
            {
                if (salla_receive.data != null)
                {
                    TimeZoneInfo tst = OlsonTimeZoneToTimeZoneInfo(salla_receive.data.date.timezone);
                    DateTime salla_invoice_date = DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(salla_receive.data.date.date ?? DateTime.UtcNow, TimeZoneInfo.Utc, tst), DateTimeKind.Utc);

                    switch (salla_receive.Event)
                    {
                        case "order.created"://Create New Order
                            bool ret_fun_done = false;
                            try
                            {
                                order = salla_receive.data;
                                user_store = await _applicationcontext.userStore.FirstOrDefaultAsync(s => s.salla_user_id == salla_user_id);

                                ret_fun_done = await check_invoice_exists();
                                if (ret_fun_done)
                                {
                                    Customers customers = await _applicationcontext.customers.FirstOrDefaultAsync(c => c.customer_type == 5 && c.user_id == salla_user_id && c.customer_address == order.payment_method);
                                    if (customers != null)
                                        await set_payment_method(customers.customer_ename);
                                    ret_fun_done = await set_item_icode();
                                    if (ret_fun_done)
                                    {
                                        ret_fun_done = await set_vou_number();
                                        if (ret_fun_done)
                                        {
                                            decimal total_amount = 0;
                                            decimal sub_total_amount = 0;
                                            decimal tax_amount = 0;
                                            int record_change = 0;
                                            int number1_serial, code2_serial, total_amount_serial, sub_total_amount_serial, tax_amount_serial;
                                            string sql = "";
                                            string return_message = "";
                                            List<object> values = new List<object>();
                                            try
                                            {
                                                values.Clear();
                                                values.Add("001");//0
                                                values.Add(0);//1
                                                try
                                                {
                                                    values.Add(salla_invoice_date);//2                                             
                                                }
                                                catch (FormatException)
                                                {
                                                    values.Add(order.date.date);//2
                                                }
                                                values.Add(order.customer.first_name + " " + order.customer.last_name);//3
                                                values.Add(user_store.baseet_user_no);//4
                                                values.Add(1);//5
                                                values.Add(user_store.cash_account);//6
                                                values.Add(user_store.sales_account);//7
                                                values.Add(user_store.tax_account);//8
                                                values.Add("");//9
                                                values.Add(2);//10
                                                values.Add(3);//11
                                                values.Add(9);//12
                                                values.Add(4);//13
                                                values.Add(6);//14
                                                values.Add(order.payment_method_code);//15
                                                values.Add(order.payment_method);//16
                                                if (order.payment_method_name != null)
                                                    values.Add(order.payment_method_name.Ar);//17
                                                else
                                                    values.Add(null);//17
                                                values.Add(order.payment_method_com_code);//18
                                                values.Add(5);//19
                                                values.Add(7);//20
                                                values.Add(order.id);//21
                                                values.Add(order.reference_id);//22
                                                foreach (var loc_number1 in list_of_locations)
                                                {
                                                    sub_total_amount = order.items.Where(c => c.code2 == loc_number1.code2).Sum(i => i.amounts.price_without_tax.amount);
                                                    tax_amount = order.items.Where(c => c.code2 == loc_number1.code2).Sum(i => i.amounts.tax.amount.amount);
                                                    total_amount = sub_total_amount + tax_amount;// order.items.Where(c => c.code2 == loc_number1.code2).Sum(i => i.amounts.total.amount);
                                                    number1_serial = values.Count;
                                                    values.Add(loc_number1.number1);
                                                    code2_serial = values.Count;
                                                    values.Add(loc_number1.code2);
                                                    total_amount_serial = values.Count;
                                                    values.Add(total_amount);
                                                    sub_total_amount_serial = values.Count;
                                                    values.Add(sub_total_amount);
                                                    tax_amount_serial = values.Count;
                                                    values.Add(tax_amount);
                                                    ret_fun_done = create_invoice(ref sql, ref values, ref return_message, loc_number1.number1, loc_number1.code2, total_amount, sub_total_amount, tax_amount, number1_serial, code2_serial, total_amount_serial, sub_total_amount_serial, tax_amount_serial);
                                                }
                                                if ((ret_fun_done) && (sql != ""))
                                                {
                                                    try
                                                    {
                                                        await _applicationcontext.Database.BeginTransactionAsync();
                                                        record_change = await _applicationcontext.Database.ExecuteSqlRawAsync(sql, values);
                                                        if (record_change > 0)
                                                        {
                                                            await _applicationcontext.Database.CommitTransactionAsync();
                                                            status = true;
                                                        }
                                                        else
                                                        {
                                                            await _applicationcontext.Database.RollbackTransactionAsync();
                                                            await savelog(_receive_data, 1);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        await _applicationcontext.Database.RollbackTransactionAsync();
                                                        await savelog(ex.Message, 2);
                                                        await savelog(_receive_data, 1);
                                                    }

                                                }
                                                else
                                                {
                                                    if (return_message != "")
                                                        await savelog(return_message, 2);
                                                    await savelog(_receive_data, 1);
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                await savelog(ex.Message, 2);
                                                await savelog(_receive_data, 1);
                                            }

                                        }
                                        else
                                        {
                                            await savelog(_receive_data, 1);
                                        }
                                    }
                                    else
                                    {
                                        await savelog(_receive_data, 1);
                                    }
                                }                                
                            }
                            catch (Exception ex)
                            {
                                await savelog(ex.Message, 2);
                                await savelog(_receive_data, 1);
                            }
                            break;
                        case "":
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await savelog(ex.Message, 2);
                await savelog(_receive_data, 1);
            }

            //try
            //{
            //    //save receive to database
            //    salla_receive.status = status ? 1 : 2;
            //    _maincontext.sallareceive.Add(salla_receive);
            //    _maincontext.SaveChanges();
            //}
            //catch
            //{

            //}

            return status;
        }
        private async Task<bool> check_invoice_exists()
        {
            bool return_value = true;
            string check_exists_string = "";
            string order_id = Convert.ToString(order.id);
            try
            {
                check_exists_string = "IF EXISTS (SELECT INVOICE_NO FROM VOU" + user_store.bran + " WHERE INVOICE_NO = '" + order_id + "') " +
                        " BEGIN " +
                        "   SELECT 0 AS int_serial,'' AS string_serial,CAST('" + order_id + "' AS numeric(18, 0)) AS decimal_serial, '' AS icode " +
                        " END " +                         
                        " ELSE " +                         
                        " BEGIN " +                    
                        "   SELECT 0 AS int_serial, '' AS string_serial,CAST(0 AS numeric(18, 0)) AS decimal_serial, '' AS icode " +                    
                        " END ";

                var check_exists = await _applicationcontext.batch.FromSqlRaw(check_exists_string).ToListAsync();
                if (check_exists != null)
                {
                    if (check_exists.First().decimal_serial > 0)
                        return_value = false;
                }
            }
            catch (Exception ex)
            {
                await savelog(ex.Message, 2);
                return_value = false;
            }
            return return_value;
        }
        private bool create_invoice(ref string sql_query, ref List<object> sql_query_values, ref string return_message, float number1, string code2, decimal total_amount, decimal sub_total_amount, decimal tax_amount,int number1_serial,int code2_serial,int total_amount_serial, int sub_total_amount_serial, int tax_amount_serial)
        {
            bool return_value = false;
            bool not_return_value = false;
            string sql = "";
            return_message = "";
            //List<object> values = new List<object>();
            try
            {
                return_value = get_vou_insert_query(ref sql, ref sql_query_values, number1, code2, total_amount, sub_total_amount, tax_amount,number1_serial, code2_serial,total_amount_serial,sub_total_amount_serial,tax_amount_serial);
                if (return_value)
                {
                    sql_query += sql;
                    return_value = get_bin_insert_query(ref sql, ref sql_query_values, number1, code2, number1_serial, code2_serial);
                    if (return_value)
                    {
                        sql_query += sql;
                        return_value = get_inv_insert_query(ref sql, ref sql_query_values, number1, code2, number1_serial, code2_serial);
                        if (return_value)
                        {
                            sql_query += sql;
                            not_return_value = get_cod_update_query(ref sql, ref sql_query_values, total_amount, sub_total_amount, tax_amount, total_amount_serial, sub_total_amount_serial, tax_amount_serial);
                            if (not_return_value)
                                sql_query += sql;
                            not_return_value = get_itm_update_query(ref sql, ref sql_query_values, code2);
                            if (not_return_value)
                                sql_query += sql;
                            not_return_value = get_batch_update_query(ref sql, ref sql_query_values, code2);
                            if (not_return_value)
                                sql_query += sql;
                            if (user_online_type == 2)
                            {
                                not_return_value = get_sitm_update_query(ref sql, ref sql_query_values, number1, code2, number1_serial, code2_serial);
                                if (not_return_value)
                                    sql_query += sql;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return_message = ex.Message;
            }
            return return_value;
        }
        private async Task<bool> set_item_icode()
        {
            bool return_value = false;
            string itm_string = "";
            try
            {
                string code2 = "";
                string icode = "";
                int startIndex = -1;
                list_of_locations = new List<OrderNumbers>();
                foreach (var item in order.items)
                {
                    code2 = "";
                    icode = "";
                    startIndex = item.sku.IndexOf("-");
                    if (startIndex >= 0)
                    {
                        code2 = item.sku.Substring(0, startIndex);
                        icode = item.sku.Substring(startIndex + 1);
                    }
                    else
                    {
                        code2 = user_store.ftype + user_store.code;
                        icode = item.sku;
                    }
                    if (list_of_locations.Find(n => n.code2 == code2) == null)
                        list_of_locations.Add(new OrderNumbers() { code2 = code2, number1 = 1 });
                    itm_string = "IF EXISTS (SELECT ICODE FROM ITM" + user_store.bran + " WHERE ICODE = '" + icode + "') " +
                        " BEGIN " +
                        "   SELECT 0 AS int_serial,'' AS string_serial,CAST(0 AS numeric(18, 0)) AS decimal_serial, '" + icode + "' AS icode " +
                        " END " +
                        " ELSE IF EXISTS (SELECT ICODE FROM ITM" + user_store.bran + " WHERE ICODE1 = '" + icode + "') " +
                        " BEGIN " +
                        "   SELECT 0 AS int_serial,'' AS string_serial,CAST(0 AS numeric(18, 0)) AS decimal_serial, ICODE AS icode FROM ITM" + user_store.bran + " WHERE ICODE1 = '" + icode + "'" +
                        " END " +
                        " ELSE IF EXISTS (SELECT ICODE FROM ITM" + user_store.bran + " WHERE ICODE2 = '" + icode + "') " +
                        " BEGIN " +
                        "   SELECT 0 AS int_serial,'' AS string_serial,CAST(0 AS numeric(18, 0)) AS decimal_serial, ICODE AS icode FROM ITM" + user_store.bran + " WHERE ICODE2 = '" + icode + "'" +
                        " END " +
                        " ELSE IF EXISTS (SELECT ICODE FROM ITM" + user_store.bran + " WHERE ICODE3 = '" + icode + "') " +
                        " BEGIN " +
                        "   SELECT 0 AS int_serial,'' AS string_serial,CAST(0 AS numeric(18, 0)) AS decimal_serial, ICODE AS icode FROM ITM" + user_store.bran + " WHERE ICODE3 = '" + icode + "'" +
                        " END ";
                    if (user_online_type == 2)
                    {
                        itm_string += " ELSE IF EXISTS (SELECT SER FROM SITM" + user_store.bran + " WHERE SER = '" + icode + "') " +
                            " BEGIN " +
                            "   SELECT 0 AS int_serial, SER AS string_serial,CAST(0 AS numeric(18, 0)) AS decimal_serial, ICODE AS icode FROM SITM" + user_store.bran + " WHERE SER = '" + icode + "'" +
                            " END ";
                    }
                    else
                    {
                        itm_string += " ELSE IF EXISTS (SELECT BATCH_SER FROM BATCH" + user_store.bran + " WHERE BATCH_SER = '" + icode + "') " +
                            " BEGIN " +
                            "   SELECT BATCH_SER AS int_serial,'' AS string_serial,CAST(0 AS numeric(18, 0)) AS decimal_serial, ICODE AS icode FROM BATCH" + user_store.bran + " WHERE BATCH_SER = '" + icode + "'" +
                            " END ";
                    }
                    itm_string += " ELSE " +
                        " BEGIN " +
                        "   SELECT 0 AS int_serial, '' AS string_serial,CAST(0 AS numeric(18, 0)) AS decimal_serial, '" + icode + "' AS icode " +
                        " END ";
                    
                    var get_icode = await _applicationcontext.batch.FromSqlRaw(itm_string).ToListAsync();
                    if (get_icode != null)
                    {
                        if (user_online_type == 2)
                        {
                            item.sku = get_icode.First().string_serial;
                        }
                        else
                        {
                            item.sku = Convert.ToString(get_icode.First().int_serial);
                        }
                        item.icode = get_icode.First().icode;
                        item.code2 = code2;
                    }
                }

                return_value = true;
            }
            catch(Exception ex)
            {
                await savelog(ex.Message,2);
            }
            return return_value;
        }
        private async Task<bool> set_vou_number()
        {
            bool return_value = false;
            float vou_number = 1;
            string get_serial_sql = "";
            string get_serial_sql_select1 = "";
            string get_serial_sql_select2 = "";
            string get_serial_sql_where = "";
            try
            {
                foreach (var loc_number1 in list_of_locations)
                {
                    if (get_serial_sql_select1 != "")
                        get_serial_sql_select1 += " UNION ALL ";
                    if (get_serial_sql_select2 != "")
                        get_serial_sql_select2 += " UNION ALL ";

                    get_serial_sql_where = " WHERE BRAN = '" + user_store.bran + "' AND BTYPE='" + loc_number1.code2.Substring(0, 1) +
                                           "' AND CODE='" + loc_number1.code2.Substring(1, loc_number1.code2.Length - 1) + "' AND FTYPE='001' AND FTYPE2=0" +
                                           " GROUP BY BRAN ,BTYPE,CODE,FTYPE,FTYPE2 ,SERIAL ";
                    get_serial_sql_select1 += "   SELECT SERIAL AS int_serial,'" + loc_number1.code2 + "' AS string_serial, CAST(0 AS numeric(18, 0)) AS decimal_serial, '1' AS icode FROM SERIALS " + get_serial_sql_where;
                    get_serial_sql_select2 += "   SELECT 0 AS int_serial,'" + loc_number1.code2 + "' AS string_serial, SERIAL AS decimal_serial, '2' AS icode FROM SERIALS " + get_serial_sql_where;                    
                    //if (user_online_type == 2)
                    //    get_serial_sql += "";
                    //else
                    //    get_serial_sql += "";                    
                }
                if (get_serial_sql_select1 != "" && get_serial_sql_select2 != "")
                {
                    get_serial_sql = "IF EXISTS (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SERIALS' AND COLUMN_NAME = 'SERIAL' AND DATA_TYPE = 'int') " +
                        " BEGIN " +
                        get_serial_sql_select1 +
                        " END " +
                        " ELSE " +
                        " BEGIN " +
                        get_serial_sql_select2 +
                        " END ";
                }
                if (get_serial_sql != "")
                {
                    var get_serial = await _applicationcontext.batch.FromSqlRaw(get_serial_sql).ToListAsync();
                    string set_serial_sql = "";
                    List<object> values = new List<object>();
                    values.Clear();
                    values.Add(user_store.bran);
                    values.Add("001");
                    values.Add(0);
                    int get_serial_serial = 3;
                    if (get_serial.Count > 0)
                    {
                        list_of_locations = new List<OrderNumbers>();
                        foreach (var serial in get_serial)
                        {
                            if (serial.icode == "1")
                            {
                                vou_number = serial.int_serial + 1;
                            }
                            else
                            {
                                vou_number = (float)(serial.decimal_serial) + 1;
                            }
                            list_of_locations.Add(new OrderNumbers() { code2 = serial.string_serial, number1 = vou_number });

                            values.Add(vou_number);
                            values.Add(serial.string_serial.Substring(0, 1));
                            values.Add(serial.string_serial.Substring(1, serial.string_serial.Length - 1));
                            set_serial_sql += "UPDATE SERIALS SET SERIAL={" + Convert.ToString(get_serial_serial) + "} WHERE BRAN = {0} AND BTYPE={" + Convert.ToString(get_serial_serial + 1) + "} AND CODE={" + Convert.ToString(get_serial_serial + 2) + "} AND FTYPE={1} AND FTYPE2={2};";
                            get_serial_serial += 3;
                        }
                    }
                    else
                    {
                        values.Add(vou_number);
                        get_serial_serial = 4;
                        foreach (var loc_number1 in list_of_locations)
                        {
                            values.Add(loc_number1.code2.Substring(0, 1));
                            values.Add(loc_number1.code2.Substring(1, loc_number1.code2.Length - 1));
                            set_serial_sql += "INSERT INTO SERIALS(SERIAL,BRAN,BTYPE,CODE,FTYPE,FTYPE2) VALUES({3},{0},{" + Convert.ToString(get_serial_serial) + "},{" + Convert.ToString(get_serial_serial + 1) + "},{1},{2});";
                            get_serial_serial += 2;
                        }
                    }
                    await _applicationcontext.Database.ExecuteSqlRawAsync(set_serial_sql, values);
                    return_value = true;
                }
                ////SERIALS serial = new SERIALS();
                ////serial = await _applicationcontext.serials.Where(s => s.BRAN == user_store.bran && s.BTYPE == user_store.ftype && 
                ////s.CODE == user_store.code && s.FTYPE == "001" && s.FTYPE2 == 0).FirstOrDefaultAsync();
                ////if (serial != null)
                ////{
                ////    vou_number = (serial.SERIAL ?? 0) + 1;
                ////    serial.SERIAL = serial.SERIAL + 1;
                ////}
                ////else
                ////{
                ////    serial = new SERIALS()
                ////    {
                ////        BRAN = user_store.bran,
                ////        BTYPE = user_store.ftype,
                ////        CODE = user_store.code,
                ////        FTYPE = "001",
                ////        FTYPE2 = 0,
                ////        SERIAL = 1
                ////    };
                ////    _applicationcontext.serials.Add(serial);
                ////}
                ////_applicationcontext.SaveChanges();
                //order.number1 = vou_number;

            }
            catch (Exception ex)
            {
                await savelog(ex.Message, 2);
            }
            return return_value;
        }
        private async Task<bool> set_payment_method(string method_name)
        {
            bool return_value = false;
            try
            {
                string get_payment_sql = "SELECT FTYPE,NAME,NAME2,CODE,DNAME,PER,ctype,WPOS,COM_CODE,MAX_COMMISION,INCODE,INDNAME,ID,ID_TYPE,ID_DATE,ID_ALL,COM_NAME,pay_type FROM ANET" + user_store.bran;
                get_payment_sql += " WHERE NAME = '" + method_name + "'";                
                var get_serial = await _applicationcontext.anet.FromSqlRaw(get_payment_sql).ToListAsync();
                if (get_serial != null)
                {
                    order.payment_method_name = new Languages() { Ar = get_serial.First().NAME, En = get_serial.First().NAME2 };
                    order.payment_method_code = get_serial.First().CODE;
                    order.payment_method_com_per = get_serial.First().PER ?? 0;
                    order.payment_method_com_code = get_serial.First().COM_CODE;
                    order.payment_method_com_max = get_serial.First().MAX_COMMISION ?? 0;
                }
                return_value = true;
            }
            catch (Exception ex)
            {
                await savelog(ex.Message, 2);
            }
            return return_value;
        }
        private bool get_vou_insert_query(ref string sql_query, ref List<object> sql_query_values, float number1, string code2, decimal total_amount, decimal sub_total_amount, decimal tax_amount,int number1_serial, int code2_serial,int total_amount_serial, int sub_total_amount_serial, int tax_amount_serial)
        {
            bool return_value = false;
            string vou_string = "";
            string vou_feilds = "FTYPE,FTYPE2,CODE2,NUMBER1,FDATE,NAME,BAMOUNT,TAX_AMOUNT,USER_NO";
            if (user_online_type != 2)
                vou_feilds += ",CALL_NO";
            vou_feilds += ",U_DATE_A,U_DATE_E,CH_RATE2,CH_RATE,CHANGE,TOTDISCOUNT,COST,PAIDE,DLVTYPE,PAID,INVOICE_NO"
                + ",FCODE,TCODE,AMOUNT,AMOUNT2,SERIAL,INVACCPOS";
            string vou_values = "";
            string vou_values_const = "";
            string vou_values_other = "";
            try
            {
                //sql_query_values.Clear();
                //sql_query_values.Add("001");
                //sql_query_values.Add("0");
                //sql_query_values.Add(code2);
                //sql_query_values.Add(number1);
                //sql_query_values.Add(order.date.date);
                //sql_query_values.Add(order.customer.first_name + " " + order.customer.last_name);
                //sql_query_values.Add(total_amount);//order.amounts.total.amount
                //sql_query_values.Add(tax_amount);//order.amounts.tax.amount.amount
                //sql_query_values.Add(user_store.baseet_user_no);
                //sql_query_values.Add(1);
                //sql_query_values.Add(order.date.date);
                //sql_query_values.Add(order.date.date);
                //sql_query_values.Add(1);
                //sql_query_values.Add(1);
                //sql_query_values.Add(0);
                //sql_query_values.Add(0);
                //sql_query_values.Add(0);
                //sql_query_values.Add(0);
                //sql_query_values.Add(0);
                //sql_query_values.Add(total_amount); //order.amounts.total.amount
                 
                //vou_values_const = "{0},{1},{2},{3},{4},{5},{6},{7},{8}";
                //if (user_online_type != 2)
                //    vou_values_const += ",{9}";
                //vou_values_const +=",{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}";


                vou_values_const = "{0},{1},{"+Convert.ToString(code2_serial) + "},"+ 
                    "{" + Convert.ToString(number1_serial) + "},{2},{3},"+ 
                    "{" + Convert.ToString(total_amount_serial) + "},{" + Convert.ToString(tax_amount_serial) + "},{4}";
                if (user_online_type != 2)
                    vou_values_const += ",{5}";
                vou_values_const += ",{2},{2},{5},{5},{1},{1},{1},{1},{1},{" + Convert.ToString(total_amount_serial) + "},{21}";

                //sql_query_values.Add(user_store.cash_account);
                //sql_query_values.Add("");
                //sql_query_values.Add(total_amount);
                //sql_query_values.Add(total_amount);
                //sql_query_values.Add(1);
                //sql_query_values.Add(1);

                vou_values_other = ",{6},{9}";//"{20},{21}";
                decimal payment_method_amount_com = 0;
                decimal payment_method_amount = 0;
                if (order.payment_method_code != "" && order.payment_method_code != null && order.payment_method_code != user_store.cash_account)
                {
                    vou_values_other += ",{1},{1}";//",{38},{38}";
                    payment_method_amount = total_amount;
                    payment_method_amount_com = Math.Round(total_amount * Convert.ToDecimal(order.payment_method_com_per) / 100, 2);
                    if (order.payment_method_com_max != 0 && Convert.ToDecimal(order.payment_method_com_max) < payment_method_amount_com)
                    {
                        payment_method_amount_com = Convert.ToDecimal(order.payment_method_com_max);
                    }
                    payment_method_amount = payment_method_amount - payment_method_amount_com;
                }
                else
                {
                    vou_values_other += ",{" + Convert.ToString(total_amount_serial) + "},{" + Convert.ToString(total_amount_serial) + "}";//",{22},{23}";
                }
                vou_values_other += ",{5},{5}";//",{24},{25}";
                vou_values = vou_values_const + vou_values_other;
                vou_string = "INSERT INTO VOU" + user_store.bran + " (" + vou_feilds + ") " + " VALUES " + " (" + vou_values + ");";

                //sql_query_values.Add("");
                //sql_query_values.Add(user_store.sales_account);
                //sql_query_values.Add(sub_total_amount);
                //sql_query_values.Add(sub_total_amount);
                //sql_query_values.Add(2);
                //sql_query_values.Add(3);

                vou_values_other = ",{9},{7},{" + Convert.ToString(sub_total_amount_serial) + "}," + 
                    "{" + Convert.ToString(sub_total_amount_serial) + "},{10},{11}";//"{26},{27},{28},{29},{30},{31}";
                vou_values = vou_values_const + vou_values_other;
                vou_string += "INSERT INTO VOU" + user_store.bran + " (" + vou_feilds + ") " + " VALUES " + " (" + vou_values + ");";

                //sql_query_values.Add("");
                //sql_query_values.Add(user_store.tax_account);
                //sql_query_values.Add(tax_amount);
                //sql_query_values.Add(tax_amount);
                //sql_query_values.Add(3);
                //sql_query_values.Add(9);

                vou_values_other = ",{9},{8},{" + Convert.ToString(tax_amount_serial) + "}," + 
                    "{" + Convert.ToString(tax_amount_serial) + "},{11},{12}";// "{32},{33},{34},{35},{36},{37}";
                vou_values = vou_values_const + vou_values_other;
                vou_string += "INSERT INTO VOU" + user_store.bran + " (" + vou_feilds + ") " + " VALUES " + " (" + vou_values + ");";


                //sql_query_values.Add(0);
                if (payment_method_amount > 0)
                {
                    vou_values_other = ",{15},{9}";
                    //sql_query_values.Add(order.payment_method_code);
                    //sql_query_values.Add("");
                    vou_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "},{" + Convert.ToString(sql_query_values.Count) + "}";
                    sql_query_values.Add(payment_method_amount);
                    //sql_query_values.Add(payment_method_amount);
                    vou_values_other += ",{13},{14},{16},{17}";
                    //sql_query_values.Add(4);
                    //sql_query_values.Add(6);
                    //sql_query_values.Add(order.payment_method);
                    //sql_query_values.Add(order.payment_method_name.Ar);

                    //vou_values_other = ",{39},{40},{41},{42},{43},{44},{45},{46}";
                    vou_values = vou_values_const + vou_values_other;
                    vou_string += "INSERT INTO VOU" + user_store.bran + " (" + vou_feilds + ",REMARK,RELA_REF) " + " VALUES " + " (" + vou_values + ");";

                    if (payment_method_amount_com > 0)
                    {
                        vou_values_other = ",{18},{9}";
                        //sql_query_values.Add(order.payment_method_com_code);
                        //sql_query_values.Add("");
                        vou_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "},{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(payment_method_amount_com);
                        vou_values_other += ",{19},{20}";
                        //sql_query_values.Add(payment_method_amount_com);
                        //sql_query_values.Add(5);
                        //sql_query_values.Add(7);

                        //vou_values_other = "{47},{48},{49},{50},{51},{52}";
                        vou_values = vou_values_const + vou_values_other;
                        vou_string += "INSERT INTO VOU" + user_store.bran + " (" + vou_feilds + ") " + " VALUES " + " (" + vou_values + ");";
                    }

                }
                return_value = true;
            }
            catch
            {
                vou_string = "";
            }
            sql_query = vou_string;
            return return_value;
        }
        private bool get_bin_insert_query(ref string sql_query, ref List<object> sql_query_values, float number1, string code2, int number1_serial, int code2_serial)
        {
            bool return_value = false;
            string bin_string = "";
            string bin_feilds = "FTYPE,FTYPE2,CODE2,NUMBER1,FDATE,FNUMT,FNUMQ,DLVNO,DLVTYPE,U_DATE_A,U_DATE_E,CH_RATE" +
                ",FCODE,TCODE,TAX_ACC,USER_NO," +
                "ICODE,IDSCR,IDSCR2,PRICE,QUAN,AMOUNT,IUNIT,IUNITF,ITYPE,BAMOUNT,SERIAL,TAX_AMOUNT,TAX_per";
            if (user_online_type == 2)
                bin_feilds += ",ISER";
            else
                bin_feilds += ",BATCH_SER";
            string bin_values = "";
            string bin_values_const = "";
            string bin_values_other = "";
            try
            {
                int item_serial = 1;
                //sql_query_values.Clear();
                //sql_query_values.Add("001");
                //sql_query_values.Add("0");
                //sql_query_values.Add(code2);
                //sql_query_values.Add(number1);
                //sql_query_values.Add(order.date.date);
                //sql_query_values.Add(code2.Substring(0,1));
                //sql_query_values.Add(code2.Substring(1));
                //sql_query_values.Add("0");
                //sql_query_values.Add("0");
                //sql_query_values.Add(order.date.date);
                //sql_query_values.Add(order.date.date);
                //sql_query_values.Add("1");
                //sql_query_values.Add(user_store.cash_account);
                //sql_query_values.Add(user_store.sales_account);
                //sql_query_values.Add(user_store.tax_account);
                //sql_query_values.Add(user_store.baseet_user_no);

                bin_values_const = "{0},{1},{"+Convert.ToString(code2_serial)+ "},{" + Convert.ToString(number1_serial) + "}," + 
                    "{2}";// "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}";
                bin_values_const += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                sql_query_values.Add(code2.Substring(0,1));
                bin_values_const += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                sql_query_values.Add(code2.Substring(1));
                bin_values_const += ",{1},{1},{2},{2},{5},{6},{7},{8},{4}";


                //int item_value_number = 15;
                foreach (var item in order.items)
                {
                    if (item.icode != "" && item.code2 == code2)
                    {
                        //bin_values_other = "";
                        //for (int i = 1; i <= 14; i++)
                        //{
                        //    item_value_number += 1;
                        //    bin_values_other += ",{" + Convert.ToString(item_value_number) + "}";
                        //}
                        //sql_query_values.Add(item.icode);
                        //sql_query_values.Add(item.name);
                        //sql_query_values.Add("");
                        //sql_query_values.Add(item.amounts.price_without_tax.amount);
                        //sql_query_values.Add(item.quantity);
                        //sql_query_values.Add(item.amounts.total.amount);
                        //sql_query_values.Add("");
                        //sql_query_values.Add("1");
                        //sql_query_values.Add("1");
                        //sql_query_values.Add(item.amounts.total.amount);
                        //sql_query_values.Add(item_serial);
                        //sql_query_values.Add(item.amounts.tax.amount.amount);
                        //sql_query_values.Add(item.amounts.tax.percent);
                        //sql_query_values.Add(item.sku);

                        bin_values_other = ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item.icode);
                        bin_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item.name);
                        bin_values_other += ",{9}";
                        bin_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item.amounts.price_without_tax.amount);
                        bin_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item.quantity);
                        bin_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item.amounts.total.amount);
                        bin_values_other += ",{9},{5},{5}";
                        bin_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item.amounts.total.amount);
                        bin_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item_serial);
                        bin_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item.amounts.tax.amount.amount);
                        bin_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item.amounts.tax.percent);
                        bin_values_other += ",{" + Convert.ToString(sql_query_values.Count) + "}";
                        sql_query_values.Add(item.sku);

                        bin_values = bin_values_const + bin_values_other;
                        bin_string += "INSERT INTO BIN" + user_store.bran + " (" + bin_feilds + ") " + " VALUES " + " (" + bin_values + ");";
                        return_value = true;
                        item_serial += 1;
                    }                    
                }                
            }
            catch
            {
                bin_string = "";
                return_value = false;
            }
            sql_query = bin_string;
            return return_value;
        }
        private bool get_inv_insert_query(ref string sql_query, ref List<object> sql_query_values, float number1, string code2, int number1_serial, int code2_serial)
        {
            bool return_value = false;
            string inv_string = "";
            try
            {
                //sql_query_values.Clear();
                //sql_query_values.Add("001");
                //sql_query_values.Add("0");
                //sql_query_values.Add(code2);
                //sql_query_values.Add(number1);
                //sql_query_values.Add(user_store.cash_account);
                //sql_query_values.Add(user_store.sales_account);
                //sql_query_values.Add(order.customer.first_name + " " + order.customer.last_name);
                //sql_query_values.Add(order.date.date);
                //sql_query_values.Add(order.date.date);
                //sql_query_values.Add(1);
                inv_string = "INSERT INTO INV" + user_store.bran +
                    " (FTYPE,FTYPE2,CODE2,NUMBER1,FCODE,TCODE,FNAME,U_DATE_A,U_DATE_E,SallaType,STRING1) " + 
                    " VALUES " +
                    " ({0},{1},{"+ Convert.ToString(code2_serial)+ "},{" + Convert.ToString(number1_serial) + "},"+
                    "{6},{7},{3},{2},{2},{5},{22});";
                return_value = true;
            }
            catch
            {
                inv_string = "";
            }
            sql_query = inv_string;
            return return_value;
        }
        private bool get_cod_update_query(ref string sql_query, ref List<object> sql_query_values, decimal total_amount, decimal sub_total_amount, decimal tax_amount, int total_amount_serial,int sub_total_amount_serial,int tax_amount_serial)
        {
            bool return_value = false;
            string cod_string = "";
            try
            {
                //sql_query_values.Clear();
                //sql_query_values.Add(total_amount);
                //sql_query_values.Add(user_store.cash_account);
                //sql_query_values.Add(sub_total_amount);
                //sql_query_values.Add(user_store.sales_account);
                //sql_query_values.Add(tax_amount);
                //sql_query_values.Add(user_store.tax_account);
                cod_string = "UPDATE COD" + user_store.bran +
                    " SET DDACC = DDACC + {"+Convert.ToString(total_amount_serial) +"}, " +
                    " DDACCCURR = DDACCCURR + {" + Convert.ToString(total_amount_serial) + "} WHERE DCODE1 = {6} ;";
                cod_string += "UPDATE COD" + user_store.bran +
                    " SET DCACC = DCACC + {" + Convert.ToString(sub_total_amount_serial) + "}, " +
                    " DCACCCURR = DCACCCURR + {" + Convert.ToString(sub_total_amount_serial) + "} WHERE DCODE1 = {7} ;";
                cod_string += "UPDATE COD" + user_store.bran +
                    " SET DCACC = DCACC + {" + Convert.ToString(tax_amount_serial) + "}, " + 
                    " DCACCCURR = DCACCCURR + {" + Convert.ToString(tax_amount_serial) + "} WHERE DCODE1 = {8} ;";
                return_value = true;
            }
            catch
            {
                cod_string = "";
            }
            sql_query = cod_string;
            return return_value;
        }
        private bool get_itm_update_query(ref string sql_query, ref List<object> sql_query_values, string code2)
        {
            bool return_value = false;
            string itm_string = "";
            string itms_table = "";
            int item_parameter_count = 0;
            try
            {
                //sql_query_values.Clear();
                foreach (var item in order.items)
                {
                    item_parameter_count = sql_query_values.Count;
                    sql_query_values.Add(item.quantity);
                    sql_query_values.Add(item.weight);
                    sql_query_values.Add(item.icode);
                    string itm_string_additional = "";
                    string itm_string_additional2 = "";
                    //string code2 = user_store.ftype + user_store.code;
                    if (code2.Substring(0,1) == "1")
                    {
                        itms_table = "ITMW" + user_store.bran;
                    }
                    else if (code2.Substring(0,1) == "2")
                    {
                        itms_table = "ITMS" + user_store.bran;
                        itm_string_additional += ", ISOLDQs = ISOLDQs + {" + Convert.ToString(item_parameter_count) + "} ";
                    }
                    if (user_online_type == 2)
                    {
                        itm_string_additional += ", ISOLDW = ISOLDW + {" + Convert.ToString(item_parameter_count+1) + "} ";
                        itm_string_additional2 += ", ISW" + code2 + " = ISW" + code2 + " + {" + Convert.ToString(item_parameter_count+1) + "}";
                    }
                    itm_string += "UPDATE ITM" + user_store.bran +
                        " SET ISOLDQ = ISOLDQ + {" + Convert.ToString(item_parameter_count) + "} " + 
                        itm_string_additional + " WHERE ICODE = {" + Convert.ToString(item_parameter_count+2) + "};";
                    itm_string += "UPDATE " + itms_table +
                            " SET ISQ" + code2 + " = ISQ" + code2 + " + {" + Convert.ToString(item_parameter_count) + 
                            "} " + itm_string_additional2 + " WHERE ICODE = {" + Convert.ToString(item_parameter_count+2) + "};";
                    //item_parameter_count += 3;
                    
                }
                if (itm_string != "")
                    return_value = true;
            }
            catch
            {
                itm_string = "";
            }
            sql_query = itm_string;
            return return_value;
        }
        private bool get_batch_update_query(ref string sql_query, ref List<object> sql_query_values, string code2)
        {
            bool return_value = false;
            string itm_string = "";
            int item_parameter_count = 0;
            //string code2 = user_store.ftype + user_store.code;
            try
            {
                //sql_query_values.Clear();
                foreach (var item in order.items)
                {
                    if (item.sku != "" && item.sku != "0")
                    {
                        item_parameter_count = sql_query_values.Count;
                        sql_query_values.Add(item.quantity);
                        sql_query_values.Add(item.sku);
                        itm_string += "UPDATE BATCH" + user_store.bran +
                            " SET ISOLDQ = ISOLDQ + {" + Convert.ToString(item_parameter_count) + "} " +
                            ", ISQ" + code2 + " = ISQ" + code2 + " + {" + Convert.ToString(item_parameter_count) + "} " +
                            " WHERE BATCH_SER = {" + Convert.ToString(item_parameter_count + 1) + "};";

                        //item_parameter_count += 2;
                    }                        
                }
                if (itm_string != "")
                    return_value = true;
            }
            catch
            {
                itm_string = "";
            }
            sql_query = itm_string;
            return return_value;
        }
        private bool get_sitm_update_query(ref string sql_query, ref List<object> sql_query_values,float number1, string code2,int number1_serial, int code2_serial)
        {
            bool return_value = false;
            string itm_string = "";
            int item_parameter_count = 0;
            try
            {
                //sql_query_values.Clear();
                foreach (var item in order.items)
                {
                    if (item.sku != "" && item.sku != "0")
                    {
                        item_parameter_count = sql_query_values.Count;
                        //sql_query_values.Add("");
                        //sql_query_values.Add("001");
                        //sql_query_values.Add(code2);
                        //sql_query_values.Add(number1);
                        //sql_query_values.Add(order.date.date);
                        sql_query_values.Add(item.weight);
                        //sql_query_values.Add(0);
                        sql_query_values.Add(item.amounts.total.amount);
                        sql_query_values.Add(item.sku);
                        itm_string += "UPDATE SITM" + user_store.bran +
                            " SET LOC = {9} " +
                            " , INVTYPE = {0}" +
                            " , INVCODE2 = {" + Convert.ToString(code2_serial) + "} " +
                            " , INVNUMBER = {" + Convert.ToString(number1_serial) + "} " +
                            " , INVDATE = {2} " +
                            " , IWEIGHTCASH = IWEIGHTCASH + {" + Convert.ToString(item_parameter_count) + "} " +
                            " , IWEIGHTCREDIT = IWEIGHTCREDIT + {1} " +
                            " , AMOUNT= AMOUNT + {" + Convert.ToString(item_parameter_count + 1) + "} " +
                            " WHERE SER = {" + Convert.ToString(item_parameter_count + 2) + "};";

                        itm_string += "UPDATE SITM_temp" + user_store.bran +
                                " SET LOC = {" + Convert.ToString(code2_serial) + "} " +
                                " , SHOWROOM = {" + Convert.ToString(code2_serial) + "} " +
                                " , INVOICE_NO = {" + Convert.ToString(number1_serial) + "} " +
                                " , INVOICE_DATE = {2} " +
                                " , AMOUNT = {" + Convert.ToString(item_parameter_count + 1) + "} " +
                                " WHERE SER = {" + Convert.ToString(item_parameter_count + 2) + "};";
                        //item_parameter_count += 3;
                    }                    
                }
               
                if (itm_string != "")
                    return_value = true;
            }
            catch
            {
                itm_string = "";
            }
            sql_query = itm_string;
            return return_value;
        }
        public async Task<MessageResponse> savelog(string log, int msg_type)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                Logs new_log = new Logs()
                {
                    msg_date = DateTime.Now,
                    msg = log,
                    msg_type = msg_type
                };
                _maincontext.logs.Add(new_log);
                int saveReturnValue = await _maincontext.SaveChangesAsync();
                if (saveReturnValue > 0)
                {
                    status = true;
                    error_code = 200;
                }
                else
                {
                    error_code = 314;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);
        }

        public async Task<MessageResponse> savemerchantapp(decimal merchant_id, DateTime created_at, string app_name)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                int saveReturnValue = 0;
                SallaMerchantApp new_app = new SallaMerchantApp()
                {
                    merchant_id = merchant_id,
                    created_at = created_at,
                    app_name = app_name
                };
                _maincontext.sallamerchantapp.Add(new_app);
                saveReturnValue = await _maincontext.SaveChangesAsync();
                if (saveReturnValue > 0)
                {
                    status = true;
                    error_code = 200;
                }
                else
                {
                    error_code = 314;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            return new MessageResponse(status, message_laguage, error_code);
        }
        public static TimeZoneInfo OlsonTimeZoneToTimeZoneInfo(string olsonTimeZoneId)
        {
            var olsonWindowsTimes = new Dictionary<string, string>()
            {
        { "Africa/Bangui", "W. Central Africa Standard Time" },
        { "Africa/Cairo", "Egypt Standard Time" },
        { "Africa/Casablanca", "Morocco Standard Time" },
        { "Africa/Harare", "South Africa Standard Time" },
        { "Africa/Johannesburg", "South Africa Standard Time" },
        { "Africa/Lagos", "W. Central Africa Standard Time" },
        { "Africa/Monrovia", "Greenwich Standard Time" },
        { "Africa/Nairobi", "E. Africa Standard Time" },
        { "Africa/Windhoek", "Namibia Standard Time" },
        { "America/Anchorage", "Alaskan Standard Time" },
        { "America/Argentina/San_Juan", "Argentina Standard Time" },
        { "America/Asuncion", "Paraguay Standard Time" },
        { "America/Bahia", "Bahia Standard Time" },
        { "America/Bogota", "SA Pacific Standard Time" },
        { "America/Buenos_Aires", "Argentina Standard Time" },
        { "America/Caracas", "Venezuela Standard Time" },
        { "America/Cayenne", "SA Eastern Standard Time" },
        { "America/Chicago", "Central Standard Time" },
        { "America/Chihuahua", "Mountain Standard Time (Mexico)" },
        { "America/Cuiaba", "Central Brazilian Standard Time" },
        { "America/Denver", "Mountain Standard Time" },
        { "America/Fortaleza", "SA Eastern Standard Time" },
        { "America/Godthab", "Greenland Standard Time" },
        { "America/Guatemala", "Central America Standard Time" },
        { "America/Halifax", "Atlantic Standard Time" },
        { "America/Indianapolis", "US Eastern Standard Time" },
        { "America/Indiana/Indianapolis", "US Eastern Standard Time" },
        { "America/La_Paz", "SA Western Standard Time" },
        { "America/Los_Angeles", "Pacific Standard Time" },
        { "America/Mexico_City", "Mexico Standard Time" },
        { "America/Montevideo", "Montevideo Standard Time" },
        { "America/New_York", "Eastern Standard Time" },
        { "America/Noronha", "UTC-02" },
        { "America/Phoenix", "US Mountain Standard Time" },
        { "America/Regina", "Canada Central Standard Time" },
        { "America/Santa_Isabel", "Pacific Standard Time (Mexico)" },
        { "America/Santiago", "Pacific SA Standard Time" },
        { "America/Sao_Paulo", "E. South America Standard Time" },
        { "America/St_Johns", "Newfoundland Standard Time" },
        { "America/Tijuana", "Pacific Standard Time" },
        { "Antarctica/McMurdo", "New Zealand Standard Time" },
        { "Atlantic/South_Georgia", "UTC-02" },
        { "Asia/Almaty", "Central Asia Standard Time" },
        { "Asia/Amman", "Jordan Standard Time" },
        { "Asia/Baghdad", "Arabic Standard Time" },
        { "Asia/Baku", "Azerbaijan Standard Time" },
        { "Asia/Bangkok", "SE Asia Standard Time" },
        { "Asia/Beirut", "Middle East Standard Time" },
        { "Asia/Calcutta", "India Standard Time" },
        { "Asia/Colombo", "Sri Lanka Standard Time" },
        { "Asia/Damascus", "Syria Standard Time" },
        { "Asia/Dhaka", "Bangladesh Standard Time" },
        { "Asia/Dubai", "Arabian Standard Time" },
        { "Asia/Irkutsk", "North Asia East Standard Time" },
        { "Asia/Jerusalem", "Israel Standard Time" },
        { "Asia/Kabul", "Afghanistan Standard Time" },
        { "Asia/Kamchatka", "Kamchatka Standard Time" },
        { "Asia/Karachi", "Pakistan Standard Time" },
        { "Asia/Katmandu", "Nepal Standard Time" },
        { "Asia/Kolkata", "India Standard Time" },
        { "Asia/Krasnoyarsk", "North Asia Standard Time" },
        { "Asia/Kuala_Lumpur", "Singapore Standard Time" },
        { "Asia/Kuwait", "Arab Standard Time" },
        { "Asia/Magadan", "Magadan Standard Time" },
        { "Asia/Muscat", "Arabian Standard Time" },
        { "Asia/Novosibirsk", "N. Central Asia Standard Time" },
        { "Asia/Oral", "West Asia Standard Time" },
        { "Asia/Rangoon", "Myanmar Standard Time" },
        { "Asia/Riyadh", "Arab Standard Time" },
        { "Asia/Seoul", "Korea Standard Time" },
        { "Asia/Shanghai", "China Standard Time" },
        { "Asia/Singapore", "Singapore Standard Time" },
        { "Asia/Taipei", "Taipei Standard Time" },
        { "Asia/Tashkent", "West Asia Standard Time" },
        { "Asia/Tbilisi", "Georgian Standard Time" },
        { "Asia/Tehran", "Iran Standard Time" },
        { "Asia/Tokyo", "Tokyo Standard Time" },
        { "Asia/Ulaanbaatar", "Ulaanbaatar Standard Time" },
        { "Asia/Vladivostok", "Vladivostok Standard Time" },
        { "Asia/Yakutsk", "Yakutsk Standard Time" },
        { "Asia/Yekaterinburg", "Ekaterinburg Standard Time" },
        { "Asia/Yerevan", "Armenian Standard Time" },
        { "Atlantic/Azores", "Azores Standard Time" },
        { "Atlantic/Cape_Verde", "Cape Verde Standard Time" },
        { "Atlantic/Reykjavik", "Greenwich Standard Time" },
        { "Australia/Adelaide", "Cen. Australia Standard Time" },
        { "Australia/Brisbane", "E. Australia Standard Time" },
        { "Australia/Darwin", "AUS Central Standard Time" },
        { "Australia/Hobart", "Tasmania Standard Time" },
        { "Australia/Perth", "W. Australia Standard Time" },
        { "Australia/Sydney", "AUS Eastern Standard Time" },
        { "Etc/GMT", "UTC" },
        { "Etc/GMT+11", "UTC-11" },
        { "Etc/GMT+12", "Dateline Standard Time" },
        { "Etc/GMT+2", "UTC-02" },
        { "Etc/GMT-12", "UTC+12" },
        { "Europe/Amsterdam", "W. Europe Standard Time" },
        { "Europe/Athens", "GTB Standard Time" },
        { "Europe/Belgrade", "Central Europe Standard Time" },
        { "Europe/Berlin", "W. Europe Standard Time" },
        { "Europe/Brussels", "Romance Standard Time" },
        { "Europe/Budapest", "Central Europe Standard Time" },
        { "Europe/Dublin", "GMT Standard Time" },
        { "Europe/Helsinki", "FLE Standard Time" },
        { "Europe/Istanbul", "GTB Standard Time" },
        { "Europe/Kiev", "FLE Standard Time" },
        { "Europe/London", "GMT Standard Time" },
        { "Europe/Minsk", "E. Europe Standard Time" },
        { "Europe/Moscow", "Russian Standard Time" },
        { "Europe/Paris", "Romance Standard Time" },
        { "Europe/Sarajevo", "Central European Standard Time" },
        { "Europe/Warsaw", "Central European Standard Time" },
        { "Indian/Mauritius", "Mauritius Standard Time" },
        { "Pacific/Apia", "Samoa Standard Time" },
        { "Pacific/Auckland", "New Zealand Standard Time" },
        { "Pacific/Fiji", "Fiji Standard Time" },
        { "Pacific/Guadalcanal", "Central Pacific Standard Time" },
        { "Pacific/Guam", "West Pacific Standard Time" },
        { "Pacific/Honolulu", "Hawaiian Standard Time" },
        { "Pacific/Pago_Pago", "UTC-11" },
        { "Pacific/Port_Moresby", "West Pacific Standard Time" },
        { "Pacific/Tongatapu", "Tonga Standard Time" }
            };

            var windowsTimeZoneId = default(string);
            var windowsTimeZone = default(TimeZoneInfo);
            if (olsonWindowsTimes.TryGetValue(olsonTimeZoneId, out windowsTimeZoneId))
            {
                try { windowsTimeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId); }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }
            return windowsTimeZone;
        }
    }
}
