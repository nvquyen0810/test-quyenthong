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
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<int> CreateOrder(OrderCreateDTO orderCreateDTO)
        {
            var result = await _orderRepository.CreateOrder(orderCreateDTO);
            return result;
        }

        public async Task<OrderInfoDTO> getOrder(int id)
        {
            var result = await _orderRepository.getOrder(id);
            return result;
        }
    }
}
