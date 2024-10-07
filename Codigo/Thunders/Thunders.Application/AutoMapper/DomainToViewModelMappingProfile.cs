using AutoMapper;
using Thunders.Application.Dtos;
using Thunders.Domain.Entities;

namespace Thunders.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {           
            CreateMap<Produto, ProdutoDto>();
           
        }
    }
}
