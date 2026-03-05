using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Model.TypeId;

[JsonConverter(typeof(TypedIdJsonConverterFactory))]
public readonly struct IntId<TEntity> : IEquatable<IntId<TEntity>>, ITypedId<int>
{
    internal int Value { get; }

    public IntId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Int Id must be greater than zero", nameof(value));

        Value = value;
    }

    public static IntId<TEntity> Parse(string value) => new(int.Parse(value));
    public static IntId<TEntity> Parse(int value) => new(value);

    public static bool TryParse(string? value, out IntId<TEntity> result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default;
            return false;
        }

        if (int.TryParse(value, out var intValue) && intValue > 0)
        {
            result = new IntId<TEntity>(intValue);
            return true;
        }

        result = default;
        return false;
    }

    public int ToInternal() => Value;

    public override string ToString() => Value.ToString();
    public override int GetHashCode() => Value.GetHashCode();
    public override bool Equals(object? obj) => obj is IntId<TEntity> other && Equals(other);
    public bool Equals(IntId<TEntity> other) => Value == other.Value;

    public static bool operator ==(IntId<TEntity> left, IntId<TEntity> right) => left.Equals(right);
    public static bool operator !=(IntId<TEntity> left, IntId<TEntity> right) => !left.Equals(right);

    public static explicit operator int(IntId<TEntity> id) => id.Value;
    public static explicit operator IntId<TEntity>(int value) => new(value);
}

public class IntIdJsonConverter<T> : JsonConverter<IntId<T>>
{
    public override IntId<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var value = reader.GetInt32();
            return IntId<T>.Parse(value);
        }

        var str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str))
            return default;

        return IntId<T>.Parse(str);
    }

    public override void Write(Utf8JsonWriter writer, IntId<T> value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
