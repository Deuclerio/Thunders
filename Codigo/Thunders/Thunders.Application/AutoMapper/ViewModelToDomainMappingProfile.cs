using AutoMapper;
using Thunders.Application.Dtos;
using Thunders.Domain.Entities;

namespace Thunders.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ProdutoDto, Produto>();
        }
    }
}
