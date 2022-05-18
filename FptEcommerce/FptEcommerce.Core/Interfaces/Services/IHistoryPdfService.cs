using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Interfaces.Services
{
    public interface IHistoryPdfService
    {
        // Tạo mới 
        Task<int> CreateHistoryPdf(HistoryPdfCreateDTO historyPdfCreateDTO);
    }
}
