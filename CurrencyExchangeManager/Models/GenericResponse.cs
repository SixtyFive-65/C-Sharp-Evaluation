namespace Models
{
    public interface IGenericResponseModel
    {
        Exception Ex { get; set; }
        string Message { get; set; }
        bool Success { get; set; }
    }

    public class GenericResponse<T> : IGenericResponseModel
    {
        public T Data { get; set; }
        public Exception Ex { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
