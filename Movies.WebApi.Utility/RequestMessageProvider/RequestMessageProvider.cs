using System.Net.Http;
using SimpleInjector;

namespace Movies.WebApi.Utility.RequestMessageProvider
{
    public class RequestMessageProvider : IRequestMessageProvider
    {
        private readonly Container _container;

        public RequestMessageProvider(Container container)
        {
            _container = container;
        }

        public HttpRequestMessage CurrentMessage => _container.GetCurrentHttpRequestMessage();
    }
}