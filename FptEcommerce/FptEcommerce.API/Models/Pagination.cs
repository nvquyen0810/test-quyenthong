using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FptEcommerce.API.Models
{
    public class Pagination<T>
    {
        public int Total { get; set; }
        public List<T> Items { get; set; }
    }
}
