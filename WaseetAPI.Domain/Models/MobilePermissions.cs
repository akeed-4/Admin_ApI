using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    [DataContract]
    public class MobilePermissions
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public int? id { get; set; }
        public int? user_id { get; set; }
        [DataMember]
        public string permission_name { get; set; }
        [DataMember]
        public bool permission_value { get; set; }
    }
    public class MobilePermissionsRespose
    {
        public List<MobilePermissions> list_of_permissions { get; set; }
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public MobilePermissionsRespose(List<MobilePermissions> listOfPermissions, bool response_status, Languages response_message, int response_error_code)
        {
            list_of_permissions = listOfPermissions;
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
