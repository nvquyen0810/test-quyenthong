using Dapper;
using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Entities;
using FptEcommerce.Core.Helper;
using FptEcommerce.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;
        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CustomerInfoDTO> GetUserByUsernameAndPassword(CutomerLoginDTO userLogin)
        {
            var username = userLogin.Username;
            var password = userLogin.Password.ToMD5();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT CustomerId, Username, Fullname, Email, Phone
                                FROM Customers 
                                 WHERE Username = @username AND Password = @password";
                //string query = @"EXEC Test";

                var result = await connection.QueryFirstOrDefaultAsync<CustomerInfoDTO>(query, new { username, password });
                return result;
            }
        }
    }
}
