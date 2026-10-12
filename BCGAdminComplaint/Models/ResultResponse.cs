using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCGAdminComplaint.Models
{
    public class ResultResponse
    {
        public object Data { get; set; }
        public object Data2 { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}