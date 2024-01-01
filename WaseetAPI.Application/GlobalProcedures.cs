using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using WaseetAPI.Domain.Models;

namespace WaseetAPI.Application
{
    public class GlobalProcedures
    {
        public void getUserInfo(string currentUserRequest, ref int userOnlineType, ref int userId, ref int is_own_database, ref string connectionStr)
        {
            string local_currentUserRequest = currentUserRequest;
            userOnlineType = Convert.ToInt32(local_currentUserRequest.Substring(0, local_currentUserRequest.IndexOf(";")));
            local_currentUserRequest = local_currentUserRequest.Substring(local_currentUserRequest.IndexOf(";") + 1);
            if (local_currentUserRequest.IndexOf(";") > 0)
            {
                userId = Convert.ToInt32(local_currentUserRequest.Substring(0, local_currentUserRequest.IndexOf(";")));

                local_currentUserRequest = local_currentUserRequest.Substring(local_currentUserRequest.IndexOf(";") + 1);
                if (local_currentUserRequest.IndexOf(";") > 0)
                {
                    is_own_database = Convert.ToInt32(local_currentUserRequest.Substring(0, local_currentUserRequest.IndexOf(";")));
                    connectionStr = local_currentUserRequest.Substring(local_currentUserRequest.IndexOf(";") + 1);
                }
                else
                {
                    is_own_database = 1;
                    connectionStr = local_currentUserRequest;
                }


                //connectionStr = local_currentUserRequest.Substring(local_currentUserRequest.IndexOf(";") + 1);
            }
            else
            {
                userId = userOnlineType;
                userOnlineType = 1;
                is_own_database = 1;
                connectionStr = local_currentUserRequest;
            }
        }
        public Languages GetMessageLanguageFromCode(int error_code, string error_message = "")
        {
            string ar_message = "";
            string en_message = "";
            switch (error_code)
            {
                case 200:
                    ar_message = "تمت العملية بنجاح"; en_message = "success"; break;
                case 301:
                    ar_message = "العميل غير موجود"; en_message = "Customer Not Exists"; break;
                case 302:
                    ar_message = "لم تتم عملية الحفظ لوجود عمليات  "; en_message = "Not Saved"; break;
                case 303:
                    ar_message = "نوع الفاتورة غير مسموح به"; en_message = "Invoice Type Not Allowed"; break;
                case 304:
                    ar_message = "لا يمكن اجراء العملية على الفاتورة الحالية : الفاتورة لم تعتمد بعد"; en_message = "Action Not Allowed to This Invoice : Invoice Not Approved Yet"; break;
                case 305:
                    ar_message = "لا يمكن اجراء العملية على الفاتورة الحالية : الفاتورة تم حذفها مسبقا"; en_message = "Action Not Allowed to This Invoice : Invoice Already Deleted"; break;
                case 306:
                    ar_message = "الفاتورة غير موجودة"; en_message = "Invoice Not Exists"; break;
                case 307:
                    ar_message = "لا يمكن اجراء العملية على استلام البضاعة الحالي : تم رفض الاستلام مسبقا"; en_message = "Action Not Allowed to This Receive Products : Receive Products Already Canceled"; break;
                case 308:
                    ar_message = "لا يمكن اجراء العملية على استلام البضاعة الحالي : تم قبول الاستلام مسبقا"; en_message = "Action Not Allowed to This Receive Products : Receive Products Already Accepted"; break;
                case 309:
                    ar_message = "استلام البضاعة غير موجود"; en_message = "Receive Products Not Exists"; break;
                case 310:
                    ar_message = "رقم استلام البضاعة غير صحيح"; en_message = "Invalid Receive Products ID"; break;
                case 311:
                    ar_message = "المنتج غير موجود او لا يوجد له رصيد كافي"; en_message = "Product Not Exists or There Is NOT Enough Quantity"; break;
                case 312:
                    ar_message = "لا يمكن اجراء العملية على الفاتورة الحالية : الفاتورة تم اعتمادها مسبقا"; en_message = "Action Not Allowed to This Invoice : Invoice Already Approved"; break;
                case 313:
                    ar_message = "رقم الفاتورة غير صحيح"; en_message = "Invalid Invoice ID"; break;
                case 314:
                    ar_message = "حدث خطأ اثناء عملية تسجيل الدخول"; en_message = "An Error Occurred During Login"; break;
                case 315:
                    ar_message = "خادم المستخدم غير متوفر"; en_message = "User Server not Available"; break;
                case 316:
                    ar_message = "المستخدم مسجل دخول مسبقا"; en_message = "User is Already Logged in"; break;
                case 317:
                    ar_message = "انتهى اشتراك المستخدم"; en_message = "User Subscription has Expired"; break;
                case 318:
                    ar_message = "المستخدم غير موجود"; en_message = "User not Available"; break;
                case 319:
                    ar_message = "حدث خطأ اثناء عملية تسجيل الخروج"; en_message = "An Error Occurred During Logout"; break;
                case 320:
                    ar_message = "البيانات غير صحيحة : لا يوجد مبالغ للدفع"; en_message = "Not Valid Data : No Existing Invoices Amount to Pay"; break;
                case 321:
                    ar_message = "لا يمكن انشاء سند واحد لأكثر من عميل"; en_message = "Cannot Create Receipt for Different Customers"; break;
                case 322:
                    ar_message = "البيانات غير صحيحة : لا يوجد مبالغ غير مدفوعة"; en_message = "Not Valid Data : No Existing Invoices Amount to Pay"; break;
                case 323:
                    ar_message = "لا يمكن اجراء العملية على السند الحالي : السند تم اعتماده مسبقا"; en_message = "Action Not Allowed to This Receipt : Receipt Already Approved"; break;
                case 324:
                    ar_message = "لا يمكن اجراء العملية على السند الحالي : السند تم حذفه مسبقا"; en_message = "Action Not Allowed to This Receipt : Receipt Already Deleted"; break;
                case 325:
                    ar_message = "السند غير موجود"; en_message = "Receipt Not Exists"; break;
                case 326:
                    ar_message = "رقم السند غير صحيح"; en_message = "Invalid Receipt ID"; break;
                case 327:
                    ar_message = "Token غير صالح"; en_message = "Invalid Token"; break;
                case 328:
                    ar_message = "بيانات الشركة غير صحيحة"; en_message = "Company Data Not Correct"; break;
                case 329:
                    ar_message = "خطأ في التسلسل"; en_message = "Error In Serial"; break;
                case 330:
                    ar_message = ""; en_message = ""; break;
                case 331:
                    ar_message = ""; en_message = ""; break;
                case 332:
                    ar_message = ""; en_message = ""; break;
                case 333:
                    ar_message = ""; en_message = ""; break;
                case 334:
                    ar_message = ""; en_message = ""; break;
                case 335:
                    ar_message = ""; en_message = ""; break;
                case 336:
                    ar_message = ""; en_message = ""; break;
                case 337:
                    ar_message = ""; en_message = ""; break;
                case 338:
                    ar_message = ""; en_message = ""; break;
                case 339:
                    ar_message = ""; en_message = ""; break;
                case 340:
                    ar_message = ""; en_message = ""; break;
                case 341:
                    ar_message = ""; en_message = ""; break;
                case 342:
                    ar_message = ""; en_message = ""; break;
                case 343:
                    ar_message = ""; en_message = ""; break;
                case 344:
                    ar_message = ""; en_message = ""; break;
                case 345:
                    ar_message = ""; en_message = ""; break;
                case 346:
                    ar_message = ""; en_message = ""; break;
                case 347:
                    ar_message = ""; en_message = ""; break;
                case 348:
                    ar_message = ""; en_message = ""; break;
                case 349:
                    ar_message = ""; en_message = ""; break;
                case 350:
                    ar_message = ""; en_message = ""; break;
                case 351:
                    ar_message = ""; en_message = ""; break;
                case 352:
                    ar_message = ""; en_message = ""; break;
                case 353:
                    ar_message = ""; en_message = ""; break;
                case 354:
                    ar_message = ""; en_message = ""; break;
                case 355:
                    ar_message = ""; en_message = ""; break;
                case 356:
                    ar_message = ""; en_message = ""; break;
                case 357:
                    ar_message = ""; en_message = ""; break;
                case 358:
                    ar_message = ""; en_message = ""; break;
                case 359:
                    ar_message = ""; en_message = ""; break;
                case 360:
                    ar_message = ""; en_message = ""; break;
                case 361:
                    ar_message = ""; en_message = ""; break;
                case 362:
                    ar_message = ""; en_message = ""; break;
                case 363:
                    ar_message = ""; en_message = ""; break;
                case 364:
                    ar_message = ""; en_message = ""; break;
                case 365:
                    ar_message = ""; en_message = ""; break;
                case 366:
                    ar_message = ""; en_message = ""; break;
                case 367:
                    ar_message = ""; en_message = ""; break;
                case 368:
                    ar_message = ""; en_message = ""; break;
                case 369:
                    ar_message = ""; en_message = ""; break;
                case 370:
                    ar_message = ""; en_message = ""; break;
                case 371:
                    ar_message = ""; en_message = ""; break;
                case 372:
                    ar_message = ""; en_message = ""; break;
                case 373:
                    ar_message = ""; en_message = ""; break;
                case 374:
                    ar_message = ""; en_message = ""; break;
                case 375:
                    ar_message = ""; en_message = ""; break;
                case 376:
                    ar_message = ""; en_message = ""; break;
                case 377:
                    ar_message = ""; en_message = ""; break;
                case 378:
                    ar_message = ""; en_message = ""; break;
                case 379:
                    ar_message = ""; en_message = ""; break;
                case 380:
                    ar_message = ""; en_message = ""; break;
                case 381:
                    ar_message = ""; en_message = ""; break;
                case 382:
                    ar_message = ""; en_message = ""; break;
                case 383:
                    ar_message = ""; en_message = ""; break;
                case 384:
                    ar_message = ""; en_message = ""; break;
                case 385:
                    ar_message = ""; en_message = ""; break;
                case 386:
                    ar_message = ""; en_message = ""; break;
                case 387:
                    ar_message = ""; en_message = ""; break;
                case 388:
                    ar_message = ""; en_message = ""; break;
                case 389:
                    ar_message = ""; en_message = ""; break;
                case 390:
                    ar_message = ""; en_message = ""; break;
                case 391:
                    ar_message = ""; en_message = ""; break;
                case 392:
                    ar_message = ""; en_message = ""; break;
                case 393:
                    ar_message = ""; en_message = ""; break;
                case 394:
                    ar_message = ""; en_message = ""; break;
                case 395:
                    ar_message = ""; en_message = ""; break;
                case 396:
                    ar_message = ""; en_message = ""; break;
                case 397:
                    ar_message = ""; en_message = ""; break;
                case 398:
                    ar_message = ""; en_message = ""; break;
                case 399:
                    ar_message = ""; en_message = ""; break;
                case 400:
                    ar_message = error_message; en_message = error_message; break;
            }
            return new Languages()
            {
                Ar = ar_message,
                En = en_message
            };
        }
        public DateTime CovertTimeZone(DateTime currentDateTime)
        {
            // Get Standard Time zone
            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
            DateTime tstTime = DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(currentDateTime, TimeZoneInfo.Utc, tst), DateTimeKind.Utc);
            //DateTime tstTime_convert_again = TimeZoneInfo.ConvertTimeToUtc(tstTime, tst);
            return tstTime;
        }
        
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
