using Soenneker.Entities.Entity.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

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
    public static List<string> ToIds<T>(this IEnumerable<T>? value) where T : IEntity
    {
        if (value is null)
            return [];

        // Fast paths (no enumerator boxing, known counts)
        if (value is List<T> list)
        {
            var result = new List<string>(list.Count);
            for (var i = 0; i < list.Count; i++)
                result.Add(list[i].Id);
            return result;
        }

        if (value is T[] array)
        {
            var result = new List<string>(array.Length);
            for (var i = 0; i < array.Length; i++)
                result.Add(array[i].Id);
            return result;
        }

        if (value is IReadOnlyList<T> roList)
        {
            var result = new List<string>(roList.Count);
            for (var i = 0; i < roList.Count; i++)
                result.Add(roList[i].Id);
            return result;
        }

        // Pre-size if we can get a count without enumeration
        if (value is ICollection<T> collection)
        {
            var result = new List<string>(collection.Count);

            // Some ICollection<T> are also IList<T>; for-loop avoids foreach over interface
            if (collection is IList<T> ilist)
            {
                for (var i = 0; i < ilist.Count; i++)
                    result.Add(ilist[i].Id);
            }
            else
            {
                foreach (T item in collection)
                    result.Add(item.Id);
            }

            return result;
        }

        // Default: try to pre-size without enumerating; otherwise grow as needed
        if (System.Linq.Enumerable.TryGetNonEnumeratedCount(value, out int count) && count > 0)
        {
            var result = new List<string>(count);
            foreach (T item in value)
                result.Add(item.Id);
            return result;
        }
        else
        {
            var result = new List<string>();
            foreach (T item in value)
                result.Add(item.Id);
            return result;
        }
    }

    /// <summary>
    /// Determines whether the sequence contains an entity with the specified identifier.
    /// </summary>
    /// <remarks>Returns false if either the sequence or the identifier is null or empty. The method performs
    /// an ordinal, case-sensitive comparison of the Id property of each entity.</remarks>
    /// <typeparam name="T">The type of entity in the sequence. Must implement the IEntity interface.</typeparam>
    /// <param name="entityEnumerable">The sequence of entities to search. Can be null.</param>
    /// <param name="id">The identifier to locate within the sequence. The comparison is case-sensitive and uses ordinal comparison. Can
    /// be null.</param>
    /// <returns>true if an entity with the specified identifier exists in the sequence; otherwise, false.</returns>
    [Pure]
    public static bool ContainsId<T>(this IEnumerable<T>? entityEnumerable, string? id) where T : IEntity
    {
        if (entityEnumerable is null || string.IsNullOrEmpty(id))
            return false;

        // Cheap emptiness check without enumerating when possible
        if (entityEnumerable is ICollection<T> c && c.Count == 0)
            return false;

        if (entityEnumerable is List<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (string.Equals(list[i].Id, id, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        // Fast paths
        if (entityEnumerable is T[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (string.Equals(array[i].Id, id, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        if (entityEnumerable is IReadOnlyList<T> roList)
        {
            for (var i = 0; i < roList.Count; i++)
            {
                if (string.Equals(roList[i].Id, id, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        if (entityEnumerable is IList<T> ilist)
        {
            for (var i = 0; i < ilist.Count; i++)
            {
                if (string.Equals(ilist[i].Id, id, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        // Default
        foreach (T item in entityEnumerable)
        {
            if (string.Equals(item.Id, id, StringComparison.Ordinal))
                return true;
        }

        return false;
    }
}