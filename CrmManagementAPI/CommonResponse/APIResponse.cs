using System.Net;

namespace CrmManagementAPI.CommonResponse
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public string ActionResponse { get; set; }
        public object Result { get; set; }

    }

    public class APIPaginationSearchingResponse
    {
        public bool IsSuccess { get; set; }

        public string? ActionResponse { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public object? Result { get; set; }

    }

}
