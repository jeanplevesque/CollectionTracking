# CollectionTracking

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square)](LICENSE) ![Version](https://img.shields.io/nuget/v/CollectionTracking?style=flat-square) ![Downloads](https://img.shields.io/nuget/dt/CollectionTracking?style=flat-square)

## Goal
This package was developed to interpret a series of immutable lists (from a business layer) as one mutable lists (in a presentation layer).
In other words, convert `IObservable<IEnumerable<T>>` to `ObservableCollection<T>`.

## Usage

### 1. Get the differences
Use `CollectionTracker.GetOperations` to compare 2 enumerables.

```csharp
using CollectionTracking;

// (...)

var source = new[] { "B", "C", "A" };
var target = new[] { "A", "B", "C", "D" };

var operations = CollectionTracker.GetOperations(source, target);

foreach (var operation in operations)
{
	Console.WriteLine(operation);
}
```

This code prints the following.
```
Move 'A' from [2] to [0]
Insert 'D' at [3]
```

### 2. Use the differences to modify an `IList`

Use `CollectionTracker.ApplyOperations` to apply a series of `CollectionOperation`s on an `IList`.

```csharp
var list = source.ToList();
list.ApplyOperations(operations);
```

This code initializes the `list` variable with the content being [BCA].
The `ApplyOperations` modifies that `list` so that it becomes [ABCD].
This is very useful when manipulating an `ObservableCollection` bound to a UI component (such as `ItemsControl.ItemsSource`).

# CollectionTracking.DynamicData

## Goal
Expose conversion methods to relevant types from the [DynamicData library](https://github.com/reactivemarbles/DynamicData).

## Usage

```csharp
using CollectionTracking;
using DynamicData;

// (...)

var source = new[] { "B", "C", "A" };
var target = new[] { "A", "B", "C", "D" };

var operations = CollectionTracker.GetOperations(source, target);

// Convert all changes.
IChangeSet<string> changeSet = operations.ToChangeSet();

// Convert a single change.
Change<string> change = operations[0].ToChange();
```
