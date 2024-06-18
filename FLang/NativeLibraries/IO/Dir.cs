using FriedLanguage;
using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FriedLang.NativeLibraries
{
    public partial class IO
    { 
        public static class Dir
        {
            public static FValue Exists(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");


                if (System.IO.Directory.Exists(path.Value))
                    return FBool.True;
                else
                    return FBool.False;
            }
            public static FValue Create(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");


                System.IO.Directory.CreateDirectory(path.Value);

                return FBool.True;
            }
            public static FValue Delete(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FBool force)
                    throw new Exception("Expected argument 1 to be a bool");

                if (!System.IO.Directory.Exists(path.Value))
                    throw new Exception("Directory not found!");


                System.IO.Directory.Delete(path.Value, force.Value);

                return FBool.True;
            }
            public static FValue GetDirectories(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");

                if (!System.IO.Directory.Exists(path.Value))
                    throw new Exception("Directory not found!");


                string[] lines = System.IO.Directory.EnumerateDirectories(path.Value).ToArray();

                return new FList(lines);
            }
            public static FValue GetFiles(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");

                if (!System.IO.Directory.Exists(path.Value))
                    throw new Exception("Directory not found!");


                string[] lines = System.IO.Directory.EnumerateFiles(path.Value).ToArray();

                return new FList(lines);
            }
        }
    }
}
