using Movies.Model.Common;

namespace Movies.Mapper.Factory.Category
{
    public interface ICategoryFactory
    {
        ICommonDto GetModel(string name);
    }
}