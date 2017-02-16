namespace Movies.Service.Logger
{
    public interface ILogService
    {
        void Log(string errorMessage, string innerErrorMessage, string stackTrace);
    }
}