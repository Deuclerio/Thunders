using Thunders.Application.Base;
using Thunders.Application.Dtos;
using Thunders.Application.Service.Interfaces;
using Thunders.Domain.Core.Interfaces.Repositories;
using Thunders.Domain.Core.Interfaces.Services;
using Thunders.Domain.Entities;
using Thunders.Domain.Filter;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Text.Json;

namespace Thunders.Application.Services
{
    public class ProdutoAppService : AppService, IProdutoAppService
    {
        private readonly IDatabase _redisDatabase;
        private readonly CacheSettings _cacheSettings;

        private readonly IMapper _mapper;
        private readonly IProdutoService _produtoService;
        private readonly string _nomeTela = "Produto";

        public ProdutoAppService(IConnectionMultiplexer redis, IUnitOfWork uoW,
                                 IMapper mapper,
                                 IProdutoService produtoService,
                                  CacheSettings cacheSettings) : base(uoW, mapper)
        {
            _redisDatabase = redis.GetDatabase();
            _mapper = mapper;
            _produtoService = produtoService;
            _cacheSettings = cacheSettings; 
        }

        public async Task<IEnumerable<Produto>> GetByDto(ProdutoFilter filter)
        {
            try
            {
                var listaProduto = await _produtoService.GetByDto(filter);
                var listaProdutoDto = _mapper.Map<IEnumerable<Produto>>(listaProduto);

                return listaProdutoDto;
            }
            catch (Exception ex)
            {
                Error(ex, "GetByDto");
                throw;
            }
        }

        public async Task<IEnumerable<ProdutoDto>> GetAllActive()
        {
            try
            {
                var cacheKey = "produtos:allActive"; 
                var cachedProdutos = await _redisDatabase.StringGetAsync(cacheKey);

                if (cachedProdutos.HasValue)
                {
                    return JsonSerializer.Deserialize<IEnumerable<ProdutoDto>>(cachedProdutos);
                }

                var listaProduto = await _produtoService.GetAll();
                var listaProdutoDto = _mapper.Map<IEnumerable<ProdutoDto>>(listaProduto);

                await _redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(listaProdutoDto), TimeSpan.FromSeconds(_cacheSettings.ProdutoCacheDuration));

                return listaProdutoDto;
            }
            catch (Exception ex)
            {
                Error(ex, "GetAllActive");
                throw;
            }
        }

        public async Task<ProdutoDto> GetById(long Id)
        {
            try
            {
                var cacheKey = $"produto:{Id}";
                var cachedProduto = await _redisDatabase.StringGetAsync(cacheKey);
                if (cachedProduto.HasValue)
                {
                    return JsonSerializer.Deserialize<ProdutoDto>(cachedProduto);
                }

                var produto = await _produtoService.GetByIdActive(Id);
                if (produto == null) throw new Exception("Produto não encontrado.");

                var produtoDto = _mapper.Map<ProdutoDto>(produto);

                await _redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(produtoDto), TimeSpan.FromSeconds(_cacheSettings.ProdutoCacheDuration));

                return produtoDto;
            }
            catch (Exception ex)
            {
                Error(ex, "GetById");
                throw;
            }
        }

        public async Task Inactivate(long id)
        {
            try
            {
                UoW.BeginTransaction();
                var produtoBase = await _produtoService.GetByIdActive(id);
                if (produtoBase == null)
                {
                    throw new Exception("Produto não encontrado.");
                }

                produtoBase.Situacao = false;
                await _produtoService.Inactivate(Mapper.Map<Produto>(produtoBase));

                UoW.Commit();
            }
            catch (Exception ex)
            {
                UoW.Rollback();
                Error(ex, "Inactivate");
                throw ex;
            }
        }

        public async Task<ProdutoDto> Insert(ProdutoDto obj)
        {
            try
            {
                UoW.BeginTransaction();

                ValidationDate(obj.DataFabricacao, obj.DataValidade);

                var produto = Mapper.Map<Produto>(obj);
                var produtoSave = await _produtoService.Insert(Mapper.Map<Produto>(produto));

                UoW.Commit();

                return Mapper.Map<ProdutoDto>(produtoSave);
            }
            catch (Exception ex)
            {
                UoW.Rollback();
                Error(ex, "Insert");
                throw;
            }
        }

        public static void ValidationDate(DateTime? dataFabricacao, DateTime? dataValidade)
        {
            if (!dataFabricacao.HasValue)
            {
                throw new Exception("Data de fabricação é inválida.");
            }

            if (!dataValidade.HasValue)
            {
                throw new Exception("Data de validade é inválida.");
            }

            if (dataFabricacao > DateTime.Now)
            {
                throw new Exception("Data de fabricação não pode ser uma data futura.");
            }

            if (dataValidade > DateTime.Now && dataFabricacao >= dataValidade)
            {
                throw new Exception("Data de fabricação deve ser menor que a data de validade.");
            }
        }

        public async Task<ProdutoDto> Update(long id, ProdutoDto obj)
        {
            try
            {
                ValidationDate(obj.DataFabricacao, obj.DataValidade);

                UoW.BeginTransaction();
                var produtoBase = await _produtoService.GetByIdActive(id);
                if (produtoBase == null)
                {
                    throw new Exception("Produto não encontrado.");
                }

                produtoBase.Id = id;
                produtoBase.Descricao = obj.Descricao;
                produtoBase.Situacao = obj.Situacao;

                produtoBase.DataFabricacao = obj.DataFabricacao;
                produtoBase.DataValidade = obj.DataValidade;

                produtoBase.CodigoFornecedor = obj.CodigoFornecedor;
                produtoBase.DescricaoFornecedor = obj.DescricaoFornecedor;
                produtoBase.CnpjFornecedor = obj.CnpjFornecedor;

                var produto = await _produtoService.Update(produtoBase);


                UoW.Commit();
                return Mapper.Map<ProdutoDto>(produto);
            }
            catch (Exception ex)
            {
                UoW.Rollback();
                Error(ex, "Update");
                throw;
            }
        }

        private void Error(Exception ex, string acao)
        {
            _produtoService.showError(_nomeTela, ex?.Message);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}
