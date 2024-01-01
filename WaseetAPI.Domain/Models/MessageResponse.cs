using System;
using System.Collections.Generic;
using System.Text;

namespace WaseetAPI.Domain.Models
{
    public class MessageResponse
    {
        public bool status { get; set; }
        public Languages message { get; set; }
        public int error_code { get; set; }
        public MessageResponse(bool response_status, Languages response_message, int response_error_code)
        {
            status = response_status;
            message = response_message;
            error_code = response_error_code;
        }
    }
}
