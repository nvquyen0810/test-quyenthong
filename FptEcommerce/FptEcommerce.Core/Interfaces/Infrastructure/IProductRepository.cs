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
        Task<List<ProductInfoDTO>> getProductsByPage(int perPage, int currentPage);
        Task<int> getProductQuantity();
        Task<ProductInfoDTO> getProductDetail(int id);
    }
}
