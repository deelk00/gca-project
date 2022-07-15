using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using DynamicQL.Model;
using DynamicQL.Model.ScalarTypes;
using DynamicQL.Model.Types;
using DynamicQL.Services;
using GraphQL;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace DynamicQL.Utility;

public static class StaticData
{
    /// <summary>
    /// Key   -> Type == basetype of graphtype
    /// Key   -> bool == IsSingleValue
    /// Value -> DynamicQLOutputTypeMetaInfo
    /// </summary>
    public static readonly Dictionary<Type, TypeMetaInfo> TypeToGraphQLTypeMetaInfoMap =
        new();
    
    public static readonly Dictionary<Type, Type> EnumToGraphQLEnumTypeMap =
        new();

    public static readonly Dictionary<Type, List<PropertyMetaInfo>>
        LeftOverTypeToGraphQLFieldMetaInfoMap = new();

    public static readonly Dictionary<Type, Type> BaseTypeToQueryTypeMap = new Dictionary<Type, Type>()
    {
        { typeof(string), typeof(NonNullGraphType<StringGraphType>) },
        { typeof(bool), typeof(NonNullGraphType<BooleanGraphType>) },
        { typeof(Guid), typeof(GuidGraphType) },
        { typeof(BigInteger), typeof(NonNullGraphType<BigIntGraphType>) },
        { typeof(long), typeof(NonNullGraphType<LongGraphType>) },
        { typeof(int), typeof(IntGraphType) },
        { typeof(short), typeof(NonNullGraphType<ShortGraphType>) },
        { typeof(byte), typeof(NonNullGraphType<ByteGraphType>) },
        { typeof(float), typeof(NonNullGraphType<FloatGraphType>) },
        { typeof(double), typeof(NonNullGraphType<FloatGraphType>) },
        { typeof(DateTime), typeof(NonNullGraphType<DateGraphType>) },
        { typeof(DateOnly), typeof(NonNullGraphType<DateOnlyGraphType>) },
        { typeof(DateTimeOffset), typeof(NonNullGraphType<DateTimeOffsetGraphType>) },
        { typeof(decimal), typeof(NonNullGraphType<DecimalGraphType>) },
        { typeof(TimeSpan), typeof(NonNullGraphType<TimeSpanMillisecondsGraphType>) },
        { typeof(TimeOnly), typeof(NonNullGraphType<TimeOnlyGraphType>) },
        { typeof(Uri), typeof(NonNullGraphType<UriGraphType>) },
        { typeof(ushort), typeof(NonNullGraphType<UShortGraphType>) },
        { typeof(uint), typeof(NonNullGraphType<UIntGraphType>) },
        { typeof(ulong), typeof(NonNullGraphType<ULongGraphType>) },
        { typeof(sbyte), typeof(NonNullGraphType<SByteGraphType>) },

        { typeof(bool?), typeof(BooleanGraphType) },
        { typeof(Guid?), typeof(GuidGraphType) },
        { typeof(BigInteger?), typeof(BigIntGraphType) },
        { typeof(long?), typeof(LongGraphType) },
        { typeof(int?), typeof(IntGraphType) },
        { typeof(short?), typeof(ShortGraphType) },
        { typeof(byte?), typeof(ByteGraphType) },
        { typeof(float?), typeof(FloatGraphType) },
        { typeof(double?), typeof(FloatGraphType) },
        { typeof(DateTime?), typeof(DateGraphType) },
        { typeof(DateOnly?), typeof(DateOnlyGraphType) },
        { typeof(DateTimeOffset?), typeof(DateTimeOffsetGraphType) },
        { typeof(decimal?), typeof(DecimalGraphType) },
        { typeof(TimeSpan?), typeof(TimeSpanMillisecondsGraphType) },
        { typeof(TimeOnly?), typeof(TimeOnlyGraphType) },
        { typeof(ushort?), typeof(UShortGraphType) },
        { typeof(uint?), typeof(UIntGraphType) },
        { typeof(ulong?), typeof(ULongGraphType) },
        { typeof(sbyte?), typeof(SByteGraphType) },
        
        { typeof(Dictionary<string, string?>), typeof(DictionaryGraphType<string, string?>) }
    };
    
    public static readonly Dictionary<Type, Type> BaseTypeToInputTypeMap = new Dictionary<Type, Type>()
    {
        { typeof(string), typeof(NonNullGraphType<StringGraphType>) },
        { typeof(bool), typeof(NonNullGraphType<BooleanGraphType>) },
        { typeof(Guid), typeof(GuidGraphType) },
        { typeof(BigInteger), typeof(NonNullGraphType<BigIntGraphType>) },
        { typeof(long), typeof(NonNullGraphType<LongGraphType>) },
        { typeof(int), typeof(IntGraphType) },
        { typeof(short), typeof(NonNullGraphType<ShortGraphType>) },
        { typeof(byte), typeof(NonNullGraphType<ByteGraphType>) },
        { typeof(float), typeof(NonNullGraphType<FloatGraphType>) },
        { typeof(double), typeof(NonNullGraphType<FloatGraphType>) },
        { typeof(DateTime), typeof(NonNullGraphType<DateGraphType>) },
        { typeof(DateOnly), typeof(NonNullGraphType<DateOnlyGraphType>) },
        { typeof(DateTimeOffset), typeof(NonNullGraphType<DateTimeOffsetGraphType>) },
        { typeof(decimal), typeof(NonNullGraphType<DecimalGraphType>) },
        { typeof(TimeSpan), typeof(NonNullGraphType<TimeSpanMillisecondsGraphType>) },
        { typeof(TimeOnly), typeof(NonNullGraphType<TimeOnlyGraphType>) },
        { typeof(Uri), typeof(NonNullGraphType<UriGraphType>) },
        { typeof(ushort), typeof(NonNullGraphType<UShortGraphType>) },
        { typeof(uint), typeof(NonNullGraphType<UIntGraphType>) },
        { typeof(ulong), typeof(NonNullGraphType<ULongGraphType>) },
        { typeof(sbyte), typeof(NonNullGraphType<SByteGraphType>) },

        { typeof(bool?), typeof(BooleanGraphType) },
        { typeof(Guid?), typeof(GuidGraphType) },
        { typeof(BigInteger?), typeof(BigIntGraphType) },
        { typeof(long?), typeof(LongGraphType) },
        { typeof(int?), typeof(IntGraphType) },
        { typeof(short?), typeof(ShortGraphType) },
        { typeof(byte?), typeof(ByteGraphType) },
        { typeof(float?), typeof(FloatGraphType) },
        { typeof(double?), typeof(FloatGraphType) },
        { typeof(DateTime?), typeof(DateGraphType) },
        { typeof(DateOnly?), typeof(DateOnlyGraphType) },
        { typeof(DateTimeOffset?), typeof(DateTimeOffsetGraphType) },
        { typeof(decimal?), typeof(DecimalGraphType) },
        { typeof(TimeSpan?), typeof(TimeSpanMillisecondsGraphType) },
        { typeof(TimeOnly?), typeof(TimeOnlyGraphType) },
        { typeof(ushort?), typeof(UShortGraphType) },
        { typeof(uint?), typeof(UIntGraphType) },
        { typeof(ulong?), typeof(ULongGraphType) },
        { typeof(sbyte?), typeof(SByteGraphType) },
    };

    public static readonly MethodInfo SingleQueryResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralSingleQueryResolver))!
        .GetGenericMethodDefinition();
    
    public static readonly MethodInfo MultiQueryResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralMultiQueryResolver))!
        .GetGenericMethodDefinition();
    
    public static readonly MethodInfo SingleCreateResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralSingleCreateMutationResolver))!
        .GetGenericMethodDefinition();
    
    public static readonly MethodInfo MultiCreateResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralMultiCreateMutationResolver))!
        .GetGenericMethodDefinition();

    
    public static readonly MethodInfo SingleUpdateResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralSingleUpdateMutationResolver))!
        .GetGenericMethodDefinition();
    
    public static readonly MethodInfo MultiUpdateResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralMultiUpdateMutationResolver))!
        .GetGenericMethodDefinition();
    
    public static readonly MethodInfo SingleDeleteByIdResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralSingleDeleteByIdMutationResolver))!
        .GetGenericMethodDefinition();
    
    public static readonly MethodInfo MultiDeleteByIdResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralMultiDeleteByIdMutationResolver))!
        .GetGenericMethodDefinition();
    
    public static readonly MethodInfo SingleDeleteByEntityResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralSingleDeleteByEntityMutationResolver))!
        .GetGenericMethodDefinition();
    
    public static readonly MethodInfo MultiDeleteByEntityResolverMethodInfo = typeof(StaticResolvers)
        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        .First(m => m.Name == nameof(StaticResolvers.GeneralMultiDeleteByEntityMutationResolver))!
        .GetGenericMethodDefinition();

}