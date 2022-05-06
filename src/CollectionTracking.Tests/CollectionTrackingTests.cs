using CollectionTracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;

namespace CollectionTracking.Tests
{
	[TestClass]
	public class CollectionTrackingTests
	{
		[TestMethod]
		public void Move_works_with_single_item()
		{
			var source = new[] { "B", "A" };
			var target = new[] { "A", "B" };

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Move_works_with_multiple_items()
		{
			var source = new[] { "B", "C", "A" };
			var target = new[] { "A", "B", "C" };

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move);
		}

		[TestMethod]
		public void Insert_works_on_empty_lists()
		{
			var source = new string[0];
			var target = new[] { "A", "B", "C" };

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Insert, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Insert_multiple_works_at_the_start()
		{
			var source = new[] { "D", "E" };
			var target = new[] { "A", "B", "C", "D", "E" };

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Insert, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Insert_works_in_center()
		{
			var source = new[] { "A", "C" };
			var target = new[] { "A", "B", "C" };

			var operations = source.GetOperations(target).ToList();

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Insert, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Insert_multiple_works_in_the_center()
		{
			var source = new[] { "A", "E" };
			var target = new[] { "A", "B", "C", "D", "E" };

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Insert, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Insert_multiple_works_at_the_end()
		{
			var source = new[] { "A", "B" };
			var target = new[] { "A", "B", "C", "D", "E" };

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Insert, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Remove_multiple_works_at_the_start()
		{
			var source = new[] { "A", "B", "C", "D", "E" };
			var target = new[] { "C", "D", "E" };

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Remove, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Remove_multiple_works_in_the_center()
		{
			var source = new[] { "A", "B", "C", "D", "E" };
			var target = new[] { "A", "D", "E" };

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Remove, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Remove_multiple_works_at_the_end()
		{
			var source = new[] { "A", "B", "C", "D", "E" };
			var target = new[] { "A", "B", "C" };

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Remove, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Remove_multiple_works_to_empty_lists()
		{
			var source = new[] { "A", "B", "C", "D", "E" };
			var target = new string[0];

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Remove, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Move_edge_case1()
		{
			var source = "DABCEF";
			var target = "ABCDEF";

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Move_edge_case2()
		{
			var source = "BCDAEF";
			var target = "ABCDEF";

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Move_edge_case3()
		{
			var source = "BACDEF";
			var target = "ABCDEF";

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Move_edge_case4()
		{
			var source = "BA";
			var target = "AB";

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Move_edge_case5()
		{
			var source = "BCDAE";
			var target = "ABCDE";

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Move_edge_case6()
		{
			var source = "BCDFAE";
			var target = "ABCDE";

			var ops = AssertApplyOperations(source, target);
			Assert.AreEqual(2, ops.Count);
		}

		[TestMethod]
		public void Move_edge_case7()
		{
			var source = "BCAE";
			var target = "ABCDE";

			var ops = AssertApplyOperations(source, target);
			Assert.AreEqual(2, ops.Count);
		}

		[TestMethod]
		public void Move_edge_case8()
		{
			var source = "BECDFA";
			var target = "ABCDEF";

			var ops = AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move, expectedOperationCount: 2);
		}

		[TestMethod]
		public void Move_edge_case9()
		{
			var source = "FBCADE";
			var target = "ABCDEF";

			var ops = AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move, expectedOperationCount: 2);
		}

		[TestMethod]
		public void Move_edge_case10()
		{
			var source = "DBCAEF";
			var target = "ABCDEF";

			var ops = AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Move, expectedOperationCount: 2);
		}

		[TestMethod]
		public void Replace_edge_case1()
		{
			var source = "A";
			var target = "B";

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Replace, expectedOperationCount: 1);
		}

		[Ignore]
		[TestMethod]
		public void Replace_edge_case2()
		{
			var source = "ABC";
			var target = "BBC";

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Replace, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Replace_edge_case3()
		{
			var source = "ABC";
			var target = "ABD";

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Replace, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Replace_edge_case4()
		{
			var source = "ABCDEF";
			var target = "ABZDEF";

			AssertApplyOperations(source, target);
			AssertAllOperations(source, target, CollectionOperationType.Replace, expectedOperationCount: 1);
		}

		[TestMethod]
		public void Move_extension_edge_case4()
		{
			//                     012345
			const string source = "ABCDEF";

			var obs = new ObservableCollection<char>(source);
			obs.Move(1, 3);
			Assert.AreEqual("ACDBEF", new string(obs.ToArray()));

			obs = new ObservableCollection<char>(source);
			obs.Move(0, 3);
			Assert.AreEqual("BCDAEF", new string(obs.ToArray()));

			obs = new ObservableCollection<char>("AB");
			obs.Move(0, 1);
			Assert.AreEqual("BA", new string(obs.ToArray()));

			// ------

			var list = new List<char>(source);
			list.Move(1, 3);
			Assert.AreEqual("ACDBEF", new string(list.ToArray()));

			list = new List<char>(source);
			list.Move(0, 3);
			Assert.AreEqual("BCDAEF", new string(list.ToArray()));

			list = new List<char>("AB");
			list.Move(0, 1);
			Assert.AreEqual("BA", new string(list.ToArray()));
		}

		[TestMethod]
		public void LastIndexOf_works_with_regular_list()
		{
			//									   012345
			var linkedList = new LinkedList<char>("ABCDCD");

			Assert.AreEqual(5, linkedList.LastIndexOf('D'));
			Assert.AreEqual(4, linkedList.LastIndexOf('C'));
			Assert.AreEqual(1, linkedList.LastIndexOf('B'));
			Assert.AreEqual(0, linkedList.LastIndexOf('A'));
		}

		[TestMethod]
		public void LastIndexOf_works_with_single_item_list()
		{
			var linkedList = new LinkedList<char>("A");

			Assert.AreEqual(0, linkedList.LastIndexOf('A'));
		}

		[TestMethod]
		public void LastIndexOf_works_with_empty_list()
		{
			var linkedList = new LinkedList<char>("");

			Assert.AreEqual(-1, linkedList.LastIndexOf('A'));
		}

		[TestMethod]
		public void IndexOfFromTheEnd_works_with_regular_list()
		{
			//									   012345
			//									   543210
			var linkedList = new LinkedList<char>("ABCDCD");

			Assert.AreEqual(0, linkedList.IndexOfFromTheEnd('D'));
			Assert.AreEqual(1, linkedList.IndexOfFromTheEnd('C'));
			Assert.AreEqual(4, linkedList.IndexOfFromTheEnd('B'));
			Assert.AreEqual(5, linkedList.IndexOfFromTheEnd('A'));
		}

		[TestMethod]
		public void IndexOfFromTheEnd_works_with_single_item_list()
		{
			var linkedList = new LinkedList<char>("A");

			Assert.AreEqual(0, linkedList.IndexOfFromTheEnd('A'));
		}

		[TestMethod]
		public void IndexOfFromTheEnd_works_with_empty_list()
		{
			var linkedList = new LinkedList<char>("");

			Assert.AreEqual(-1, linkedList.IndexOfFromTheEnd('A'));
		}

		[TestMethod]
		public void CustomTest()
		{
			var source = "SUTACAGMQ";
			var target = "TUYOQAXSY";

			//source.GetDamerauLevenshteinDistance(target);

			var ops = AssertApplyOperations(source, target);
		}

		[TestMethod]
		public void Algorithm_works_random_lists()
		{
			const int iterationCount = 10000;
			const string dataSource = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			int[] maxLengths = new int[] { 3, 5, 6, 8, 10, 13, 17, 20, 25, 30, 50, 1000 };

			for (int i = 0; i < maxLengths.Length; i++)
			{
				ShuffleTest(iterationCount, maxLengths[i], dataSource, CollectionOperationOptions.Default);
				ShuffleTest(iterationCount, maxLengths[i], dataSource, CollectionOperationOptions.NoReplace);
				ShuffleTest(iterationCount, maxLengths[i], dataSource, CollectionOperationOptions.NoMove);
				ShuffleTest(iterationCount, maxLengths[i], dataSource, CollectionOperationOptions.NoMove | CollectionOperationOptions.NoReplace);
			}
		}

		private void ShuffleTest(int iterationCount, int maxLength, string dataSource, CollectionOperationOptions options)
		{
			var totalOperations = 0;
			var totalTicks = 0L;
			Parallel.For(0, iterationCount, i =>
			{
				var random = new Random(i);
				var source = new string(Enumerable.Range(0, random.Next(maxLength)).Select(_ => dataSource[random.Next(0, dataSource.Length - 1)]).ToArray());
				var target = new string(Enumerable.Range(0, random.Next(maxLength)).Select(_ => dataSource[random.Next(0, dataSource.Length - 1)]).ToArray());

				var chrono = Stopwatch.StartNew();
				var operations = AssertApplyOperations(source, target, options);
				chrono.Stop();
				var time = chrono.ElapsedTicks;
				Interlocked.Add(ref totalOperations, operations.Count);
				Interlocked.Add(ref totalTicks, time);
			});

			var averageOperationCount = totalOperations / (double)iterationCount;
			var averageTicks = totalTicks / (double)iterationCount;
			Debug.WriteLine($"ShuffleTest {options,-17} maxLength: {maxLength}, avg ops: {averageOperationCount:N2}, avg ticks: {averageTicks:N0}");
		}

		[TestMethod]
		public void Algorithm_works_fast_on_big_lists_will_small_diff()
		{
			const int iterationCount = 100;
			const string dataSource = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			int[] maxLengths = new int[] { 1000, 10000, 100000 };

			for (int i = 0; i < maxLengths.Length; i++)
			{
				SmallDiffTest(iterationCount, maxLengths[i], 3, dataSource, CollectionOperationOptions.Default);
				SmallDiffTest(iterationCount, maxLengths[i], 3, dataSource, CollectionOperationOptions.NoReplace);
				SmallDiffTest(iterationCount, maxLengths[i], 3, dataSource, CollectionOperationOptions.NoMove);
				SmallDiffTest(iterationCount, maxLengths[i], 3, dataSource, CollectionOperationOptions.NoMove | CollectionOperationOptions.NoReplace);
			}
		}

		private void SmallDiffTest(int iterationCount, int maxLength, int maxDifferences, string dataSource, CollectionOperationOptions options)
		{
			var totalOperations = 0;
			var totalTicks = 0L;
			Parallel.For(0, iterationCount, i =>
			{
				var random = new Random(i);
				var source = new string(Enumerable.Range(0, random.Next(maxLength)).Select(_ => dataSource[random.Next(0, dataSource.Length - 1)]).ToArray());

				var target = source.ToList();

				for (int d = 0; d < maxDifferences; d++)
				{
					var operationType = (CollectionOperationType)random.Next(0, 4);
					switch (operationType)
					{
						case CollectionOperationType.Insert:
							var index = random.Next(0, target.Count);
							var item = dataSource[random.Next(0, dataSource.Length - 1)];
							target.Insert(index, item);
							break;
						case CollectionOperationType.Remove:
							if (target.Count > 0)
							{
								index = random.Next(0, target.Count - 1);
								target.RemoveAt(index);
							}
							break;
						case CollectionOperationType.Replace:
							if (target.Count > 0)
							{
								index = random.Next(0, target.Count - 1);
								item = dataSource[random.Next(0, dataSource.Length - 1)];
								target.Insert(index, item);
								target.RemoveAt(index + 1);
							}
							break;
						case CollectionOperationType.Move:
							if (target.Count > 0)
							{
								var fromIndex = random.Next(0, target.Count - 1);
								index = random.Next(0, target.Count - 1);
								target.Move(fromIndex, index);
							}
							break;
					}
				}

				var chrono = Stopwatch.StartNew();
				var operations = AssertApplyOperations(source, target, options);
				chrono.Stop();
				var time = chrono.ElapsedTicks;
				Interlocked.Add(ref totalOperations, operations.Count);
				Interlocked.Add(ref totalTicks, time);
			});

			var averageOperationCount = totalOperations / (double)iterationCount;
			var averageTicks = totalTicks / (double)iterationCount;
			Debug.WriteLine($"SmallDiffTest {options,-17} maxLength: {maxLength}, avg ops: {averageOperationCount:N2}, avg ticks: {averageTicks:N0}");
		}


		private List<CollectionOperation<T>> AssertApplyOperations<T>(IEnumerable<T> source, IEnumerable<T> target, CollectionOperationOptions options = CollectionOperationOptions.Default)
		{
			var operations = source.GetOperations(target, options: options).ToList();

			if (options.HasFlag(CollectionOperationOptions.NoReplace))
			{
				Assert.IsFalse(operations.Any(op => op.Type == CollectionOperationType.Replace));
			}

			if (options.HasFlag(CollectionOperationOptions.NoMove))
			{
				Assert.IsFalse(operations.Any(op => op.Type == CollectionOperationType.Move));
			}

			var sourceList = source.ToList();
			sourceList.ApplyOperations(operations);

			Assert.IsTrue(target.SequenceEqual(sourceList));

			return operations;
		}

		private void AssertAllOperations<T>(IEnumerable<T> source, IEnumerable<T> target, CollectionOperationType expectedType, int? expectedOperationCount = null)
		{
			var operations = source.GetOperations(target).ToList();

			Assert.IsTrue(operations.All(o => o.Type == expectedType));
			if (expectedOperationCount.HasValue)
			{
				Assert.AreEqual(expectedOperationCount, operations.Count);
			}
		}
	}
}
