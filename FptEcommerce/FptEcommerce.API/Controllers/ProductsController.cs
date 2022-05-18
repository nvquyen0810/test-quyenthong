using FptEcommerce.API.Caching;
using FptEcommerce.API.Models;
using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FptEcommerce.API.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Lấy ra 4 sản phẩm mới nhất (thêm vào gần đây nhất)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lastest")]
        public async Task<IActionResult> getLastest()
        {
            try
            {
                var result = await _productService.getLastest();

                return Ok(new Response()
                {
                    Success = true,
                    Message = "OK",
                    Data = result
                }
                 );
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Dùng để phân trang sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("pages")]
        public async Task<IActionResult> getProductsByPage(string search, int perPage, int currentPage)
        {
            if (search == null || search == " ")
                search = "";

            try
            {
                var count = await _productService.getProductQuantity(search);
                var result = await _productService.getProductsByPage(search, perPage, currentPage);

                return Ok(new Response()
                {
                    Success = true,
                    Message = "OK",
                    Data = new Pagination<ProductInfoDTO>()
                    {
                        Total = count,
                        Items = result
                    }
                }
                 );
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Lấy chi tiết sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> getProductDetail(int id)
        {
            try
            {
                var result = await _productService.getProductDetail(id);

                return Ok(new Response()
                {
                    Success = true,
                    Message = "OK",
                    Data = result
                }
                 );
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
