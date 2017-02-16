using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Movies.WebApi.Utility.Exception.CustomException;

namespace Movies.WebApi.Utility.Filter
{
    public class CheckPaginationQueryAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var page = (int)actionContext.ActionArguments["page"];
            var pageSize = (int)actionContext.ActionArguments["pageSize"];

            if (page < 1 || pageSize < 1)
            {
                var ex = new CustomException();
                ex.ThrowBadRequestException("Value of page and pageSize cannot be less than 1");
            }
        }
    }
}