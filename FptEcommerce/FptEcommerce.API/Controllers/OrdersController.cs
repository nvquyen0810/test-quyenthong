using Confluent.Kafka;
using FptEcommerce.API.Caching;
using FptEcommerce.API.Models;
using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FptEcommerce.API.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        //
        private readonly IHistoryEmailService _historyEmailService;
        private readonly IHistoryPdfService _historyPdfService;
        //
        private readonly IProductService _productService;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IConfiguration _configuration;

        public OrdersController(IOrderService orderService,
            IOrderDetailService orderDetailService,
            IHistoryEmailService historyEmailService,
            IHistoryPdfService historyPdfService,
            IProductService productService,
            IRedisCacheService redisCacheService,
            IConfiguration configuration)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;

            //
            _historyEmailService = historyEmailService;
            _historyPdfService = historyPdfService;
            //
            _productService = productService;
            _redisCacheService = redisCacheService;
            // _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
            _configuration = configuration;
        }


        /// <summary>
        /// lấy thông tin hóa đơn từ id, API này chỉ dùng để test
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> getOrder(int id)   // API này chỉ dùng để test
        {
            try
            {
                var result = await _orderService.getOrder(id);

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
        /// Tạo order mới
        /// </summary>
        /// <param name="orderCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]  // có thể đặt là 'checkout'
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO orderCreateDTO)
        {
            if (orderCreateDTO.TotalMoney <= 0
                || orderCreateDTO.CustomerId <= 0
                || orderCreateDTO.AddressId <= 0
                || orderCreateDTO.Cart == null
                || orderCreateDTO.Cart?.Count == 0)
                return BadRequest(new Response()
                {
                    Success = false,
                    Message = "Invalid data from request body",
                });

            string key = HttpContext.Request.Headers["Authorization"];
            var result = _redisCacheService.Get<string>(key);
            if (!object.Equals(result, default(string)))
            {
                // có thể lấy ra id qua claims
                //var isAuthenticated = User.Identity.IsAuthenticated;
                //var name = User.Identity.Name;
                // var userId = Int32.Parse(User.FindFirst("Id")?.Value);
                var _id = FptEcommerce.Core.Helper.Token.ValidateToken2(_configuration["AppSettings:SecretKey"], result);

                if (_id < 0)
                    return Ok(new Response()
                    {
                        Success = true,
                        Message = "CustomerId does not exist",

                    });

                if (_id != orderCreateDTO.CustomerId)
                    return StatusCode(StatusCodes.Status401Unauthorized, new Response()
                    {
                        Success = true,
                        Message = "Unauthorized",
                    });

                // Tạo Order
                var resultOrderId = await _orderService.CreateOrder(orderCreateDTO);

                if (resultOrderId > 0)
                {
                    List<int> listOrderDetailId = new List<int>();

                    // Tạo Order Detail & update lại Product (số lượng hàng tồn kho)
                    foreach (var item in orderCreateDTO.Cart)
                    {
                        #region UpdateProduct
                        var updateProduct = new ProductUpdateDTO()
                        {
                            ProductID = item.ProductID,
                            UnitsInStockMinus = item.Quantity
                        };
                        var resultUpdateProduct = await _productService.updateProduct(updateProduct);
                        #endregion

                        #region CreateOrderDetail
                        var createOrderDetail = new OrderDetailCreateDTO()
                        {
                            OrderId = resultOrderId,
                            ProductId = item.ProductID,
                            Quantity = item.Quantity,
                            TotalMoney = item.TotalMoney
                        };
                        var resultCreateOrderDetail = await _orderDetailService.CreateOrderDetail(createOrderDetail);

                        listOrderDetailId.Add(resultCreateOrderDetail);
                        #endregion
                    }

                    // push message to Kafka (Producer)
                    var kafkaMessage = new KafkaMessage()
                    {
                        OrderId = resultOrderId,
                        Token = result
                    };
                    var messageKaf = JsonSerializer.Serialize<KafkaMessage>(kafkaMessage);

                    ProducerConfig config = new ProducerConfig
                    { BootstrapServers = _configuration["KafkaSettings:BootstrapServers"] };
                    string topic = "fptecommerce_topic";

                    using (var producer =
                    new ProducerBuilder<string, string>(config).Build())
                    {
                        try
                        {



                            Task t1 = Task.Run(() => producer.ProduceAsync(topic, new Message<string, string>
                            {
                                Key = "OrderIdAndTokenEmail",
                                Value = messageKaf  // OrderId & Token
                            }));

                            Task t2 = Task.Run(() => producer.ProduceAsync(topic, new Message<string, string>
                            {
                                Key = "OrderIdAndTokenPdf",
                                Value = messageKaf   // OrderId & Token
                            }));

                            await Task.WhenAll(t1, t2).ConfigureAwait(false);



                            //Object xEmail = producer.ProduceAsync(topic, new Message<string, string>
                            //{
                            //    Key = "OrderIdAndTokenEmail",
                            //    Value = messageKaf  // OrderId & Token
                            //})
                            //    .GetAwaiter()     // tìm hiểu lại thuật ngữ
                            //    .GetResult();

                            //Object xPdf = producer.ProduceAsync(topic, new Message<string, string>
                            //{
                            //    Key = "OrderIdAndTokenPdf",
                            //    Value = messageKaf   // OrderId & Token
                            //})
                            //    .GetAwaiter()
                            //    .GetResult();


                            // response to client
                            return StatusCode(StatusCodes.Status201Created, new Response()
                            {
                                Success = true,
                                Message = "Created order successfuly",
                                Data = new
                                {
                                    OrderID = resultOrderId,
                                    //HistoryEmailID = resultCreateHistoryEmail,
                                    //HistoryPdfID = resultCreateHistoryPdf,
                                    ListOrderDetailID = listOrderDetailId
                                }
                            });
                        }
                        catch (Exception e)
                        {
                            return Ok(new Response()
                            {
                                Success = false,
                                Message = "Create Order failed",
                            });
                        }
                    }

                }
                else
                {
                    return Ok(new Response()
                    {
                        Success = false,
                        Message = "Create Order failed",
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


        [HttpPost]
        [Route("create-history-email")]
        public async Task<IActionResult> CreateHistoryEmail([FromBody] CreateHistoryDTO createHistory)
        {
            string key = HttpContext.Request.Headers["Authorization"];
            var result = _redisCacheService.Get<string>(key);
            if (!object.Equals(result, default(string)))
            {
                // có thể lấy ra id qua claims
                // check if Orders(createHistory.OrderId, customerId) exist in Database
                // var userId = User.FindFirst("Id")?.Value;
                // var id = Int32.Parse(User.FindFirst("Id")?.Value);
                var customerIdFromToken = FptEcommerce.Core.Helper.Token.ValidateToken2(_configuration["AppSettings:SecretKey"], result);
                var customerIdFromOrder = await _orderService.getOrder(createHistory.OrderId);
                if (customerIdFromOrder.CustomerId != customerIdFromToken)
                    return BadRequest(new Response()
                    {
                        Success = false,
                        Message = "Invalid data from request body",
                    });


                // Start sending email
                try
                {
                    var HistoryEmailCreate = new HistoryEmailCreateDTO()
                    {
                        OrderId = createHistory.OrderId
                    };

                    var numSecondsDelay = 8;
                    // giả vờ delay 5 giây, có thể chỉ dùng 1 câu lệnh await Task.Delay(numSecondsDelay * 1000) là đủ
                    var t = Task.Run(async delegate
                    {
                        await Task.Delay(numSecondsDelay * 1000);
                        return numSecondsDelay;
                    });
                    t.Wait();
                    //
                    var resultCreateHistoryEmail = await _historyEmailService.CreateHistoryEmail(HistoryEmailCreate);
                    return StatusCode(StatusCodes.Status201Created, new Response()
                    {
                        Success = true,
                        Message = "Created history email successfuly",
                        Data = resultCreateHistoryEmail
                    });

                }
                catch (Exception ex)
                {
                    return Ok(new Response()
                    {
                        Success = false,
                        Message = "Create history email failed",
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


        [HttpPost]
        [Route("create-history-pdf")]
        public async Task<IActionResult> CreateHistoryPdf([FromBody] CreateHistoryDTO createHistory)
        {
            string key = HttpContext.Request.Headers["Authorization"];
            var result = _redisCacheService.Get<string>(key);
            if (!object.Equals(result, default(string)))
            {
                // có thể lấy ra id qua claims
                // check if Orders(createHistory.OrderId, customerId) exist in Database
                // var userId = User.FindFirst("Id")?.Value;
                // var id = Int32.Parse(User.FindFirst("Id")?.Value);
                var customerIdFromToken = FptEcommerce.Core.Helper.Token.ValidateToken2(_configuration["AppSettings:SecretKey"], result);
                var customerIdFromOrder = await _orderService.getOrder(createHistory.OrderId);
                if (customerIdFromOrder.CustomerId != customerIdFromToken)
                    return BadRequest(new Response()
                    {
                        Success = false,
                        Message = "Invalid data from request body",
                    });

                // Start creating pdf
                try
                {
                    var historyPdfCreateDTO = new HistoryPdfCreateDTO()
                    {
                        OrderId = createHistory.OrderId
                    };

                    var numSecondsDelay = 8;
                    // giả vờ delay 5 giây, có thể chỉ dùng 1 câu lệnh await Task.Delay(numSecondsDelay * 1000) là đủ
                    var t = Task.Run(async delegate
                    {
                        await Task.Delay(numSecondsDelay * 1000);
                        return numSecondsDelay;
                    });
                    t.Wait();
                    //
                    var resultCreateHistoryPdf = await _historyPdfService.CreateHistoryPdf(historyPdfCreateDTO);
                    return StatusCode(StatusCodes.Status201Created, new Response()
                    {
                        Success = true,
                        Message = "Created history pdf successfuly",
                        Data = resultCreateHistoryPdf
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new Response()
                    {
                        Success = false,
                        Message = "Create history pdf failed",
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
