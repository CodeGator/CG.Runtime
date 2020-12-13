using System;
using System.IO;
using System.Reflection;

namespace CG.Runtime.QuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            var loader = new AssemblyLoader();

            //var asm = loader.LoadFromAssemblyName(
            //    new AssemblyName("TestLibrary")
            //    );

            var path = @"..\..\..\..\TestLibrary\bin\Debug\netcoreapp3.1\TestLibrary.dll";
            if (Path.IsPathRooted(path))
            {
                var asm = loader.LoadFromAssemblyPath(
                    path
                    );
            }
            else
            {
                path = Path.GetFullPath(path);
                var asm = loader.LoadFromAssemblyPath(
                    path
                    );
            } 
        }
    }
}
