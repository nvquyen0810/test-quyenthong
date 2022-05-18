using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FptEcommerce.API.Models
{
    public class KafkaMessage
    {
        public int OrderId { get; set; }
        public string Token { get; set; }
    }
}
