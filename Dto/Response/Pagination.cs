namespace user_service_api.Dto;

public class Pagination<T>
{
    public int Current { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<T> Data { get; set; }
}