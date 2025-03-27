using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Soenneker.Entities.Entity.Abstract;

namespace Soenneker.Extensions.Enumerable.Entities;

/// <summary>
/// A collection of helpful extension methods for working with <see cref="IEnumerable{T}"/> sequences of entities implementing <see cref="IEntity"/>.
/// </summary>
public static class EnumerableEntitiesExtension
{
    /// <summary>
    /// Projects a sequence of entities into a list of their <c>Id</c> values.
    /// </summary>
    /// <typeparam name="T">The entity type implementing <see cref="IEntity"/>.</typeparam>
    /// <param name="value">The enumerable collection of entities to extract IDs from.</param>
    /// <returns>
    /// A list of entity IDs. If <paramref name="value"/> is <c>null</c>, an empty list is returned.
    /// </returns>
    /// <remarks>
    /// This method is optimized for performance:
    /// <list type="bullet">
    ///   <item><description>Uses <see cref="ICollection{T}.Count"/> to preallocate list capacity when available.</description></item>
    ///   <item><description>Uses index-based iteration for <see cref="IList{T}"/> to minimize enumerator overhead.</description></item>
    ///   <item><description>Materializes a list of strings with no deferred execution or LINQ allocations.</description></item>
    /// </list>
    /// </remarks>
    [Pure]
    public static List<string> ToIds<T>(this IEnumerable<T> value) where T : IEntity
    {
        switch (value)
        {
            case null:
                return [];

            case ICollection<T> collection:
                {
                    var result = new List<string>(collection.Count);
                    // Use indexer if available to avoid iterator allocation
                    if (collection is IList<T> list)
                    {
                        for (var i = 0; i < list.Count; i++)
                        {
                            result.Add(list[i].Id);
                        }
                    }
                    else
                    {
                        foreach (T doc in collection)
                        {
                            result.Add(doc.Id);
                        }
                    }

                    return result;
                }

            default:
                {
                    // Avoid multiple enumerator allocations if possible
                    var result = new List<string>();
                    using IEnumerator<T> enumerator = value.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        result.Add(enumerator.Current.Id);
                    }

                    return result;
                }
        }
    }

    /// <summary>
    /// Determines whether the specified enumerable contains an entity with the given <paramref name="id"/>.
    /// </summary>
    /// <typeparam name="T">The entity type implementing <see cref="IEntity"/>.</typeparam>
    /// <param name="entityEnumerable">The enumerable collection of entities to search.</param>
    /// <param name="id">The ID to match against entity <c>Id</c> values.</param>
    /// <returns>
    /// <c>true</c> if any entity in the sequence has a matching ID; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method is optimized to avoid allocations and minimize overhead:
    /// <list type="bullet">
    ///   <item><description>Handles <see cref="IList{T}"/> and <see cref="ICollection{T}"/> for efficient traversal.</description></item>
    ///   <item><description>Avoids LINQ and lambda expressions to reduce memory usage and increase speed.</description></item>
    ///   <item><description>Short-circuits on the first matching ID for optimal performance.</description></item>
    /// </list>
    /// </remarks>
    [Pure]
    public static bool ContainsId<T>(this IEnumerable<T> entityEnumerable, string id) where T : IEntity
    {
        if (entityEnumerable.IsNullOrEmpty())
            return false;

        switch (entityEnumerable)
        {
            case IList<T> list:
                for (var i = 0; i < list.Count; i++)
                {
                    if (list[i].Id == id)
                        return true;
                }

                break;

            case ICollection<T> collection:
                foreach (T item in collection)
                {
                    if (item.Id == id)
                        return true;
                }

                break;

            default:
                {
                    using IEnumerator<T> enumerator = entityEnumerable.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Id == id)
                            return true;
                    }

                    break;
                }
        }

        return false;
    }
}
