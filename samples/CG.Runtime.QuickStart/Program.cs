using System;
using System.Reflection;

namespace CG.Runtime.QuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            var loader = new AssemblyLoader();

            var asm = loader.LoadFromAssemblyName(
                new AssemblyName("TestLibrary")
                );
        }
    }
}
