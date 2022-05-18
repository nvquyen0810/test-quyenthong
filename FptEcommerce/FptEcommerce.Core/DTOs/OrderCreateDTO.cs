using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.DTOs
{
    public class OrderCreateDTO
    {
        public int TotalMoney { get; set; }
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public List<CartDTO> Cart { get; set; }
    }
}
