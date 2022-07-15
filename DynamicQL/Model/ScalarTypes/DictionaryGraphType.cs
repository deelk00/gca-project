using System.Text.Json;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace DynamicQL.Model.ScalarTypes;

public class DictionaryGraphType<TKey, TProperty> : ScalarGraphType
    where TKey : notnull
{
    public override object? ParseValue(object? value)
    {
        return value switch
        {
            NullValue => null,
            StringValue stringValue => JsonSerializer.Deserialize<Dictionary<TKey, TProperty>>(stringValue.Value),
            _ => ThrowLiteralConversionError(value as IValue)
        };
    }

    public override object Serialize(object? value)
    {
        return JsonSerializer.Serialize(value);
    }
}