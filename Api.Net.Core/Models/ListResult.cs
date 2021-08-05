using System.Collections;

namespace Api.Models
{
    /// <summary>
    /// Represents the data returned from a request.
    /// </summary>
    public record ListResult(int Count, IEnumerable Data);
}
