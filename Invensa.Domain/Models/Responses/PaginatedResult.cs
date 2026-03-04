namespace Invensa.Domain.Models.Responses;

public sealed class PaginatedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int Total { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
