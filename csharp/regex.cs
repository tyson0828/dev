using System;
using System.Text.RegularExpressions;

string[] lines = {
    "./Modules/ARR_ABC/ARR_ABC.mtpl",
    ".\\Modules\\ARR_XXX\\ARR_XXX.mtpl",
    "./Modules/MyTest/MyFile.mtpl"
};

var pattern = @"[.][\\/](Modules[\\/][^\\/]+[\\/][^\\/]+\.mtpl)";

foreach (var line in lines)
{
    var match = Regex.Match(line, pattern);
    if (match.Success)
    {
        // Extract and normalize to forward slashes
        string path = match.Groups[1].Value.Replace('\\', '/');
        Console.WriteLine($"Extracted: {path}");
    }
}
