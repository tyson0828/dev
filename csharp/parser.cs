using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class PListParser
{
  private readonly Dictionary<string, List<string>> rawMap = new();
  private readonly Dictionary<string, HashSet<string>> flattenedMap = new();
  private readonly Dictionary<string, List<string>> directPatternsMap = new();
  private readonly Dictionary<string, List<string>> subPlistsMap = new();

  public Dictionary<string, HashSet<string>> GetFlattenedMap() => flattenedMap;
  public Dictionary<string, List<string>> GetDirectPatternMap() => directPatternsMap;
  public Dictionary<string, List<string>> GetSubPlistMap() => subPlistsMap;
  
  public void ParseAllPlists(string directoryPath)
  {
    foreach (var filePath in Directory.GetFiles(directoryPath, "*.plist", SearchOption.AllDirectories))
    {
      
      var lines = File.ReadAllLines(filePath);
      ParseNestedPlists(lines, null, Path.GetFileName(filePath));
    }

    foreach (var plist in rawMap.Keys)
    {
      var resolved = new HashSet<string>();
      ResolveFlattenedPatterns(plist, resolved, new HashSet<string>());
      flattenedMap[plist] = resolved;

      var directPats = new List<string>();
      var subPlists = new List<string>();
  
      foreach (var line in rawMap[plist])
      {
          if (line.StartsWith("Pat "))
              directPats.Add(line.Substring(4).Trim(';', ' '));
          else if (line.StartsWith("RefPList ") || 
                   line.StartsWith("PreExecRefPList ") || 
                   line.StartsWith("PostExecRefPList "))
          {
              var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
              if (parts.Length >= 2)
                  subPlists.Add(parts[1].Trim(';', ' '));
          }
      }
  
      directPatternsMap[plist] = directPats;
      subPlistsMap[plist] = subPlists;      
    }
  }

    private void ParseNestedPlists(string[] lines, string? parentPrefix, string fileName)
    {
        var stack = new Stack<(string Name, List<string> Content, int StartLine)>();
        string? currentPrefix = parentPrefix;

        for (int i = 0; i < lines.Length; i++)
        {
            var rawLine = lines[i];
            var line = rawLine.Trim();
            int lineNumber = i + 1;

            var inlineMatch = Regex.Match(line, @"^PList\s+(\w+(?:\.\w+)*)\s*{\s*(.*?)\s*}\s*$");
            if (inlineMatch.Success)
            {
                var name = inlineMatch.Groups[1].Value;
                var inlineContent = inlineMatch.Groups[2].Value;
                var fullName = currentPrefix != null ? $"{currentPrefix}.{name}" : name;
                rawMap.TryAdd(fullName, new List<string>());
                rawMap[fullName].AddRange(SplitInlineContent(inlineContent));

                if (stack.Count > 0)
                    stack.Peek().Content.Add($"RefPList {fullName}");
                continue;
            }

            var matchOpenBrace = Regex.Match(line, "^PList\\s+(\\w+(?:\\.\\w+)*)\\s*{\\s*$");
            if (matchOpenBrace.Success)
            {
                var name = matchOpenBrace.Groups[1].Value;
                var fullName = currentPrefix != null ? $"{currentPrefix}.{name}" : name;
                stack.Push((fullName, new List<string>(), lineNumber));
                currentPrefix = fullName;
                continue;
            }

            var nameOnlyMatch = Regex.Match(line, "^PList\\s+(\\w+(?:\\.\\w+)*)\\s*$");
            if (nameOnlyMatch.Success)
            {
                var name = nameOnlyMatch.Groups[1].Value;
                var fullName = currentPrefix != null ? $"{currentPrefix}.{name}" : name;
                stack.Push((fullName, new List<string>(), lineNumber));
                currentPrefix = fullName;
                continue;
            }

            if (Regex.IsMatch(line, @"^\{\s*(#|//|$)")) continue;

            if (Regex.IsMatch(line, @"^\}\s*(#.*)?$"))
            {
                if (stack.Count > 0)
                {
                    var (plistName, content, _) = stack.Pop();
                    rawMap.TryAdd(plistName, new List<string>());
                    rawMap[plistName].AddRange(content);
                    if (stack.Count > 0)
                    {
                        stack.Peek().Content.Add($"RefPList {plistName}");
                        currentPrefix = GetParentPrefix(plistName);
                    }
                    else currentPrefix = parentPrefix;
                }
                else
                {
                    Console.WriteLine($"[Warning] Unmatched }} at {fileName}:{lineNumber}");
                }
                continue;
            }

            if (stack.Count > 0) stack.Peek().Content.Add(line);
            else Console.WriteLine($"[Warning] Line outside PList at {fileName}:{lineNumber}: {line}");
        }

        foreach (var (plistName, _, startLine) in stack)
            Console.WriteLine($"[Error] Unclosed PList '{plistName}' from {fileName}:{startLine}");
    }

    private void ResolveFlattenedPatterns(string plistName, HashSet<string> resolved, HashSet<string> visited)
    {
        if (!rawMap.ContainsKey(plistName) || visited.Contains(plistName)) return;
        visited.Add(plistName);

        foreach (var line in rawMap[plistName])
        {
            if (line.StartsWith("Pat "))
                resolved.Add(line.Substring(4).Trim(';', ' '));
            else if (line.StartsWith("RefPList "))
            {
                var refName = line.Substring(9).Trim(';', ' ');
                ResolveFlattenedPatterns(refName, resolved, visited);
            }
        }

        visited.Remove(plistName);
    }

    private string GetParentPrefix(string name)
    {
        var parts = name.Split('.');
        return parts.Length > 1 ? string.Join('.', parts, 0, parts.Length - 1) : string.Empty;
    }

    private List<string> SplitInlineContent(string content)
    {
        var list = new List<string>();
        foreach (var part in content.Split(';'))
        {
            var trimmed = part.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed)) list.Add(trimmed);
        }
        return list;
    }

    public void PrintFlattenedResults()
    {
        foreach (var kvp in flattenedMap)
        {
            Console.WriteLine($"{kvp.Key} => {{ {string.Join(", ", kvp.Value)} }}");
        }
    }
}
