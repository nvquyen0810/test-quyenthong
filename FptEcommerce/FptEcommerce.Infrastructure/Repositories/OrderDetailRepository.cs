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
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly string _connectionString;
        public OrderDetailRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateOrderDetail(OrderDetailCreateDTO orderDetailCreateDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"INSERT into OrderDetails (OrderID, ProductID, Quantity, TotalMoney)
                                    OUTPUT Inserted.OrderDetailID
                                    values (@orderId, @productId, @quantity, @totalMoney)"; // SQL query that we want to execute with dapper
                var parameters = new DynamicParameters();
                parameters.Add("orderId", orderDetailCreateDTO.OrderId, DbType.Int32);
                parameters.Add("productId", orderDetailCreateDTO.ProductId, DbType.Int32);
                parameters.Add("quantity", orderDetailCreateDTO.Quantity, DbType.Int32);
                parameters.Add("totalMoney", orderDetailCreateDTO.TotalMoney, DbType.Int32);

                int result = await connection.QuerySingleAsync<int>(sqlQuery, parameters);
                return result;
            }
        }
    }
}
