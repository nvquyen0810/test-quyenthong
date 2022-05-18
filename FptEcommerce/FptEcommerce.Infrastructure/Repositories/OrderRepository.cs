using Dapper;
using FptEcommerce.Core.DTOs;
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
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateOrder(OrderCreateDTO orderCreateDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"INSERT into Orders (OrderDatetime, TotalMoney, CustomerID, AddressID)
                                    OUTPUT Inserted.OrderID
                                values (@time, @money, @customerId, @addressId)"; // SQL query that we want to execute with dapper
                var parameters = new DynamicParameters();
                parameters.Add("time", DateTime.Now, DbType.DateTime2);
                parameters.Add("money", orderCreateDTO.TotalMoney, DbType.Int32);
                parameters.Add("customerId", orderCreateDTO.CustomerId, DbType.Int32);
                parameters.Add("addressId", orderCreateDTO.AddressId, DbType.Int32);

                int result = await connection.QuerySingleAsync<int>(sqlQuery, parameters);
                return result;
            }
        }

        public async Task<OrderInfoDTO> getOrder(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT OrderID, OrderDatetime, TotalMoney, CustomerID, AddressID
                                FROM Orders 
                                 WHERE OrderID = @id";
                //string query = @"EXEC Test";
                var result = await connection.QueryFirstOrDefaultAsync<OrderInfoDTO>(query, new { id });
                return result;
            }
        }
    }
}
