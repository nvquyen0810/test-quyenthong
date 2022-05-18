using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.DTOs
{
    public class SearchDTO
    {
        public string Search { get; set; }
        public int perPage { get; set; }
        public int currentPage { get; set; }
    }
}
