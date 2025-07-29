using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

public class XmlLoader
{
    public static T DeserializeAndTrim<T>(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using FileStream fs = new FileStream(filePath, FileMode.Open);
        T obj = (T)serializer.Deserialize(fs)!;

        TrimStringProperties(obj);
        return obj;
    }

    private static void TrimStringProperties(object? obj)
    {
        if (obj == null) return;

        Type type = obj.GetType();
        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || !prop.CanWrite) continue;

            if (prop.PropertyType == typeof(string))
            {
                string? val = (string?)prop.GetValue(obj);
                if (val != null && (val != val.Trim()))
                {
                    Console.WriteLine($"[Trimmed] {type.Name}.{prop.Name}: '{val}' → '{val.Trim()}'");
                    prop.SetValue(obj, val.Trim());
                }
            }
            else if (!prop.PropertyType.IsPrimitive && !prop.PropertyType.IsEnum && !prop.PropertyType.IsArray)
            {
                var subObj = prop.GetValue(obj);
                if (subObj != null)
                {
                    if (subObj is System.Collections.IEnumerable collection)
                    {
                        foreach (var item in collection)
                            TrimStringProperties(item);
                    }
                    else
                    {
                        TrimStringProperties(subObj);
                    }
                }
            }
        }
    }
}
