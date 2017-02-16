using System.Web.Http.Routing;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Mapper.Factory.Common
{
    public abstract class FactoryBase
    {
        protected UrlHelper Url { get; set; }
        protected FactoryBase(IRequestMessageProvider requestMessageProvider)
        {
            if (requestMessageProvider.CurrentMessage != null)
                Url = new UrlHelper(requestMessageProvider.CurrentMessage);
        }

        protected bool TypesEqual<T1, T2>()
            => typeof(T1) == typeof(T2);

        protected bool TypesEqual<T>(object model)
            => model.GetType() == typeof(T);
    }
}