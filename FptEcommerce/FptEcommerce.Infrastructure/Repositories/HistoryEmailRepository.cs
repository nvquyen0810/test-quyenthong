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
    public class HistoryEmailRepository : IHistoryEmailRepository
    {
        private readonly string _connectionString;
        public HistoryEmailRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateHistoryEmail(HistoryEmailCreateDTO historyEmailCreateDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"INSERT into HistoryEmail (OrderID)
                                    OUTPUT Inserted.HistoryEmailID
                                    values (@orderId)"; // SQL query that we want to execute with dapper
                var parameters = new DynamicParameters();
                parameters.Add("orderId", historyEmailCreateDTO.OrderId, DbType.Int32);

                int result = await connection.QuerySingleAsync<int>(sqlQuery, parameters);
                return result;
            }
        }
    }
}
