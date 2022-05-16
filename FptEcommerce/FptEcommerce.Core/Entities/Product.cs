using System;
using System.Collections.Generic;

#nullable disable

namespace FptEcommerce.Core.Entities
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumb { get; set; }
        public int? Price { get; set; }
        public int? UnitsInStock { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
