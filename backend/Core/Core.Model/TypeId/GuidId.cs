using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Model.TypeId;

[JsonConverter(typeof(TypedIdJsonConverterFactory))]
public readonly struct GuidId<TEntity> : IEquatable<GuidId<TEntity>>, ITypedId<Guid>
{
    internal Guid Value { get; }

    public GuidId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Guid Id cannot be empty", nameof(value));

        Value = value;
    }

    public static GuidId<TEntity> New() => new(Guid.NewGuid());
    public static GuidId<TEntity> Parse(string value) => new(Guid.Parse(value));
    public static GuidId<TEntity> Parse(Guid value) => new(value);

    public static bool TryParse(string? value, out GuidId<TEntity> result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default;
            return false;
        }

        if (Guid.TryParse(value, out var guid) && guid != Guid.Empty)
        {
            result = new GuidId<TEntity>(guid);
            return true;
        }

        result = default;
        return false;
    }

    public Guid ToInternal() => Value;

    public override string ToString() => Value.ToString();
    public override int GetHashCode() => Value.GetHashCode();
    public override bool Equals(object? obj) => obj is GuidId<TEntity> other && Equals(other);
    public bool Equals(GuidId<TEntity> other) => Value == other.Value;

    public static bool operator ==(GuidId<TEntity> left, GuidId<TEntity> right) => left.Equals(right);
    public static bool operator !=(GuidId<TEntity> left, GuidId<TEntity> right) => !left.Equals(right);

    public static explicit operator Guid(GuidId<TEntity> id) => id.Value;
    public static explicit operator GuidId<TEntity>(Guid value) => new(value);
}

public class GuidIdJsonConverter<T> : JsonConverter<GuidId<T>>
{
    public override GuidId<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            return default;

        return GuidId<T>.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, GuidId<T> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
