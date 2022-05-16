using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FptEcommerce.API.Models
{
    public class Response
    {
        public bool Success { get; set; }   // true/false
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
