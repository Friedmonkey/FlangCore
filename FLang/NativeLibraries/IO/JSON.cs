using FriedLanguage;
using FriedLanguage.BuiltinType;
using FriedLanguage.Models.Parsing;
using FriedLanguage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static FriedLang.NativeLibraries.IO;
using Friedlang.NativeLibraries.IO;

namespace FriedLang.NativeLibraries
{
    public partial class IO
    { 
        public static class JSON
        {
            public static FValue Serialize(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FValue obj)
                    throw new Exception("Expected argument 0 to be a value");

                var objec = FLang.FromFriedVar(obj);

                var json = JsonConvert.SerializeObject(objec);

                return new FString(json);
            }
            public static FValue Deserialize(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString json)
                    throw new Exception("Expected argument 0 to be a string");

                JsonParser parser = new JsonParser();

                FDynamic dynamic = parser.Parse(json.Value);


                return dynamic;
            }
            
        }
    }
}
