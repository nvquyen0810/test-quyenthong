using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.DTOs
{
    public class ProductInfoDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumb { get; set; }
        public int? Price { get; set; }
    }
}
