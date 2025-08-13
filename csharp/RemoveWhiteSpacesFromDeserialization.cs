using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

public static class XmlLoader
{
    public static T DeserializeAndTrim<T>(string filePath)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var fs = new FileStream(filePath, FileMode.Open);
        var obj = (T)serializer.Deserialize(fs)!;

        // Use reference-based visited set to break cycles
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        TrimStringMembers(obj!, visited, depth: 0);
        return obj;
    }

    private static void TrimStringMembers(object obj, HashSet<object> visited, int depth)
    {
        if (obj == null) return;

        // Cap depth defensively (optional)
        if (depth > 256) return;

        // Strings: nothing to recurse into
        if (obj is string) return;

        // Break cycles by identity
        if (!visited.Add(obj)) return;

        var type = obj.GetType();

        // Optionally skip framework types to reduce noise
        // if (type.Namespace != null && type.Namespace.StartsWith("System", StringComparison.Ordinal)) return;

        // Properties
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead) continue;

            var propType = prop.PropertyType;
            if (propType == typeof(string) && prop.CanWrite)
            {
                var val = (string?)prop.GetValue(obj);
                if (val != null)
                {
                    var trimmed = val.Trim();
                    if (!ReferenceEquals(val, trimmed) && val != trimmed)
                    {
                        Console.WriteLine($"[Trimmed] {type.Name}.{prop.Name}: '{val}' → '{trimmed}'");
                        prop.SetValue(obj, trimmed);
                    }
                }
            }
            else if (!propType.IsPrimitive && !propType.IsEnum && !propType.IsArray)
            {
                var subObj = prop.GetValue(obj);
                RecurseInto(subObj, visited, depth);
            }
        }

        // Fields
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            var fType = field.FieldType;

            if (fType == typeof(string))
            {
                var val = (string?)field.GetValue(obj);
                if (val != null)
                {
                    var trimmed = val.Trim();
                    if (val != trimmed)
                    {
                        Console.WriteLine($"[Trimmed] {type.Name}.{field.Name}: '{val}' → '{trimmed}'");
                        field.SetValue(obj, trimmed);
                    }
                }
            }
            else if (!fType.IsPrimitive && !fType.IsEnum && !fType.IsArray)
            {
                var subObj = field.GetValue(obj);
                RecurseInto(subObj, visited, depth);
            }
        }
    }

    private static void RecurseInto(object? subObj, HashSet<object> visited, int depth)
    {
        if (subObj == null) return;
        if (subObj is string) return;

        if (subObj is IEnumerable seq)
        {
            foreach (var item in seq)
            {
                if (item == null || item is string) continue;
                TrimStringMembers(item, visited, depth + 1);
            }
        }
        else
        {
            TrimStringMembers(subObj, visited, depth + 1);
        }
    }

    // Reference equality comparer for cycle detection
    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new();
        public new bool Equals(object x, object y) => ReferenceEquals(x, y);
        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
