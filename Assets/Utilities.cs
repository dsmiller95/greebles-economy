using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Utilities
{
    public static string SerializeDictionary<T>(Dictionary<ResourceType, T> dictionary, Func<T, string> serializer = null)
    {
        serializer = serializer ?? (s => s.ToString());
        return dictionary.Select(entry => $"{Enum.GetName(typeof(ResourceType), entry.Key)}: {serializer(entry.Value)}").Aggregate((agg, current) => agg + "; " + current);
    }

    /// <summary>
    /// create or destroy objects based on how many are needed in the list
    /// </summary>
    /// <param name="neededObjects"></param>
    /// <param name="objectList">the list of objects to modify</param>
    /// <param name="objectGenerator">a generator function to create new objects</param>
    /// <param name="objectDestroyer">a function to be called when an object is removed</param>
    public static IList<T> EnsureAllObjectsCreated<T>(int neededObjects, IList<T> objectList, Func<T> objectGenerator, Action<T> objectDestroyer)
    {
        if (objectList.Count > neededObjects)
        {
            foreach (var objectToDelete in objectList.Skip(neededObjects))
            {
                objectDestroyer(objectToDelete);
            }
            return objectList.Take(neededObjects).ToList();
        }

        if (objectList.Count < neededObjects)
        {
            for (int dotToAdd = objectList.Count; dotToAdd < neededObjects; dotToAdd++)
            {
                objectList.Add(objectGenerator());
            }
        }
        return objectList;
    }
}
