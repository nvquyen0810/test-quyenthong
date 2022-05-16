using System;
using System.Collections.Generic;

#nullable disable

namespace FptEcommerce.Core.Entities
{
    public partial class HistoryPdf
    {
        public int HistoryPdfId { get; set; }
        public int? OrderId { get; set; }

        public virtual Order Order { get; set; }
    }
}
