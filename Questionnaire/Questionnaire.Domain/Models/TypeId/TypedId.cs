using System.Text.Json;
using System.Text.Json.Serialization;

namespace Questionnaire.Models.TypeId;

public interface ITypedId<out TValue>
{
    TValue ToInternal();
}

[JsonConverter(typeof(TypedIdJsonConverterFactory))]
public readonly struct TypedId<TValue, TEntity> : IEquatable<TypedId<TValue, TEntity>>, ITypedId<TValue>
    where TValue : notnull
{
    internal TValue Value { get; }

    public TypedId(TValue value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (value is string str && string.IsNullOrWhiteSpace(str))
            throw new ArgumentException("String Id cannot be empty", nameof(value));

        Value = value;
    }

    public TValue ToInternal() => Value;

    public override string ToString() => Value.ToString() ?? string.Empty;
    public override int GetHashCode() => Value.GetHashCode();
    public override bool Equals(object? obj) => obj is TypedId<TValue, TEntity> other && Equals(other);
    public bool Equals(TypedId<TValue, TEntity> other) => EqualityComparer<TValue>.Default.Equals(Value, other.Value);

    public static bool operator ==(TypedId<TValue, TEntity> left, TypedId<TValue, TEntity> right) => left.Equals(right);
    public static bool operator !=(TypedId<TValue, TEntity> left, TypedId<TValue, TEntity> right) => !left.Equals(right);

    public static explicit operator TValue(TypedId<TValue, TEntity> id) => id.Value;
}

// JSON converter for TypedId
public class TypedIdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        var genericDef = typeToConvert.GetGenericTypeDefinition();
        return genericDef == typeof(TypedId<,>) ||
               genericDef == typeof(StringId<>) ||
               genericDef == typeof(GuidId<>) ||
               genericDef == typeof(IntId<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var genericDef = typeToConvert.GetGenericTypeDefinition();

        if (genericDef == typeof(StringId<>))
        {
            Type entityType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(StringIdJsonConverter<>).MakeGenericType(entityType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        if (genericDef == typeof(GuidId<>))
        {
            Type entityType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(GuidIdJsonConverter<>).MakeGenericType(entityType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        if (genericDef == typeof(IntId<>))
        {
            Type entityType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(IntIdJsonConverter<>).MakeGenericType(entityType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        // Generic TypedId<TValue, TEntity>
        Type[] typeArgs = typeToConvert.GetGenericArguments();
        Type valueType = typeArgs[0];
        Type entityType2 = typeArgs[1];

        if (valueType == typeof(string))
        {
            Type converterType = typeof(StringIdJsonConverter<>).MakeGenericType(entityType2);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
        else if (valueType == typeof(Guid))
        {
            Type converterType = typeof(GuidIdJsonConverter<>).MakeGenericType(entityType2);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
        else if (valueType == typeof(int))
        {
            Type converterType = typeof(IntIdJsonConverter<>).MakeGenericType(entityType2);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        throw new NotSupportedException($"TypedId with value type {valueType} is not supported");
    }
}
