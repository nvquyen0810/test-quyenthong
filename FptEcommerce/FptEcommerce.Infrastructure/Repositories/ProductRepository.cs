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
                string query = @"EXEC Proc_GetLastestProducts";
                var result = await connection.QueryAsync<ProductInfoDTO>(query);

                return result.ToList();
            }
        }

        public async Task<List<ProductInfoDTO>> getProductsByPage(string search, int perPage, int currentPage)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Products.ProductID, Products.Name, Description, Thumb, Price
                                FROM Products 
                                WHERE LOWER(Products.Name) LIKE N'%' + @my + '%'
                                 ORDER BY ProductID ASC
                                 OFFSET @skipPage ROWS 
                                 FETCH NEXT @perPage ROWS ONLY";
                //string query = @"EXEC Test";
                var result = await connection.QueryAsync<ProductInfoDTO>(query,
                    new
                    {
                        my = search.ToLower(),
                        skipPage = (currentPage - 1) * 5,
                        perPage
                    });

                return result.ToList();
            }
        }

        //public async Task<int> getProductQuantity(string search)
        //{
        //    //var sQuery = "N%" + search + "%";
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        string query = @"SELECT Count(Products.ProductID)
        //                        FROM Products 
        //                        WHERE LOWER(Name) LIKE @sQuery";
        //        //string query = @"EXEC Test";
        //        var result = await connection.ExecuteScalarAsync<int>(query, new { sQuery = "N%" + search + "%" });
        //        var x = 2;
        //        return result;
        //    }
        //}
        public async Task<int> getProductQuantity(string search)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Count(Products.ProductID)
                                FROM Products 
                                WHERE LOWER(Name) LIKE N'%' + @my + '%'";
                //string query = @"EXEC Test";
                var result = await connection.ExecuteScalarAsync<int>(query, new { my = search.ToLower() });
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

        public async Task<int> updateProduct(ProductUpdateDTO updateDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string q = @"UPDATE dbo.Products 
                         SET UnitsInStock = UnitsInStock - @unit
                         WHERE ProductID = @id";   // không nên đếm count(*)

                var result = await connection.ExecuteAsync(q,
                               new
                               {
                                   @unit = updateDTO.UnitsInStockMinus,
                                   @id = updateDTO.ProductID
                               });
                return result;
            }
        }
    }
}
