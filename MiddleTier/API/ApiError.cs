namespace API
{
    public class ApiError
    {
        public ApiError(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }
    }
}
