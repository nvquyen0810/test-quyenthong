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
    public class HistoryPdfService : IHistoryPdfService
    {
        private readonly IHistoryPdfRepository _historyPDfRepository;
        public HistoryPdfService(IHistoryPdfRepository historyPdfRepository)
        {
            _historyPDfRepository = historyPdfRepository;
        }

        public async Task<int> CreateHistoryPdf(HistoryPdfCreateDTO historyPdfCreateDTO)
        {
            var result = await _historyPDfRepository.CreateHistoryPdf(historyPdfCreateDTO);
            return result;
        }
    }
}
