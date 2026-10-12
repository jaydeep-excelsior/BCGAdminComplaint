using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string path = @"Controllers\ReportsController.cs";
        string content = File.ReadAllText(path);
        
        // Match `if (Request.Cookies["Key"].Value != null)`
        // and replace with `if (Request.Cookies["Key"] != null && Request.Cookies["Key"].Value != null)`
        string pattern = @"if\s*\(\s*Request\.Cookies\[""([^""]+)""\]\.Value\s*!=\s*null\s*\)";
        string replacement = @"if (Request.Cookies[""$1""] != null && Request.Cookies[""$1""].Value != null)";
        
        string newContent = Regex.Replace(content, pattern, replacement);
        
        File.WriteAllText(path, newContent);
        Console.WriteLine("Done replacing logic in ReportsController.");
    }
}
