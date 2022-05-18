using FptEcommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Interfaces.Infrastructure
{
    public interface IHistoryEmailRepository
    {
        // Tạo mới 
        Task<int> CreateHistoryEmail(HistoryEmailCreateDTO historyEmailCreateDTO);
    }
}
