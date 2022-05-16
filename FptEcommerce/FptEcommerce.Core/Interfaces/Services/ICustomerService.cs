using FptEcommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<CustomerInfoDTO> GetUserByUsernameAndPassword(CutomerLoginDTO userLogin);
    }
}
