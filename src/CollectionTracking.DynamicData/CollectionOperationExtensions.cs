using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionTracking
{
	/// <summary>
	/// This class exposes extension methods on <see cref="CollectionOperation{T}"/> and on enumerables of said class.
	/// </summary>
	public static class CollectionOperationExtensions
	{
		/// <summary>
		/// Creates a <see cref="IChangeSet{T}"/> from a series of <see cref="CollectionOperation{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="collectionOperations">The collection operations.</param>
		/// <returns>A <see cref="IChangeSet{T}"/> equivalent to the provided <paramref name="collectionOperations"/>.</returns>
		public static IChangeSet<T> ToChangeSet<T>(this IEnumerable<CollectionOperation<T>> collectionOperations)
		{
			return new ChangeSet<T>(collectionOperations.Select(co => co.ToChange()));
		}

		/// <summary>
		/// Creates a <see cref="Change{T}"/> from a <see cref="CollectionOperation{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="collectionOperation">The collection operation.</param>
		/// <returns>A <see cref="Change{T}"/> equivalent to the provided <paramref name="collectionOperation"/>.</returns>
		public static Change<T> ToChange<T>(this CollectionOperation<T> collectionOperation)
		{
			switch (collectionOperation.Type)
			{
				case CollectionOperationType.Insert:
					if (collectionOperation.Items.Count > 1)
					{
						return new Change<T>(ListChangeReason.AddRange, collectionOperation.Items, collectionOperation.Index);
					}
					else
					{
						return new Change<T>(ListChangeReason.Add, collectionOperation.Items.First(), collectionOperation.Index);
					}
				case CollectionOperationType.Remove:
					if (collectionOperation.Items.Count > 1)
					{
						return new Change<T>(ListChangeReason.RemoveRange, collectionOperation.Items, collectionOperation.Index);
					}
					else
					{
						return new Change<T>(ListChangeReason.Remove, collectionOperation.Items.First(), collectionOperation.Index);
					}
				case CollectionOperationType.Move:
					return new Change<T>(collectionOperation.Items.First(), collectionOperation.Index, collectionOperation.FromIndex);
				case CollectionOperationType.Replace:
					return new Change<T>(ListChangeReason.Replace, collectionOperation.Items.First(), collectionOperation.PreviousItem, collectionOperation.Index, collectionOperation.Index);
				default:
					throw new NotSupportedException();
			}
		}
	}
}
