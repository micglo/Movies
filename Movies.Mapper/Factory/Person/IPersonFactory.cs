using Movies.Model.Common;

namespace Movies.Mapper.Factory.Person
{
    public interface IPersonFactory
    {
        ICommonDto GetModel(string firstName, string lastName, string personType = "actor");
    }
}