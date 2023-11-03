using FriedLanguage;
using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;

namespace FriedLang.NativeLibraries
{
    public partial class IO
    { 
        public static class File
        {
            public static FValue Exists(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");


                if (System.IO.File.Exists(path.Value))
                    return FBool.True;
                else
                    return FBool.False;
            }
            public static FValue Write(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FString text)
                    throw new Exception("Expected argument 1 to be a string");


                System.IO.File.WriteAllText(path.Value, text.Value);

                return FBool.True;
            }
            public static FValue Read(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");

                if (!System.IO.File.Exists(path.Value))
                    throw new Exception("File not found!");


                string text = System.IO.File.ReadAllText(path.Value);

                return new FString(text);
            }
        }
    }
}
