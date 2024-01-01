using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    [DataContract]
    public class VOU_SETTING
    {
        [Key]
        [Column(Order = 1)]
        [DataMember]
        public string FTYPE { get; set; }
        [Key]
        [Column(Order = 2)]
        public Int16? FTYPE2 { get; set; }
        public string NAME1 { get; set; }
        public string NAME2 { get; set; }
        public string SHORT_NAME1 { get; set; }
        public string SHORT_NAME2 { get; set; }
        public string SMALL_PRINTER_TITLEBTC { get; set; }
        public string SMALL_PRINTER_TITLEBTC2 { get; set; }
        public string SMALL_PRINTER_TITLEBTB { get; set; }
        public string SMALL_PRINTER_TITLEBTB2 { get; set; }
        [DataMember]
        public Languages NAME
        {
            get
            {
                return
                    new Languages()
                    {
                        Ar = NAME1,
                        En = NAME2
                    };
            }
        }
        [DataMember]
        public Languages SHORT_NAME
        {
            get
            {
                return
                    new Languages()
                    {
                        Ar = SHORT_NAME1,
                        En = SHORT_NAME2
                    };
            }
        }
        [DataMember]
        public Languages BTC_TITLE
        {
            get
            {
                return
                    new Languages()
                    {
                        Ar = SMALL_PRINTER_TITLEBTC,
                        En = SMALL_PRINTER_TITLEBTC2
                    };
            }
        }
        [DataMember]
        public Languages BTB_TITLE
        {
            get
            {
                return
                    new Languages()
                    {
                        Ar = SMALL_PRINTER_TITLEBTB,
                        En = SMALL_PRINTER_TITLEBTB2
                    };
            }
        }
    }
    //public class VOU_SETTINGObject
    //{
    //    public string FTYPE { get; set; }
    //    public Languages NAME { get; set; }
    //    public Languages SHORT_NAME { get; set; }
    //    public Languages BTC_TITLE { get; set; }
    //    public Languages BTB_TITLE { get; set; }
    //}
    public class VOU_SETTINGResponse
    {
        public VOU_SETTING data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public VOU_SETTINGResponse(VOU_SETTING vou_SettingObject, bool response_status, Languages response_message, int response_error_code)
        {
            data = vou_SettingObject;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
    public class VOU_SETTINGListResponse
    {
        public List<VOU_SETTING> data { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public VOU_SETTINGListResponse(List<VOU_SETTING> listOfVOU_SETTING, bool response_status, Languages response_message, int response_error_code)
        {
            data = listOfVOU_SETTING;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
