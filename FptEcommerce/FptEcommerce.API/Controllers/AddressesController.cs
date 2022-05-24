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
    [Route("api/v1/addresses")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAddressService _addressService;

        public AddressesController(IRedisCacheService redisCacheService, IAddressService addressService)
        {
            _redisCacheService = redisCacheService;
            _addressService = addressService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAddress()
        {
            try
            {
                var listAddress = _redisCacheService.Get<List<AddressDTO>>("list_address");
                if (listAddress != null)
                    return Ok(new Response()
                    {
                        Success = true,
                        Message = "List Address (Redis)",
                        Data = listAddress
                    });

                var addresses = await _addressService.getAll();
                if (addresses != null)
                {
                    _redisCacheService.Set<List<AddressDTO>>("list_address", addresses, 24 * 60, 60);
                    return Ok(new Response()
                    {
                        Success = true,
                        Message = "List Address (Database)",
                        Data = addresses
                    });
                }
                else
                {
                    return Ok(new Response()
                    {
                        Success = true,
                        Message = "Empty",
                        Data = null
                    });
                }

            }
            catch (Exception ex)  //ex.Message, ex.Data
            {
                //var result = new
                //{
                //    devMsg = ex.Message,
                //    useMsg = "có lỗi xảy ra",
                //    data = DBNull.Value,
                //    moreInfo = ""
                //};
                //return StatusCode(500, result);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
