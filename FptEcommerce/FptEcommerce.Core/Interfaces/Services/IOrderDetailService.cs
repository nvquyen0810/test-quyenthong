using FptEcommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Interfaces.Services
{
    public interface IOrderDetailService
    {
        // Tạo mới 1 OrderDetail, trả về id vừa tạo
        Task<int> CreateOrderDetail(OrderDetailCreateDTO orderDetailCreateDTO);
    }
}
