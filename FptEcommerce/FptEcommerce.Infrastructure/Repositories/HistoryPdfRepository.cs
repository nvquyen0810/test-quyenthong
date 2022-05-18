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
    public class HistoryPdfRepository : IHistoryPdfRepository
    {
        private readonly string _connectionString;
        public HistoryPdfRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateHistoryPdf(HistoryPdfCreateDTO historyPdfCreateDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"INSERT into HistoryPdf (OrderID)
                                    OUTPUT Inserted.HistoryPdfID
                                    values (@orderId)"; // SQL query that we want to execute with dapper
                var parameters = new DynamicParameters();
                parameters.Add("orderId", historyPdfCreateDTO.OrderId, DbType.Int32);

                int result = await connection.QuerySingleAsync<int>(sqlQuery, parameters);
                return result;
            }
        }
    }
}
