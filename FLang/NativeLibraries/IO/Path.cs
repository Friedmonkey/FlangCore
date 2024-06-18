using FriedLanguage;
using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FriedLang.NativeLibraries
{
    public partial class IO
    { 
        public static class Path
        {
            public static FValue Exists(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");


                if (System.IO.Path.Exists(path.Value))
                    return FBool.True;
                else
                    return FBool.False;
            }
            public static FValue Combine(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not (FString or FList))
                    throw new Exception("Expected argument 0 to be a string or a list");

                if (arguments[0] is FString path)
                {
                    if (arguments[1] is not FString path2)
                        throw new Exception("Expected argument 1 to be a string");

                    var combined = System.IO.Path.Combine(path.Value, path2.Value);
                    return new FString(combined);
                }

                if (arguments[0] is FList paths)
                {
                    var objs = FLang.FromFriedList<string>(paths.Value);
                    var combined = System.IO.Path.Combine(objs.ToArray());
                    return new FString(combined);
                }



                return FBool.True;
            }
            public static FValue GetFileName(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");


                if (!System.IO.Path.Exists(path.Value))
                    throw new Exception("File not found!");


                var name = System.IO.Path.GetFileName(path.Value);

                return new FString(name);
            }
            public static FValue GetDirectoryName(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString path)
                    throw new Exception("Expected argument 0 to be a string");


                if (!System.IO.Path.Exists(path.Value))
                    throw new Exception("Directory not found!");


                var name = System.IO.Path.GetDirectoryName(path.Value);

                return new FString(name);
            }
        }
    }
}
