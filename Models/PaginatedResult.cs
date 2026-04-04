namespace MoneyFixClient.Models
{
    public class PaginatedResult<T>
    {
        public T Items { get; set; } = default!;
        public Pagination? Pagination { get; set; }
    }
}