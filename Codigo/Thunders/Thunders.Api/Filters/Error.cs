namespace Thunders.Api.Filters
{
    public class Error
    {
        public string message { get; set; }
        public int status { get; set; }

        public Error(int status, string message)
        {
            this.status = status;
            this.message = message;
        }

        public Error()
        {
            message = string.Empty;
        }
    }
}
