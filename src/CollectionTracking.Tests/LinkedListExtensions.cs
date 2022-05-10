using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionTracking.Tests
{
	public static class LinkedListExtensions
	{
		public static int LastIndexOf<T>(this LinkedList<T> linkedList, T item, IEqualityComparer<T>? comparer = null)
		{
			comparer ??= EqualityComparer<T>.Default;

			var node = linkedList.Last;
			for (int i = linkedList.Count - 1; i >= 0; --i)
			{
				if (comparer.Equals(node.Value, item))
				{
					return i;
				}
				node = node.Previous;
			}
			return -1;
		}

		/// <summary>
		/// Index from the end is 0 if the item is at the last position.
		/// </summary>
		public static int IndexOfFromTheEnd<T>(this LinkedList<T> linkedList, T item, IEqualityComparer<T>? comparer = null)
		{
			comparer ??= EqualityComparer<T>.Default;

			var node = linkedList.Last;
			for (int i = 0; i < linkedList.Count; i++)
			{
				if (comparer.Equals(node.Value, item))
				{
					return i;
				}
				node = node.Previous;
			}

			return -1;
		}
	}
}
