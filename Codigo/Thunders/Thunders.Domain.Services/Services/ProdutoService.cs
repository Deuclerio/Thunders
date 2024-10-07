using Thunders.Domain.Core.Interfaces.Repositories;
using Thunders.Domain.Core.Interfaces.Services;
using Thunders.Domain.Entities;
using Thunders.Domain.Filter;
using Thunders.Domain.Services.Validations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thunders.Domain.Services.Services
{
    public class ProdutoService : Service<Produto>, IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IUnitOfWork context, IProdutoRepository produtoRepository) : base(context, produtoRepository)
        {
            Validator = new ProdutoValidator();
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<Produto>> GetByDto(ProdutoFilter filter)
        {
            return await _produtoRepository.GetByDto(filter);
        }

        public async Task<Produto> GetByIdActive(long? id)
        {
            return await _produtoRepository.GetAllActive(id!.Value);
        }
    }
}
