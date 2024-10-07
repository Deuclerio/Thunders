using AutoMapper;
using Thunders.Domain.Core.Interfaces.Repositories;

namespace Thunders.Application.Base
{
    public abstract class AppService(IUnitOfWork uoW, IMapper Mapper)
    {
        protected IUnitOfWork UoW { get; set; } = uoW;
        protected IMapper Mapper { get; set; } = Mapper;
       
    }
}
