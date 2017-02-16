using System.Web.Http;

namespace Movies.WebApi.Utility.VersioningPrefix
{
    public class ApiVersion1RoutePrefixAttribute : RoutePrefixAttribute
    {
        private const string RouteBase = "api/v1";
        private const string PrefixRouteBase = RouteBase + "/";

        public ApiVersion1RoutePrefixAttribute(string routePrefix)
            : base(string.IsNullOrWhiteSpace(routePrefix) ? RouteBase : PrefixRouteBase + routePrefix)
        {
        }
    }
}