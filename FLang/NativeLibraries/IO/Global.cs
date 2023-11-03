using FriedLanguage;
using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FriedLang.NativeLibraries
{
    public partial class IO
    { 
        public static class Global
        {
            public static FValue Print(Scope scope, List<FValue> arguments)
            {
                Console.WriteLine(arguments.First().SpagToCsString());
                return arguments.First();
            }
            public static FValue Read(Scope scope, List<FValue> arguments)
            {
                return new FString(Console.ReadLine());
            }
        }
    }
}
