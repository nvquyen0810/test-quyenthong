using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.DTOs
{
    public class ProductUpdateDTO
    {
        public int ProductID { get; set; }
        public int UnitsInStockMinus { get; set; }
    }
}
