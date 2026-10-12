using System;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;

public class Program
{
    public static void Main()
    {
        var provider = CodeDomProvider.CreateProvider("JScript");
        var parameters = new CompilerParameters();
        parameters.GenerateExecutable = false;
        parameters.GenerateInMemory = true;
        var src = File.ReadAllText("test.js");
        var results = provider.CompileAssemblyFromSource(parameters, src);
        if (results.Errors.HasErrors)
        {
            foreach (CompilerError error in results.Errors)
            {
                Console.WriteLine("Line {0}, Col {1}: {2}", error.Line, error.Column, error.ErrorText);
            }
        }
        else
        {
            Console.WriteLine("No syntax errors!");
        }
    }
}
