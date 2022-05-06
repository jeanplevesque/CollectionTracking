using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionTracking
{
	/// <summary>
	/// Represents the types of collection operations.
	/// </summary>
	public enum CollectionOperationType
	{
		/// <summary>
		/// Indicates an insertion of one or many items at a specific index in the collection.
		/// </summary>
		Insert,

		/// <summary>
		/// Indicates an deletion of one or many items at a specific index in the collection.
		/// </summary>
		Remove,

		/// <summary>
		/// Indicates a replacement of one item at a specific index in the collection.
		/// </summary>
		Replace,

		/// <summary>
		/// Indicates a movement of item from one index to another.
		/// </summary>
		Move,

		/// <summary>
		/// Indicates equality between to items.
		/// </summary>
		/// <remarks>
		/// This is only used for debugging purposes.
		/// </remarks>
		Equality
	}

	/// <summary>
	/// Represents an operation to apply on a collection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CollectionOperation<T>
	{
		/// <summary>
		/// Gets the type of operation.
		/// </summary>
		public CollectionOperationType Type { get; internal set; }

		/// <summary>
		/// Gets the items involed in the operations.
		/// </summary>
		public IList<T> Items { get; internal set; }

		/// <summary>
		/// Gets the previous item when the operation is of type <see cref="CollectionOperationType.Replace"/>.
		/// </summary>
		public T PreviousItem { get; internal set; }

		/// <summary>
		/// Gets the index indicating where in collection the operation happens.
		/// </summary>
		public int Index { get; internal set; }

		/// <summary>
		/// Gets the index indicating where in the collection a <see cref="CollectionOperationType.Move"/> operation originates from.
		/// </summary>
		public int FromIndex { get; internal set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			switch (Type)
			{
				case CollectionOperationType.Insert:
					return $"Insert '{string.Join(",", Items)}' at [{Index}]";
				case CollectionOperationType.Move:
					return $"Move '{Items[0]}' from [{FromIndex}] to [{Index}]";
				case CollectionOperationType.Remove:
					return $"Remove '{string.Join(",", Items)}' from [{Index}]";
				case CollectionOperationType.Replace:
					return $"Replace '{PreviousItem}' by '{Items[0]}' at [{Index}]";
				case CollectionOperationType.Equality:
					return $"Equality of '{Items[0]}' at [{Index}]";
			}
			return base.ToString();
		}
	}
}
