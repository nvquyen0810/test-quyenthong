using FptEcommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Interfaces.Infrastructure
{
    public interface IProductRepository
    {
        Task<List<ProductInfoDTO>> getLastest();
        Task<List<ProductInfoDTO>> getProductsByPage(string search, int perPage, int currentPage);
        Task<int> getProductQuantity(string search);
        Task<ProductInfoDTO> getProductDetail(int id);
        Task<int> updateProduct(ProductUpdateDTO updateDTO);
    }
}
