namespace Movies.WebApi.Utility.Exception.CustomException
{
    public interface ICustomException
    {
        void ThrowNotFoundException();
        void ThrowNotFoundException(string message);
        void ThrowBadRequestException(string message);
    }
}