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
using static FriedLang.NativeLibraries.Lang;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Claims;

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

                object objec = new Dictionary<object,object>();

                if ((!(obj is
                 FBaseFunction or
                 FBool or
                 FClass or
                 //FClassInstance or
                 FDictionary or
                 FDouble or
                 FDynamic or
                 FException or
                 FFloat or
                 FFunction or
                 FInt or
                 FLabel or
                 FList or
                 FLong or
                 FNativeFunction or
                 FNativeLibraryImporter or
                 FNull or
                 FString
                    )) && obj is FClassInstance classInstance)
                {
                    Dictionary<object, object> Dictionary = new Dictionary<object, object>();

                    foreach (var (key, value) in classInstance.InstanceTable)
                    {
                        if (value is not FFunction)
                            Dictionary.Add(key, FLang.FromFriedVar(value));
                    }
                        
                    objec = Dictionary;
                }
                else
                { 
                    objec = FLang.FromFriedVar(obj);
                }

                var json = JsonConvert.SerializeObject(objec);

                return new FString(json);
            }
            public static FValue Deserialize(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString json)
                    throw new Exception("Expected argument 0 to be a string");

                JsonParser parser = new JsonParser();

                FValue output = parser.Parse(json.Value);


                return output;
            }
            public static FValue DeserializeClass(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString json)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FClass clas)
                    throw new Exception("Expected argument 1 to be a class");

                JsonParser parser = new JsonParser();
                FValue output = parser.Parse(json.Value);

                if (output is FDynamic dynamic)
                {
                    var instance = new FClassInstance(clas);
                    foreach (var (fkey, value) in dynamic.Value) 
                    {
                        if (fkey is FString key)
                        {
                            instance.SetValue(key.Value, value, scope);
                        }
                    }
                    return instance;
                }

                return output;
            }

        }
    }
}
