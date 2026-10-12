using System;
using System.IO;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string path = @"Views\DCCommittee\Index.cshtml";
        string content = File.ReadAllText(path);
        string newContent = Regex.Replace(content, @"(?m)^\s*console\.log.*$\r?\n", "");
        File.WriteAllText(path, newContent);
    }
}
