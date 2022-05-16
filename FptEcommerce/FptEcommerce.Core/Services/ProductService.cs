using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Interfaces.Infrastructure;
using FptEcommerce.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductInfoDTO>> getLastest()
        {
            var result = await _productRepository.getLastest();
            return result;
        }

        public async Task<int> getProductQuantity()
        {
            var result = await _productRepository.getProductQuantity();
            return result;
        }

        public async Task<List<ProductInfoDTO>> getProductsByPage(int perPage, int currentPage)
        {
            var result = await _productRepository.getProductsByPage(perPage, currentPage);
            return result;
        }

        public async Task<ProductInfoDTO> getProductDetail(int id)
        {
            var result = await _productRepository.getProductDetail(id);
            return result;
        }
    }
}
