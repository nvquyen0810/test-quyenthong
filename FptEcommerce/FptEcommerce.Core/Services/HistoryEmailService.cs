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
    public class HistoryEmailService : IHistoryEmailService
    {
        private readonly IHistoryEmailRepository _historyEmailRepository;
        public HistoryEmailService(IHistoryEmailRepository historyEmailRepository)
        {
            _historyEmailRepository = historyEmailRepository;
        }

        public async Task<int> CreateHistoryEmail(HistoryEmailCreateDTO historyEmailCreateDTO)
        {
            var result = await _historyEmailRepository.CreateHistoryEmail(historyEmailCreateDTO);
            return result;
        }
    }
}
