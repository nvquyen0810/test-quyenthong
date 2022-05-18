using Dapper;
using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Entities;
using FptEcommerce.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public async Task<List<AddressDTO>> getAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //string query = @"SELECT TOP 4*from dbo.Products ORDER BY ProductID DESC";
                string query = @"SELECT AddressID, Location
                                 FROM dbo.Addresses";
                var result = await connection.QueryAsync<AddressDTO>(query);

                return result.ToList();
            }
        }
    }
}
