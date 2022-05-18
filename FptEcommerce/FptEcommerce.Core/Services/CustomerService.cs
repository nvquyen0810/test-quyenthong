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
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerInfoDTO> GetUserByUsernameAndPassword(CutomerLoginDTO userLogin)
        {
            var result = await _customerRepository.GetUserByUsernameAndPassword(userLogin);
            return result;
        }

        public async Task<int> TestCreateReturnId(CustomerInfoUpdateDTO updateDTO)
        {
            var result = await _customerRepository.TestCreateReturnId(updateDTO);
            return result;
        }

        public async Task<int> UpdateCustomerInfo(int customerId, CustomerInfoUpdateDTO userUpdate)
        {
            var result = await _customerRepository.UpdateCustomerInfo(customerId, userUpdate);
            return result;
        }
    }
}
