using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.DTOs
{
    public class OrderInfoDTO
    {
        public int OrderId { get; set; }
        public DateTime? OrderDatetime { get; set; }
        public int? TotalMoney { get; set; }
        public int? CustomerId { get; set; }
        public int? AddressId { get; set; }
    }
}
