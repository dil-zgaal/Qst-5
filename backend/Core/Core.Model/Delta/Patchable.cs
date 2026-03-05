using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Model.Delta;

[JsonConverter(typeof(PatchableJsonConverter))]
public struct Patchable<T>
{
    private readonly T? _value;
    private readonly PatchState _state;

    private Patchable(T? value, PatchState state)
    {
        _value = value;
        _state = state;
    }

    public static Patchable<T> NotGiven() => new(default, PatchState.NotGiven);
    public static Patchable<T> Clear() => new(default, PatchState.Clear);
    public static Patchable<T> Set(T value) => new(value, PatchState.Set);

    public bool IsNotGiven => _state == PatchState.NotGiven;
    public bool IsClear => _state == PatchState.Clear;
    public bool IsSet => _state == PatchState.Set;
    public bool HasValue => _state == PatchState.Set;

    public T? Value => _state == PatchState.Set ? _value : default;

    public void Apply<TTarget>(TTarget target, Action<TTarget, T?> setter)
    {
        if (_state == PatchState.NotGiven)
            return;

        if (_state == PatchState.Clear)
        {
            setter(target, default);
            return;
        }

        setter(target, _value);
    }

    private enum PatchState
    {
        NotGiven,
        Clear,
        Set
    }
}

// Custom JSON converter for Patchable<T>
public class PatchableJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(Patchable<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(PatchableConverter<>).MakeGenericType(valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class PatchableConverter<T> : JsonConverter<Patchable<T>>
{
    public override Patchable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return Patchable<T>.Clear();
        }

        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return value == null ? Patchable<T>.Clear() : Patchable<T>.Set(value);
    }

    public override void Write(Utf8JsonWriter writer, Patchable<T> value, JsonSerializerOptions options)
    {
        if (value.IsNotGiven)
        {
            // Don't write anything for NotGiven
            return;
        }

        if (value.IsClear)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }
}
