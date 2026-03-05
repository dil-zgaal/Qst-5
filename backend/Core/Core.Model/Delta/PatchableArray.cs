using System.Text.Json.Serialization;

namespace Core.Model.Delta;

public class PatchableArray<T, TId> where TId : struct
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PatchableArrayOperation Operation { get; set; }

    public int? Index { get; set; }
    public T? Item { get; set; }
    public List<T>? Items { get; set; }
    public TId? ItemId { get; set; }
    public int? Count { get; set; }
    public int? ToIndex { get; set; }

    public static PatchableArray<T, TId> Replace(List<T> items) => new()
    {
        Operation = PatchableArrayOperation.Replace,
        Items = items
    };

    public static PatchableArray<T, TId> Add(T item, int? index = null) => new()
    {
        Operation = PatchableArrayOperation.Add,
        Item = item,
        Index = index
    };

    public static PatchableArray<T, TId> AddRange(List<T> items, int? index = null) => new()
    {
        Operation = PatchableArrayOperation.AddRange,
        Items = items,
        Index = index
    };

    public static PatchableArray<T, TId> Remove(int index) => new()
    {
        Operation = PatchableArrayOperation.Remove,
        Index = index
    };

    public static PatchableArray<T, TId> RemoveById(TId itemId) => new()
    {
        Operation = PatchableArrayOperation.RemoveById,
        ItemId = itemId
    };

    public static PatchableArray<T, TId> RemoveRange(int index, int count) => new()
    {
        Operation = PatchableArrayOperation.RemoveRange,
        Index = index,
        Count = count
    };

    public static PatchableArray<T, TId> Move(int fromIndex, int toIndex) => new()
    {
        Operation = PatchableArrayOperation.Move,
        Index = fromIndex,
        ToIndex = toIndex
    };

    public static PatchableArray<T, TId> MoveById(TId itemId, int toIndex) => new()
    {
        Operation = PatchableArrayOperation.MoveById,
        ItemId = itemId,
        ToIndex = toIndex
    };

    public static PatchableArray<T, TId> Clear() => new()
    {
        Operation = PatchableArrayOperation.Clear
    };
}

public enum PatchableArrayOperation
{
    Replace,      // Replace entire array
    Add,          // Add single item at index (or end if null)
    AddRange,     // Add multiple items at index (or end if null)
    Remove,       // Remove item at index
    RemoveById,   // Remove item by ID (for items with Id property)
    RemoveRange,  // Remove range of items
    Move,         // Move item from index to index
    MoveById,     // Move item by ID to index
    Clear         // Clear all items
}
