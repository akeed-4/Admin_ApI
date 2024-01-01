using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaseetAPI.Application;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;

namespace WaseetAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class loginController : ControllerBase
    {
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly MainDbContext _ctx;
        private GlobalProcedures global;
        public loginController(IJwtAuthenticationManager jwtAuthenticationManager, MainDbContext ctx)
        {
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _ctx = ctx;
            global = new GlobalProcedures();
        }

        public async Task<UsersResponse>  Get(Admin user)
        {
            string token = "";
            string company_name = null;
            string company_tax_id = null;
            string company_address = null;
            string company_phone = null;
            string company_fax = null;
            string company_segal = null;
            string company_country = null;
            string company_city = null;
            string company_state = null;
            string company_postcode = null;
            string company_building = null; 
            string company_street = null;
            bool status = false;
            string message = "error";
            int error_code = 400;
            Languages message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            AuthenticateUser authkUser = new AuthenticateUser();
            try
            {
                UsersConnection checkUser = await new LoginProcedures(_ctx).CheckAvailableUser(user);
                //chexkUser = _ctx.users.Where(x => x.company_id == user.company_id && x.user_name == user.user_name && x.password == user.password).FirstOrDefault();

                if (checkUser.status)
                {
                    authkUser.user_id = checkUser.user_data.user_id ?? 0;
                    authkUser.user_name = checkUser.user_data.user_name;
                    authkUser.user_image = checkUser.user_data.user_image;
                    authkUser.user_address = checkUser.user_data.user_address;
                    SERIALSRespose last_serial = await new InvoiceProcedures(checkUser.connection).GetInvoicesSerial(checkUser.user_data.user_id??0);
                    MobilePermissionsRespose user_permissions = await new InvoiceProcedures(checkUser.connection).GetUserPermissions(checkUser.user_data.user_id ?? 0);
                    WSHOWResponse company_info = await new InvoiceProcedures(checkUser.connection).GetCompanyInfo(checkUser.user_data.user_id??0);
                    if (company_info.status)
                    {
                        company_name = company_info.data.name;
                        company_tax_id = company_info.data.tax_id;
                        company_address = company_info.data.address;
                        company_phone = company_info.data.tel;
                        company_fax = company_info.data.fax;
                        company_segal = company_info.data.DSEGAL;
                        company_country = company_info.data.CANTRY;
                        company_city = company_info.data.CITY;
                        company_state = company_info.data.LOCALAREA;
                        company_postcode = company_info.data.DBOXID;
                        company_building = company_info.data.DBULDINGID;
                        company_street = company_info.data.INFO1;
                        if (last_serial.status)
                            authkUser.user_invoices_serials = last_serial.list_of_serials;
                        else
                        {
                            authkUser.user_invoices_serials = null;
                            //serial_message = " | Get User Serials Error : error_code="+ last_serial.error_code + " message=" + last_serial.message;
                        }
                        if (user_permissions.status)
                            authkUser.user_permissions = user_permissions.list_of_permissions;
                        else
                            authkUser.user_permissions = null;
                        string tokenValue = Convert.ToString(checkUser.user_data.user_online_type) + ";" + Convert.ToString(checkUser.user_data.user_id) + ";"+ Convert.ToString(checkUser.user_data.company.is_own_database) + ";" + checkUser.connection;
                        token = _jwtAuthenticationManager.Authenticate(tokenValue);
                        if (token != null)
                        {
                            MessageResponse isLogin = await new LoginProcedures(_ctx).SetUserLogin(checkUser.user_data.user_id ?? 0);
                            if (isLogin.status)
                            {
                                error_code = 200;
                                message_laguage = global.GetMessageLanguageFromCode(error_code);
                                status = true;
                            }
                            else
                            {
                                message_laguage = isLogin.message;
                                error_code = isLogin.error_code;
                            }
                        }
                        else
                        {
                            error_code = 327;
                            message_laguage = global.GetMessageLanguageFromCode(error_code);
                        }
                    }
                    else
                    {
                        error_code = 328;
                        message_laguage = global.GetMessageLanguageFromCode(error_code);
                    }
                }
                else
                {
                    message_laguage = checkUser.message;
                    error_code = checkUser.error_code;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                message_laguage = global.GetMessageLanguageFromCode(error_code, message);
            }
            return new UsersResponse(authkUser, token, company_name, company_tax_id, company_address, company_phone, company_fax, company_segal, company_country, company_city, company_state, company_postcode, company_building, company_street, status, message_laguage, error_code);
        }                  
    }
}
