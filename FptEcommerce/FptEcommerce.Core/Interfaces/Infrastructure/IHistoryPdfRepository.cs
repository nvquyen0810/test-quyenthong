using FptEcommerce.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Interfaces.Infrastructure
{
    public interface IHistoryPdfRepository
    {
        // Tạo mới 
        Task<int> CreateHistoryPdf(HistoryPdfCreateDTO historyPdfCreateDTO);
    }
}
