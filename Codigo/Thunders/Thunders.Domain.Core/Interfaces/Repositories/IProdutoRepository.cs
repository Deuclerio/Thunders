using Thunders.Domain.Entities;
using Thunders.Domain.Filter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thunders.Domain.Core.Interfaces.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<Produto> GetAllActive(long? id);
        Task<IEnumerable<Produto>> GetByDto(ProdutoFilter filter);
    }
}
