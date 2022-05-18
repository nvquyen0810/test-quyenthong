using FptEcommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Interfaces.Infrastructure
{
    public interface IOrderDetailRepository
    {
        // Tạo mới 1 OrderDetail
        Task<int> CreateOrderDetail(OrderDetailCreateDTO orderDetailCreateDTO);
    }
}
