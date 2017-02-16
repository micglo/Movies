namespace Movies.WebApi.Utility.Exception.CustomException
{
    public class CustomException : ICustomException
    {
        public void ThrowNotFoundException()
        {
            throw new NotFoundException("Resource not found.");
        }

        public void ThrowNotFoundException(string message)
        {
            throw new NotFoundException($"{message}");
        }

        public void ThrowBadRequestException(string message)
        {
            throw new BadRequestException($"{message}");
        }
    }

    public class NotFoundException : System.Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, System.Exception ex) : base(message, ex) { }
    }

    public class BadRequestException : System.Exception
    {
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string message, System.Exception ex) : base(message, ex) { }
    }
}