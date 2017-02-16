using Movies.Domain.Common;
using Movies.Model.Common;

namespace Movies.Mapper.Factory.Common
{
    public interface IDomainFactory
    {
        ICommonEntity GetModel(ICommonDto dtoModel);
    }
}