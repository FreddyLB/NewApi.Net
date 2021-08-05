using System.Collections;

namespace Api.Models
{
    /// <summary>
    /// Represents the data returned from a request.
    /// </summary>
    public record ListResult(int Count, IEnumerable Data)
    {
        /// <summary>
        /// Number of elements available.
        /// </summary>
        public int Count { get; init; } = Count;

        /// <summary>
        /// An enumerable over the resulting elements.
        /// </summary>
        public IEnumerable Data { get; init; } = Data;
    }
}
