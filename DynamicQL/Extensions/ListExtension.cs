namespace DynamicQL.Extensions;

public static class ListExtension
{
    public static void Add<T>(this List<Type> list)
    {
        list.Add(typeof(T));
    }
}