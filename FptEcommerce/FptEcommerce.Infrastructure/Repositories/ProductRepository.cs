using Dapper;
using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using FptEcommerce.Core.

namespace FptEcommerce.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<ProductInfoDTO>> getLastest()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //string query = @"SELECT TOP 4*from dbo.Products ORDER BY ProductID DESC";
                string query = @"EXEC Test";
                var result = await connection.QueryAsync<ProductInfoDTO>(query);

                return result.ToList();
            }
        }

        public async Task<List<ProductInfoDTO>> getProductsByPage(int perPage, int currentPage)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Products.ProductID, Products.Name, Description, Thumb, Price
                                FROM Products 
                                 ORDER BY ProductID ASC
                                 OFFSET @skipPage ROWS 
                                 FETCH NEXT @perPage ROWS ONLY";
                //string query = @"EXEC Test";
                var result = await connection.QueryAsync<ProductInfoDTO>(query, new { skipPage = (currentPage - 1) * 5, perPage });

                return result.ToList();
            }
        }

        public async Task<int> getProductQuantity()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Count(Products.ProductID)
                                FROM Products ";
                //string query = @"EXEC Test";
                var result = await connection.ExecuteScalarAsync<int>(query);
                return result;
            }
        }

        public async Task<ProductInfoDTO> getProductDetail(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Products.ProductID, Products.Name, Description, Thumb, Price
                                FROM Products 
                                 WHERE ProductID = @id";
                //string query = @"EXEC Test";
                var result = await connection.QueryFirstOrDefaultAsync<ProductInfoDTO>(query, new { id });
                return result;
            }
        }
    }
}
