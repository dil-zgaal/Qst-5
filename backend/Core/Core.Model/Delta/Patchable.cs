using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Model.Delta;

/// <summary>
/// Patchable for non-nullable types. Only supports NotGiven and Set states.
/// </summary>
[JsonConverter(typeof(PatchableJsonConverter))]
public struct Patchable<T> where T : notnull
{
    private T? _value;
    private bool _isSet;

    private Patchable(T? value, bool isSet)
    {
        _value = value;
        _isSet = isSet;
    }

    public static Patchable<T> NotGiven() => new(default, false);
    public static Patchable<T> Set(T value) => new(value, true);

    public bool IsNotGiven => !_isSet;
    public bool IsSet => _isSet;
    public bool HasValue => _isSet;

    public T Value => _isSet && _value != null ? _value : throw new InvalidOperationException("Patchable has no value");

    public void Patch(Action<T> setter)
    {
        if (_isSet && _value != null)
            setter(_value);
    }

    public void Apply(Patchable<T> delta)
    {
        if (delta._isSet)
        {
            _isSet = true;
            _value = delta._value;
        }
    }
}

/// <summary>
/// Patchable for nullable types. Supports NotGiven, Clear, and Set states.
/// </summary>
[JsonConverter(typeof(PatchableNullableJsonConverter))]
public struct PatchableNullable<T> where T : notnull
{
    private T? _value;
    private PatchState _state;

    private PatchableNullable(T? value, PatchState state)
    {
        _value = value;
        _state = state;
    }

    public static PatchableNullable<T> NotGiven() => new(default, PatchState.NotGiven);
    public static PatchableNullable<T> Clear() => new(default, PatchState.Clear);
    public static PatchableNullable<T> Set(T? value) => value == null ? Clear() : new(value, PatchState.Set);

    public bool IsNotGiven => _state == PatchState.NotGiven;
    public bool IsClear => _state == PatchState.Clear;
    public bool IsSet => _state == PatchState.Set;
    public bool HasValue => _state == PatchState.Set;

    public T? Value => _state == PatchState.Set ? _value : default;

    public void Patch(Action<T?> setter)
    {
        if (_state == PatchState.NotGiven)
            return;

        if (_state == PatchState.Clear)
        {
            setter(default);
            return;
        }

        setter(_value);
    }

    public void Apply(PatchableNullable<T> delta)
    {
        if (delta.IsNotGiven)
            return;
        if (delta.IsClear)
        {
            _state = PatchState.Clear;
            _value = default;
            return;
        }
        _state = PatchState.Set;
        _value = delta._value;
    }

    private enum PatchState
    {
        NotGiven,
        Clear,
        Set
    }
}

// Custom JSON converter for Patchable<T> (non-nullable)
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

public class PatchableConverter<T> : JsonConverter<Patchable<T>> where T : notnull
{
    public override Patchable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Cannot deserialize null to non-nullable Patchable<T>. Use PatchableNullable<T> instead.");
        }

        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return value == null ? throw new JsonException("Deserialized value is null") : Patchable<T>.Set(value);
    }

    public override void Write(Utf8JsonWriter writer, Patchable<T> value, JsonSerializerOptions options)
    {
        if (value.IsNotGiven)
        {
            // Don't write anything for NotGiven
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }
}

// Custom JSON converter for PatchableNullable<T>
public class PatchableNullableJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(PatchableNullable<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(PatchableNullableConverter<>).MakeGenericType(valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

public class PatchableNullableConverter<T> : JsonConverter<PatchableNullable<T>> where T : notnull
{
    public override PatchableNullable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return PatchableNullable<T>.Clear();
        }

        var value = JsonSerializer.Deserialize<T>(ref reader, options);
        return value == null ? PatchableNullable<T>.Clear() : PatchableNullable<T>.Set(value);
    }

    public override void Write(Utf8JsonWriter writer, PatchableNullable<T> value, JsonSerializerOptions options)
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
