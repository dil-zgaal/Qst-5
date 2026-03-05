using System.Text.Json;
using System.Text.Json.Serialization;

namespace Questionnaire.Models.TypeId;

[JsonConverter(typeof(TypedIdJsonConverterFactory))]
public readonly struct StringId<TEntity> : IEquatable<StringId<TEntity>>, ITypedId<string>
{
    internal string Value { get; }

    public StringId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("String Id cannot be null or empty", nameof(value));

        Value = value;
    }

    public static StringId<TEntity> New() => new(Guid.NewGuid().ToString());
    public static StringId<TEntity> Parse(string value) => new(value);

    public static bool TryParse(string? value, out StringId<TEntity> result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default;
            return false;
        }

        try
        {
            result = new StringId<TEntity>(value);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    public string ToInternal() => Value;

    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override bool Equals(object? obj) => obj is StringId<TEntity> other && Equals(other);
    public bool Equals(StringId<TEntity> other) => Value == other.Value;

    public static bool operator ==(StringId<TEntity> left, StringId<TEntity> right) => left.Equals(right);
    public static bool operator !=(StringId<TEntity> left, StringId<TEntity> right) => !left.Equals(right);

    public static explicit operator string(StringId<TEntity> id) => id.Value;
    public static explicit operator StringId<TEntity>(string value) => new(value);
}

public class StringIdJsonConverter<T> : JsonConverter<StringId<T>>
{
    public override StringId<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            return default;

        return StringId<T>.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, StringId<T> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
