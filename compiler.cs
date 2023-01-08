using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

string sourceCode = @"
using System;

class Program
{
static void Main()
{
Console.WriteLine(""Hello, World!"");
}
}";
string il = GetILCode(sourceCode);
Console.WriteLine(il);

static string GetILCode(string sourceCode)
{
    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

    CSharpCompilation compilation = CSharpCompilation.Create("MyCompilation", new[] { syntaxTree }, GetReferences(), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    using MemoryStream stream = new();
            
    EmitResult result = compilation.Emit(stream);

    if (!result.Success)
    {
        return "compilation failed due to errors";
    }

    stream.Position = 0;
    Assembly assembly = Assembly.Load(stream.ToArray());
    Type programType = assembly.GetType("Program");
    MethodInfo mainMethod = programType.GetMethod("Main");
    return GetMethodIL(mainMethod);
            
}

static string GetMethodIL(MethodBase method)
{
    MethodBody body = method.GetMethodBody();
    byte[] ilCode = body.GetILAsByteArray();
    return ConvertILCodeToString(ilCode);
}

static string ConvertILCodeToString(byte[] ilCode)
{
    using MemoryStream stream = new(ilCode);
    using StreamReader reader = new(stream);
    return reader.ReadToEnd();
}

static MetadataReference[] GetReferences()
{
    string netCorePath = Path.GetDirectoryName(typeof(object).Assembly.Location);

    return new[]
    {
        MetadataReference.CreateFromFile(Path.Combine(netCorePath, "mscorlib.dll")),
        MetadataReference.CreateFromFile(Path.Combine(netCorePath, "System.dll")),
        MetadataReference.CreateFromFile(Path.Combine(netCorePath, "System.Core.dll")),
        MetadataReference.CreateFromFile(Path.Combine(netCorePath, "System.Runtime.dll")),
    };
}
