using System;
using System.Collections.Generic;

#nullable disable

namespace FptEcommerce.Core.Entities
{
    public partial class HistoryEmail
    {
        public int HistoryEmailId { get; set; }
        public int? OrderId { get; set; }

        public virtual Order Order { get; set; }
    }
}
