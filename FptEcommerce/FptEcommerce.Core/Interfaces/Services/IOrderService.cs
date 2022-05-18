using FptEcommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Interfaces.Services
{
    public interface IOrderService
    {
        // lấy thông tin chi tiết 1 hóa đơn theo id
        Task<OrderInfoDTO> getOrder(int id);

        // Tạo mới 1 hóa đơn, trả về id Order vừa tạo
        Task<int> CreateOrder(OrderCreateDTO orderCreateDTO);
    }
}
