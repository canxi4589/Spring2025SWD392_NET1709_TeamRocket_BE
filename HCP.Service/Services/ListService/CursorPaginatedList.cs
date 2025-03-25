using Microsoft.EntityFrameworkCore;

namespace Cursus.Core.Specifications;

public interface IEntityHasCreatedDate
{
    Guid Id { get; }
    DateTime CreatedDate { get; }
}

public class CursorPaginatedList<T> : List<T> where T : IEntityHasCreatedDate
{
    public (DateTime CreatedDate, Guid Id)? LastCursor { get; private set; }
    public (DateTime CreatedDate, Guid Id)? StartCursor { get; private set; }
    public bool HasPreviousPage { get; private set; }
    public bool HasNextPage { get; private set; }

    public CursorPaginatedList(List<T> items, (DateTime CreatedDate, Guid Id)? lastCursor,
        (DateTime CreatedDate, Guid Id)? startCursor, bool hasPreviousPage = false, bool hasNextPage = false)
    {
        LastCursor = lastCursor;
        StartCursor = startCursor;
        HasPreviousPage = hasPreviousPage;
        HasNextPage = hasNextPage;
        this.AddRange(items);
    }

    public static async Task<CursorPaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageSize,
        (DateTime CreatedDate, Guid Id)? cursor = null,
        bool isDescending = false,
        bool isPrevious = false
    )
    {
        if (!cursor.HasValue)
        {
            // Default ordering for initial page load
            source = isDescending
                ? source.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id)
                : source.OrderBy(x => x.CreatedDate).ThenBy(x => x.Id);
        }
        else
        {
            var (createdDate, id) = cursor.Value;
            if (isPrevious)
            {
                source = isDescending
                    ? source
                        .Where(x => x.CreatedDate > createdDate ||
                                    x.CreatedDate == createdDate && x.Id.CompareTo(id) > 0)
                        .OrderBy(x => x.CreatedDate)
                        .ThenBy(x => x.Id)
                    : source
                        .Where(x => x.CreatedDate < createdDate ||
                                    x.CreatedDate == createdDate && x.Id.CompareTo(id) < 0)
                        .OrderByDescending(x => x.CreatedDate)
                        .ThenByDescending(x => x.Id);
            }
            else
            {
                source = isDescending
                    ? source
                        .Where(x => x.CreatedDate < createdDate ||
                                    x.CreatedDate == createdDate && x.Id.CompareTo(id) < 0)
                        .OrderByDescending(x => x.CreatedDate)
                        .ThenByDescending(x => x.Id)
                    : source
                        .Where(x => x.CreatedDate > createdDate ||
                                    x.CreatedDate == createdDate && x.Id.CompareTo(id) > 0)
                        .OrderBy(x => x.CreatedDate)
                        .ThenBy(x => x.Id);
            }
        }

        var items = await source.Take(pageSize + 1).ToListAsync();
        if (items.Count == 0) return new CursorPaginatedList<T>(items, null, null);

        var hasNextPage = items.Count == pageSize + 1;
        var hasPreviousPage = cursor.HasValue;
        if (isPrevious)
        {
            hasPreviousPage = items.Count == pageSize + 1;
            hasNextPage = cursor.HasValue;
        }

        if (items.Count == pageSize + 1) items.RemoveAt(items.Count - 1);
        if (isPrevious) items.Reverse(); // Reverse the items to maintain ascending order
        if (items.Count == 0) return new CursorPaginatedList<T>(items, null, null);
        var startCursor = (items.First().CreatedDate, items.First().Id);
        var lastCursor = (items.Last().CreatedDate, items.Last().Id);
        return new CursorPaginatedList<T>(items, lastCursor, startCursor, hasPreviousPage, hasNextPage);
    }
}