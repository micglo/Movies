using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using Movies.Dal;
using Movies.Model.Common;
using Movies.WebApi.Utility.Exception.CustomException;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Service.Common
{
    public abstract class ServiceBase : IServiceBase
    {
        private readonly UrlHelper _url;
        protected IContext Context;
        protected ICustomException CustomException;

        protected ServiceBase(IContext context, IRequestMessageProvider requestMessageProvider, ICustomException customException)
        {
            CustomException = customException;
            Context = context;
            if (requestMessageProvider.CurrentMessage != null)
                _url = new UrlHelper(requestMessageProvider.CurrentMessage);
        }

        public abstract Task<PagedItems> GetAll(int page, int pageSize, string urlLink);
        public abstract Task<ICommonDto> GetById(string id);

        protected PagedItems CreatePagedItems(IEnumerable<ICommonDto> items, string urlLink, int page, int pageSize, long totalNumberOfRecords)
        {
            var mod = totalNumberOfRecords % pageSize;
            var totalPageCount = totalNumberOfRecords / pageSize + (mod == 0 ? 0 : 1);

            string nextPageUrl;
            if (page == totalPageCount || page > totalPageCount)
                nextPageUrl = null;
            else
            {
                nextPageUrl = _url.Link(urlLink, new
                {
                    page = page + 1,
                    pageSize
                });
            }

            string prevPageUrl;
            if (page < 2)
            {
                prevPageUrl = null;
            }
            else
            {
                prevPageUrl = _url.Link(urlLink, new
                {
                    page = page - 1,
                    pageSize
                });
            }

            return new PagedItems
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalNumberOfPages = totalPageCount,
                TotalNumberOfRecords = totalNumberOfRecords,
                NextPageUrl = nextPageUrl,
                PreviousPageUrl = prevPageUrl,
                Items = items
            };
        }
    }
}