using Thunders.Application.Dtos;
using Thunders.Domain.Entities;
using Thunders.Domain.Filter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thunders.Application.Service.Interfaces
{
    public interface IProdutoAppService : IDisposable
    {
        Task<ProdutoDto> Insert(ProdutoDto obj);
        Task<ProdutoDto> Update(long id, ProdutoDto obj);
        Task Inactivate(long id);
        Task<IEnumerable<ProdutoDto>> GetAllActive();
        Task<IEnumerable<Produto>> GetByDto(ProdutoFilter filter);
        Task<ProdutoDto> GetById(long Id);
    }
}
