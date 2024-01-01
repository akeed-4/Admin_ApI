using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    public class Users
    {
        public string company_id { get; set; }
        [Key]
        public int? user_id { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string user_image { get; set; }
        public string user_address { get; set; }
        public DateTime? activation_date { get; set; }
        public bool? logged { get; set; }
        public DateTime? logged_date { get; set; }
        public bool? is_admin { get; set; }
        public int? user_online_type { get; set; }
        [ForeignKey("company_id")]
        public Companies company { get; set; }

     
    }
    public class SallaUsers
    {
        public string company_id { get; set; }
        [Key]
        public int? user_id { get; set; }
        public string user_name { get; set; }
        public DateTime? activation_date { get; set; }
        public int? user_online_type { get; set; }
        [ForeignKey("company_id")]
        public Companies company { get; set; }
    }
    //public class OnlineUsers
    //{
    //    [Key]
    //    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    //    public int id { get; set; }
    //    public int user_id { get; set; }
    //    public string invoice_type { get; set; }
    //    public double user_last_serial { get; set; }       
    //}

    //public class OnlineUsersObject
    //{
    //    public string invoice_type { get; set; }
    //    public double user_last_serial { get; set; }
    //}
    //public class OnlineUsersRespose
    //{
    //    public List<OnlineUsersObject> list_of_serials { get; set; }
    //    public bool status { get; set; }
    //    public Languages message { get; set; }
    //    public int error_code { get; set; }
    //    public OnlineUsersRespose(List<OnlineUsersObject> listOfSerials, bool response_status, Languages response_message, int response_error_code)
    //    {
    //        list_of_serials = listOfSerials;
    //        status = response_status;
    //        message = response_message;
    //        error_code = response_error_code;
    //    }
    //}
    public class AuthenticateUser
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_image { get; set; }
        public string user_address { get; set; }
        public List<SERIALS> user_invoices_serials { get; set; }
        public List<MobilePermissions> user_permissions { get; set; }
        public AuthenticateUser(Users user = null)
        {
            if (user != null)
            {
                user_id = user.user_id ?? 0;
                user_name = user.user_name;
                user_image = user.user_image;
                user_address = user.user_address;
            }
            else
            {
                user_id = 0;
                user_name = null;
                user_image = null;
                user_address = null;
            }
        }
    }
    public class UsersConnection
    {
        public Users user_data { get; set; }
        public string connection { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public UsersConnection(int response_user_id,string response_user_name, string response_user_image, string response_user_address, int response_user_online_type, int response_is_own_database, string response_connection, bool response_status, Languages response_message, int response_error_code)
        {
            user_data = new Users()
            {
                user_id = response_user_id,
                user_name = response_user_name,
                user_image = response_user_image,
                user_address = response_user_address,
                user_online_type = response_user_online_type,
                company = new Companies() { is_own_database = response_is_own_database }
            };
            connection = response_connection;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
    public class UsersData
    {
        public string access_token { get; set; }
        public AuthenticateUser user_data { get; set; }
        public string company_name { get; set; }
        public string company_tax_id { get; set; }
        public string company_address { get; set; }
        public string company_phone { get; set; }
        public string company_fax { get; set; }
        public string company_segal { get; set; }
        public string company_country { get; set; }
        public string company_city { get; set; }
        public string company_state { get; set; }
        public string company_postcode { get; set; }
        public string company_building { get; set; }
        public string company_street { get; set; }
    }
    public class UsersResponse
    {
        public UsersData data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public UsersResponse(AuthenticateUser user, string token, string company_name, string company_tax_id, string company_address, string company_phone, string company_fax, string company_segal, string company_country, string company_city, string company_state, string company_postcode, string company_building, string company_street, bool response_status, Languages response_message,int response_error_code)
        {
            UsersData userData = new UsersData()
            {
                access_token = token,
                user_data = user,
                company_name = company_name,
                company_address = company_address,
                company_tax_id = company_tax_id,
                company_phone = company_phone,
                company_fax = company_fax,
                company_segal = company_segal,
                company_country = company_country,
                company_city = company_city,
                company_state = company_state,
                company_postcode = company_postcode,
                company_building = company_building,
                company_street = company_street
            };
            data = userData;       
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
