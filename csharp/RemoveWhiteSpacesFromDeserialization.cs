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
    
    private static void TrimStringMembers(object? obj)
    {
        if (obj == null) return;
    
        Type type = obj.GetType();
    
        // First: Handle properties
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead || !prop.CanWrite) continue;
    
            if (prop.PropertyType == typeof(string))
            {
                string? val = (string?)prop.GetValue(obj);
                if (val != null && val != val.Trim())
                {
                    Console.WriteLine($"[Trimmed] {type.Name}.{prop.Name}: '{val}' → '{val.Trim()}'");
                    prop.SetValue(obj, val.Trim());
                }
            }
            else if (!prop.PropertyType.IsPrimitive && !prop.PropertyType.IsEnum && !prop.PropertyType.IsArray)
            {
                var subObj = prop.GetValue(obj);
                HandleNested(subObj);
            }
        }
    
        // Then: Handle public fields
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.FieldType == typeof(string))
            {
                string? val = (string?)field.GetValue(obj);
                if (val != null && val != val.Trim())
                {
                    Console.WriteLine($"[Trimmed] {type.Name}.{field.Name}: '{val}' → '{val.Trim()}'");
                    field.SetValue(obj, val.Trim());
                }
            }
            else if (!field.FieldType.IsPrimitive && !field.FieldType.IsEnum && !field.FieldType.IsArray)
            {
                var subObj = field.GetValue(obj);
                HandleNested(subObj);
            }
        }
    }
    
    private static void HandleNested(object? subObj)
    {
        if (subObj == null) return;
    
        if (subObj is System.Collections.IEnumerable collection)
        {
            foreach (var item in collection)
                TrimStringMembers(item);
        }
        else
        {
            TrimStringMembers(subObj);
        }
    }
    
}
