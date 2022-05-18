using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Interfaces.Infrastructure
{
    public interface ICustomerRepository
    {
        Task<CustomerInfoDTO> GetUserByUsernameAndPassword(CutomerLoginDTO userLogin);
        Task<int> UpdateCustomerInfo(int customerId, CustomerInfoUpdateDTO userUpdate);
        Task<int> TestCreateReturnId(CustomerInfoUpdateDTO updateDTO);
    }
}
