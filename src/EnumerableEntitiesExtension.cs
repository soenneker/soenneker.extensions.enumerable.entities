using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Soenneker.Entities.Entity.Abstract;

namespace Soenneker.Extensions.Enumerable.Entities;

/// <summary>
/// A collection of helpful IEnumerable Entities extension methods
/// </summary>
public static class EnumerableEntitiesExtension
{
    /// <summary>
    /// Returns a list of ids from an enumerable of documents.
    /// </summary>
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
}
