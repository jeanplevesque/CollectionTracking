using DynamicData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CollectionTracking;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace CollectionTracking.Tests
{
	[TestClass]
	public class DynamicDataTests
	{
		[TestMethod]
		public void CollectionOperations_as_ChangeSet_produce_the_same_result()
		{
			var random = new Random(0);
			const int MaxLength = 30;
			const string dataSource = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			Subject<string> subject = new Subject<string>();
			IObservable<IEnumerable<char>> observableOfList = subject;
			IObservable<IChangeSet<char>> observableOfChangeSet = observableOfList
				.StartWith(new char[0])
				.Buffer(2, 1)
				.Select(list => list[0].GetOperations(list[1]).ToChangeSet());

			observableOfChangeSet
				.Bind(out ReadOnlyObservableCollection<char> boundCollection)
				.Subscribe();

			((INotifyCollectionChanged)boundCollection).CollectionChanged += OnCollectionChanged;

			for (int i = 0; i < 1000; i++)
			{
				var previous = string.Join("", boundCollection);
				var target = new string(Enumerable.Range(0, random.Next(MaxLength)).Select(_ => dataSource[random.Next(0, dataSource.Length - 1)]).ToArray());
				subject.OnNext(target);

				// Compare using My method.
				var ops = previous.GetOperations(target);
				var list = previous.ToList();
				list.ApplyOperations(ops);
				var current2 = string.Join("", list);
				Assert.AreEqual(target, current2);

				CompareEachChange(previous, target);

				// Compare the DynamicData method.
				var current = string.Join("", boundCollection);
				Assert.AreEqual(target, current);
			}

			void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				//Debug.WriteLine($"[{string.Join(", ", (IEnumerable<char>)sender)}] (operation: {e.Action} at index {e.NewStartingIndex})");
			}
		}

		private void CompareEachChange(string previous, string target)
		{
			var ops = previous.GetOperations(target);
			var ops2 = previous.GetOperations(target, options: CollectionOperationOptions.NoMove | CollectionOperationOptions.NoReplace);
			
			var list1 = previous.ToList();
			var list2 = previous.ToList();

			foreach (var op in ops)
			{
				var p = string.Join("", list1);

				list1.ApplyOperation(op);
				var change = op.ToChange();
				list2.Clone(new ChangeSet<char>(new[] { change }));

				var s1 = string.Join("", list1);
				var s2 = string.Join("", list2);
				Assert.AreEqual(s1, s2);
			}
		}
	}
}
