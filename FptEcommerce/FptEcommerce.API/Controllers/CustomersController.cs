using FluentValidation.Results;
using FptEcommerce.API.Caching;
using FptEcommerce.API.Models;
using FptEcommerce.API.Validators;
using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FptEcommerce.API.Controllers
{
    [Route("api/v1/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ICustomerService _customerService;

        public CustomersController(IConfiguration configuration,
            IRedisCacheService redisCacheService,
            ICustomerService customerService)
        {
            _configuration = configuration;
            _redisCacheService = redisCacheService;
            _customerService = customerService;
        }


        /// <summary>
        /// Đăng nhập (login)
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(CutomerLoginDTO userLogin)
        {

            List<string> ValidationMessages = new List<string>();
            var validateResult = await new CustomerLoginValidator().ValidateAsync(userLogin);
            if (!validateResult.IsValid)
            {
                foreach (ValidationFailure failure in validateResult.Errors)
                {
                    ValidationMessages.Add(failure.ErrorMessage);
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = ValidationMessages,
                });
            }

            try
            {
                var user = await _customerService.GetUserByUsernameAndPassword(userLogin);

                if (user == null) //không đúng
                {
                    return Ok(new Response
                    {
                        Success = false,
                        Message = "Invalid username/password"
                    });
                }

                // lưu vào token vào redis
                var accessToken = FptEcommerce.Core.Helper.Token.GenerateToken(_configuration["AppSettings:SecretKey"], user, 24, 0);
                string key = "Bearer " + accessToken;
                _redisCacheService.Set<string>(key, accessToken, 60 * 24, 60 * 24);
                //cấp token
                return Ok(new Response
                {
                    Success = true,
                    Message = "Authenticated successfully",
                    Data = new
                    {
                        accessToken,
                        user
                    }
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("create-returnid")]
        public async Task<IActionResult> TestCreateReturnId([FromBody] CustomerInfoUpdateDTO updateDTO)
        {

            var resultId = await _customerService.TestCreateReturnId(updateDTO);
            return Ok(resultId);
        }


        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            //var username = "";
            //var id = "";
            //var email = "";

            //var claimName = "";
            //var claimEmail = "";

            //if (User.Identity.IsAuthenticated)
            //{
            //    username = User.FindFirst("UserName")?.Value;
            //    id = User.FindFirst("Id")?.Value;
            //    email = User.FindFirst("Email")?.Value;

            //    //claimName = User.FindFirst(ClaimTypes.Name)?.Value;
            //    //claimEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            //}
            string key = HttpContext.Request.Headers["Authorization"];
            var result = _redisCacheService.Get<string>(key);
            if (!object.Equals(result, default(string)))
            {
                _redisCacheService.Remove(key);
                return Ok(new Response()
                {
                    Success = true,
                    Message = "Logged out successfuly"
                });
            }
            else     // token null (in redis cache)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response()
                {
                    Success = false,
                    Message = "Logged out fail"
                });
            }
        }


        [HttpPost]
        [Route("update-info")]
        [Authorize]
        public async Task<IActionResult> UpdateInfo([FromBody] CustomerInfoUpdateDTO userUpdate)
        {
            string key = HttpContext.Request.Headers["Authorization"];
            var result = _redisCacheService.Get<string>(key);
            if (!object.Equals(result, default(string)))
            {
                var _id = FptEcommerce.Core.Helper.Token.ValidateToken2(_configuration["AppSettings:SecretKey"], result);

                if (_id < 0)
                    return Ok(new Response()
                    {
                        Success = true,
                        Message = "CustomerId does not exist",

                    });

                var updateResult = await _customerService.UpdateCustomerInfo(_id, userUpdate);

                if (updateResult > 0)
                {
                    return Ok(new Response()
                    {
                        Success = true,
                        Message = "Update info successfuly",
                        Data = new
                        {
                            customerId = _id
                        }
                    });
                }
                else
                {
                    return Ok(new Response()
                    {
                        Success = false,
                        Message = "Update info failed",
                    });
                }

            }
            else     // token null (in redis cache)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response()
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
        }
    }
}
