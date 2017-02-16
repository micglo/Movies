using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using MongoDB.Bson;
using Movies.WebApi.Utility.Exception.CustomException;

namespace Movies.WebApi.Utility.Filter
{
    public class ValidateIdFormatAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ActionArguments.ContainsKey("id"))
            {
                var id = (string) actionContext.ActionArguments["id"];

                try
                {
                    var result = ObjectId.Parse(id);
                }
                catch (FormatException e)
                {
                    var ex = new CustomException();
                    ex.ThrowBadRequestException(e.Message);
                }
            }
        }
    }
}