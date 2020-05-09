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
        return dictionary.Select(entry => $"Type: {Enum.GetName(typeof(ResourceType), entry.Key)}\t Value: {serializer(entry.Value)}").Aggregate((agg, current) => agg + '\n' + current);
    }
}
