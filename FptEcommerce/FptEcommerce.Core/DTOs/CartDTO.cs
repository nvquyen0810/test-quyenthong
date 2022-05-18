using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.DTOs
{
    public class CartDTO
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int TotalMoney { get; set; }
    }
}
