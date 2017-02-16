using System.Net.Http;

namespace Movies.WebApi.Utility.RequestMessageProvider
{
    public interface IRequestMessageProvider
    {
        HttpRequestMessage CurrentMessage { get; }
    }
}