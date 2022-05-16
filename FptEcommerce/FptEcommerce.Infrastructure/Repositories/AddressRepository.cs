using FptEcommerce.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly string _connectionString;
        public AddressRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
