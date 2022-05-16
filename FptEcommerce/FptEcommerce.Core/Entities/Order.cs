using System;
using System.Collections.Generic;

#nullable disable

namespace FptEcommerce.Core.Entities
{
    public partial class Order
    {
        public Order()
        {
            HistoryEmails = new HashSet<HistoryEmail>();
            HistoryPdfs = new HashSet<HistoryPdf>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime? OrderDatetime { get; set; }
        public int? TotalMoney { get; set; }
        public int? CustomerId { get; set; }
        public int? AddressId { get; set; }

        public virtual Address Address { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<HistoryEmail> HistoryEmails { get; set; }
        public virtual ICollection<HistoryPdf> HistoryPdfs { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
