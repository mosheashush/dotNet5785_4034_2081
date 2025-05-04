using BO;
using DalApi;
using System.Reflection;
using System.Text;

namespace Helpers;

internal static class Tools
{
    private static IDal s_dal = Factory.Get; //stage 4

    public static string ToStringProperty<T>(this T obj)
    {
        //    TODO: implement a better ToStringProperty method
        /*
        if (t.GetType.Name == "Volunteer")
        {
            Console.WriteLine();
        }
        return null;
        
        if (obj == null)
            return "null";

        StringBuilder sb = new StringBuilder();
        Type type = obj.GetType();
        sb.AppendLine($"{type.Name} Properties:");

        PropertyInfo[] properties = type.GetProperties();
        foreach (var prop in properties)
        {
            object? value = prop.GetValue(obj);
            sb.AppendLine($"  {prop.Name}: {value}");
        }

        return sb.ToString();
       */
        if (obj == null)
            return "null";

        Type type = obj.GetType();
        var properties = type.GetProperties();

        string propertiesString = string.Join(Environment.NewLine,
            properties.Select(p =>
            {
                object? value = p.GetValue(obj);
                return $"  {p.Name}: {value}";
            }));

        return $"{type.Name} Properties:{Environment.NewLine}{propertiesString}";
    }

}
