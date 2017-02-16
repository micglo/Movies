using Movies.Domain.Common;
using Movies.Model.Common;

namespace Movies.Mapper.Factory.Common
{
    public interface IModelFactory
    {
        ICommonDto GetModel<TDto>(ICommonEntity domainEntity) where TDto: CommonDto;
    }
}