using Dapper;
using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Entities;
using FptEcommerce.Core.Helper;
using FptEcommerce.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
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
                // Cách 1
                //string query = @"SELECT CustomerId, Username, Fullname, Email, Phone
                //                FROM Customers 
                //                 WHERE Username = @username AND Password = @password";
                //string query = @"EXEC Test";

                //var result = await connection.QueryFirstOrDefaultAsync<CustomerInfoDTO>(query, new { username, password });

                // Cách 2
                string query = "EXEC Proc_GetUserByUsernameAndPasword @username, @password";
                var result = await connection.QueryFirstOrDefaultAsync<CustomerInfoDTO>(
                    query,
                    new { username, password });

                // Cách 3
                //string query = "Proc_GetUserByUsernameAndPasword";
                //var result = await connection.QueryFirstOrDefaultAsync<CustomerInfoDTO>(
                //    query,
                //    new { username, password },
                //commandType: System.Data.CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<int> TestCreateReturnId(CustomerInfoUpdateDTO updateDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"INSERT INTO dbo.Customers(Username, Password, FullName, Email, Phone)
                                OUTPUT Inserted.CustomerID
                                values (@username, @password, @fullname, @email, @phone)"; // SQL query that we want to execute with dapper

                string sqlQuery1 = @"Proc_CreateCustomer";

                var parameters = new DynamicParameters();
                parameters.Add("username", updateDTO.Email, DbType.String);
                parameters.Add("password", "e10adc3949ba59abbe56e057f20f883e", DbType.String);
                parameters.Add("fullname", updateDTO.FullName, DbType.String);
                parameters.Add("email", updateDTO.Email, DbType.String);
                parameters.Add("phone", updateDTO.Phone, DbType.String);

                //int result = await connection.QuerySingleAsync<int>(sqlQuery, parameters);
                int result = await connection.QuerySingleAsync<int>(sqlQuery1, parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<int> UpdateCustomerInfo(int customerId, CustomerInfoUpdateDTO userUpdate)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //string q = @"UPDATE dbo.Customers
                //         SET FullName = @fullname, Email = @email, Phone = @phone
                //         WHERE CustomerID = @id";   // không nên đếm count(*)

                //var result = await connection.ExecuteAsync(q,
                //               new
                //               {
                //                   @id = customerId,
                //                   @fullname = userUpdate.FullName,
                //                   @email = userUpdate.Email,
                //                   @phone = userUpdate.Phone
                //               });

                string q = "Proc_UpdateCustomerInfo";
                var result = await connection.ExecuteAsync(q,
                    new
                    {
                        @id = customerId,
                        @fullname = userUpdate.FullName,
                        @email = userUpdate.Email,
                        @phone = userUpdate.Phone
                    },
                    commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}
