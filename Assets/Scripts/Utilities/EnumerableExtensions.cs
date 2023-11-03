using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumerableExtensions
{
	public static T GetRandom<T>(this IEnumerable<T> source)
	{
		T[] array = source.ToArray();
		return array[Random.Range(0, array.Length)];
	}
	public static T GetRandom<T>(this IEnumerable<T> source, System.Func<T, bool> predicate)
	{
		return source.Where(predicate).GetRandom();
	}
}