using Core.Model.Delta;

namespace Core.Model.Test.Delta;

public class PatchableArrayTests
{
    private record TestItem(int Id, string Name);

    [Fact]
    public void Replace_CreatesCorrectPatch()
    {
        var items = new List<TestItem>
        {
            new(1, "Item1"),
            new(2, "Item2")
        };

        var patch = PatchableArray<TestItem, int>.Replace(items);

        Assert.Equal(PatchableArrayOperation.Replace, patch.Operation);
        Assert.Equal(items, patch.Items);
        Assert.Null(patch.Index);
        Assert.Null(patch.Item);
        Assert.Null(patch.ItemId);
        Assert.Null(patch.Count);
        Assert.Null(patch.ToIndex);
    }

    [Fact]
    public void Add_WithoutIndex_CreatesCorrectPatch()
    {
        var item = new TestItem(1, "Item1");

        var patch = PatchableArray<TestItem, int>.Add(item);

        Assert.Equal(PatchableArrayOperation.Add, patch.Operation);
        Assert.Equal(item, patch.Item);
        Assert.Null(patch.Index);
        Assert.Null(patch.Items);
        Assert.Null(patch.ItemId);
        Assert.Null(patch.Count);
        Assert.Null(patch.ToIndex);
    }

    [Fact]
    public void Add_WithIndex_CreatesCorrectPatch()
    {
        var item = new TestItem(1, "Item1");
        var index = 2;

        var patch = PatchableArray<TestItem, int>.Add(item, index);

        Assert.Equal(PatchableArrayOperation.Add, patch.Operation);
        Assert.Equal(item, patch.Item);
        Assert.Equal(index, patch.Index);
        Assert.Null(patch.Items);
        Assert.Null(patch.ItemId);
        Assert.Null(patch.Count);
        Assert.Null(patch.ToIndex);
    }

    [Fact]
    public void AddRange_WithoutIndex_CreatesCorrectPatch()
    {
        var items = new List<TestItem>
        {
            new(1, "Item1"),
            new(2, "Item2")
        };

        var patch = PatchableArray<TestItem, int>.AddRange(items);

        Assert.Equal(PatchableArrayOperation.AddRange, patch.Operation);
        Assert.Equal(items, patch.Items);
        Assert.Null(patch.Index);
        Assert.Null(patch.Item);
        Assert.Null(patch.ItemId);
        Assert.Null(patch.Count);
        Assert.Null(patch.ToIndex);
    }

    [Fact]
    public void AddRange_WithIndex_CreatesCorrectPatch()
    {
        var items = new List<TestItem>
        {
            new(1, "Item1"),
            new(2, "Item2")
        };
        var index = 1;

        var patch = PatchableArray<TestItem, int>.AddRange(items, index);

        Assert.Equal(PatchableArrayOperation.AddRange, patch.Operation);
        Assert.Equal(items, patch.Items);
        Assert.Equal(index, patch.Index);
        Assert.Null(patch.Item);
        Assert.Null(patch.ItemId);
        Assert.Null(patch.Count);
        Assert.Null(patch.ToIndex);
    }

    [Fact]
    public void Remove_CreatesCorrectPatch()
    {
        var index = 3;

        var patch = PatchableArray<TestItem, int>.Remove(index);

        Assert.Equal(PatchableArrayOperation.Remove, patch.Operation);
        Assert.Equal(index, patch.Index);
        Assert.Null(patch.Item);
        Assert.Null(patch.Items);
        Assert.Null(patch.ItemId);
        Assert.Null(patch.Count);
        Assert.Null(patch.ToIndex);
    }

    [Fact]
    public void RemoveById_CreatesCorrectPatch()
    {
        var itemId = 42;

        var patch = PatchableArray<TestItem, int>.RemoveById(itemId);

        Assert.Equal(PatchableArrayOperation.RemoveById, patch.Operation);
        Assert.Equal(itemId, patch.ItemId);
        Assert.Null(patch.Index);
        Assert.Null(patch.Item);
        Assert.Null(patch.Items);
        Assert.Null(patch.Count);
        Assert.Null(patch.ToIndex);
    }

    [Fact]
    public void RemoveRange_CreatesCorrectPatch()
    {
        var index = 1;
        var count = 3;

        var patch = PatchableArray<TestItem, int>.RemoveRange(index, count);

        Assert.Equal(PatchableArrayOperation.RemoveRange, patch.Operation);
        Assert.Equal(index, patch.Index);
        Assert.Equal(count, patch.Count);
        Assert.Null(patch.Item);
        Assert.Null(patch.Items);
        Assert.Null(patch.ItemId);
        Assert.Null(patch.ToIndex);
    }

    [Fact]
    public void Move_CreatesCorrectPatch()
    {
        var fromIndex = 2;
        var toIndex = 5;

        var patch = PatchableArray<TestItem, int>.Move(fromIndex, toIndex);

        Assert.Equal(PatchableArrayOperation.Move, patch.Operation);
        Assert.Equal(fromIndex, patch.Index);
        Assert.Equal(toIndex, patch.ToIndex);
        Assert.Null(patch.Item);
        Assert.Null(patch.Items);
        Assert.Null(patch.ItemId);
        Assert.Null(patch.Count);
    }

    [Fact]
    public void MoveById_CreatesCorrectPatch()
    {
        var itemId = 42;
        var toIndex = 3;

        var patch = PatchableArray<TestItem, int>.MoveById(itemId, toIndex);

        Assert.Equal(PatchableArrayOperation.MoveById, patch.Operation);
        Assert.Equal(itemId, patch.ItemId);
        Assert.Equal(toIndex, patch.ToIndex);
        Assert.Null(patch.Index);
        Assert.Null(patch.Item);
        Assert.Null(patch.Items);
        Assert.Null(patch.Count);
    }

    [Fact]
    public void Clear_CreatesCorrectPatch()
    {
        var patch = PatchableArray<TestItem, int>.Clear();

        Assert.Equal(PatchableArrayOperation.Clear, patch.Operation);
        Assert.Null(patch.Index);
        Assert.Null(patch.Item);
        Assert.Null(patch.Items);
        Assert.Null(patch.ItemId);
        Assert.Null(patch.Count);
        Assert.Null(patch.ToIndex);
    }

    [Fact]
    public void PatchableArray_SupportsStructTypeIds()
    {
        // Test with int
        var intPatch = PatchableArray<string, int>.RemoveById(42);
        Assert.Equal(42, intPatch.ItemId);

        // Test with Guid
        var guid = Guid.NewGuid();
        var guidPatch = PatchableArray<string, Guid>.RemoveById(guid);
        Assert.Equal(guid, guidPatch.ItemId);
    }
}
