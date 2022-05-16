using System;
using System.Collections.Generic;

#nullable disable

namespace FptEcommerce.Core.Entities
{
    public partial class Address
    {
        public Address()
        {
            Orders = new HashSet<Order>();
        }

        public int AddressId { get; set; }
        public string Location { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
