using BO;
using DalApi;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Helpers;

internal static class Tools
{
    private static IDal s_dal = Factory.Get; //stage 4

    public static string ToStringProperty<T>(this T obj)
    {
        var visited = new HashSet<object>();
        return ToStringPropertyRecursive(obj, 0, visited);
    }

    private static string ToStringPropertyRecursive(object? obj, int indentLevel, HashSet<object> visited)
    {
        if (obj == null)
            return Indent("null", indentLevel);

        if (visited.Contains(obj))
            return Indent($"(Already printed {obj.GetType().Name})", indentLevel);

        visited.Add(obj);

        var type = obj.GetType();
        var sb = new StringBuilder();
        sb.AppendLine(Indent($"{type.Name} {{", indentLevel));

        // Properties
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object? value;
            try { value = prop.GetValue(obj); }
            catch { continue; }

            sb.Append(Indent($"{prop.Name} = ", indentLevel + 1));
            sb.AppendLine(FormatValue(value, indentLevel + 1, visited));
        }

        // Fields (אם אתה לא צריך – אפשר להסיר)
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            object? value;
            try { value = field.GetValue(obj); }
            catch { continue; }

            sb.Append(Indent($"{field.Name} = ", indentLevel + 1));
            sb.AppendLine(FormatValue(value, indentLevel + 1, visited));
        }

        sb.AppendLine(Indent("}", indentLevel));
        return sb.ToString();
    }

    private static string FormatValue(object? value, int indentLevel, HashSet<object> visited)
    {
        if (value == null)
            return "null";

        if (value is string s)
            return $"\"{s}\"";

        if (value is IEnumerable enumerable and not string)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[");
            foreach (var item in enumerable)
                sb.AppendLine(ToStringPropertyRecursive(item!, indentLevel + 1, visited));
            sb.Append(Indent("]", indentLevel));
            return sb.ToString();
        }

        if (IsPrimitive(value.GetType()))
            return value.ToString()!;

        return ToStringPropertyRecursive(value, indentLevel, visited);
    }

    private static string Indent(string text, int level) =>
        new string(' ', level * 2) + text;

    private static bool IsPrimitive(Type type) =>
        type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(decimal);


}
