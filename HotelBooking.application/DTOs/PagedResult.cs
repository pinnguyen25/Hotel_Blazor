public class PagedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalRecords { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    // Tính toán số trang (tùy chọn, FE tự tính cũng được)
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
}