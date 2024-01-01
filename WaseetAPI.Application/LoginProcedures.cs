using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WaseetAPI.Database;
using WaseetAPI.Domain.Models;

namespace WaseetAPI.Application
{
    public class LoginProcedures
    {
        private MainDbContext _context;
        private GlobalProcedures global;
        public LoginProcedures(MainDbContext context)
        {
            _context = context;
            global = new GlobalProcedures();
        } 
        public async Task<UsersConnection> CheckAvailableUser(Admin user)
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
                string encrpassword = encrypt(user.password);
                
                Admin checkUser = await _context.admin
                    .Where(x => x.user_name == user.user_name && x.password == encrpassword)
                    .Include(x => x.company)
                    .FirstOrDefaultAsync();                
                if (checkUser != null)
                {
                    if ((checkUser.is_admin ?? false) || ((checkUser.activation_date ?? DateTime.Now.AddDays(-1)).Date >= DateTime.Now.Date))
                    {
                        if ((checkUser.is_admin ?? false) || !(checkUser.logged ?? false) || ((checkUser.logged_date ?? DateTime.Now.AddDays(-1)).AddDays(1) < DateTime.Now))
                        {
                            if (checkUser.company.server_name != null && checkUser.company.server_name != ""
                                && checkUser.company.db_name != null && checkUser.company.db_name != ""
                                && checkUser.company.server_user_name != null && checkUser.company.server_user_name != ""
                                && checkUser.company.server_password != null && checkUser.company.server_password != "")
                            {
                                user_id = checkUser.user_id ?? 0;
                                user_name = checkUser.user_name;
                                user_image = checkUser.user_image;
                                user_address = checkUser.user_address;
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
                            error_code = 316;
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
            return new UsersConnection(user_id, user_name, user_image, user_address, user_online_type, is_own_database, connectionStr,status, message_laguage, error_code);
        }
        public async Task<MessageResponse> SetUserLogin(int user_id)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                Users checkUser = await _context.users
                    .Where(x => x.user_id == user_id)
                    .FirstOrDefaultAsync();
                if (checkUser != null)
                {
                    checkUser.logged = true;
                    checkUser.logged_date = DateTime.Now;
                    int saveReturnValue = await _context.SaveChangesAsync();
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
            return new MessageResponse(status, message_laguage, error_code);
        }
        
        public async Task<MessageResponse> LogoutUser(int user_id)
        {
            bool status = false;
            string message = "error";
            int error_code = 400;
            try
            {
                Users checkUser = await _context.users
                    .Where(x => x.user_id == user_id)
                    .FirstOrDefaultAsync();
                if (checkUser != null)
                {
                    checkUser.logged = false;
                    int saveReturnValue = await _context.SaveChangesAsync();
                    if (saveReturnValue > 0)
                    {
                        status = true;
                        error_code = 200;
                    }
                    else
                    {
                        error_code = 319;
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
            return new MessageResponse(status, message_laguage, error_code);
        }
        private string encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }        
    }
}
