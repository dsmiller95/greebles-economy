using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class LinqExtensions
{
    public static IEnumerable<IList<T>> RollingWindow<T>(this IEnumerable<T> source, int window)
    {
        var iterator = source.GetEnumerator();

        var position = 0;
        var workingList = new List<T>(window);
        while (position < window && iterator.MoveNext())
        {
            var value = iterator.Current;
            workingList.Add(value);
            position++;
        }
        if(position < window)
        {
            yield break;
        }

        yield return workingList.ToList();
        while (iterator.MoveNext())
        {
            var value = iterator.Current;
            workingList.RemoveAt(0);
            workingList.Add(value);
            yield return workingList.ToList();
        }
    }
}

