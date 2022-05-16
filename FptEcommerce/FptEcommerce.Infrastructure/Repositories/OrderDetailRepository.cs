using FptEcommerce.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Infrastructure.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly string _connectionString;
        public OrderDetailRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
