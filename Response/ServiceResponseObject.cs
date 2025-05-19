using RESTfulBookWebsite.Models.Dto;

namespace RESTfulBookWebsite.Response
{
    public class ServiceResponseObject<T>
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }
        public int? ErrorCode { get; set; } 

    }
}
