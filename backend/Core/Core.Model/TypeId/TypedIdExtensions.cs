namespace Core.Model.TypeId;

public static class TypedIdExtensions
{
    // Extension methods for nullable TypedId types
    public static string? ToInternal<T>(this StringId<T>? id)
        => id?.ToInternal();

    public static Guid? ToInternal<T>(this GuidId<T>? id)
        => id?.ToInternal();

    public static int? ToInternal<T>(this IntId<T>? id)
        => id?.ToInternal();

    // Extension methods for TypedId<TValue, TEntity>
    public static TValue? ToInternal<TValue, TEntity>(this TypedId<TValue, TEntity>? id)
        where TValue : struct
        => id?.ToInternal();
}
