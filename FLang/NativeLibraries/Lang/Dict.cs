﻿using FriedLanguage.BuiltinType;
using FriedLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLang.NativeLibraries
{
    public partial class Lang
    {
        public static class Dict
        {
            public static FValue Add(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                if (arguments[1] is not FValue key)
                    throw new Exception("Expected argument 1 to be a key");

                if (arguments[2] is not FValue val)
                    throw new Exception("Expected argument 2 to be a value");

                self.Value.Add(new(key,val));

                return key;
            }
            public static FValue Get(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                if (arguments[1] is not FValue key)
                    throw new Exception("Expected argument 1 to be a key");

                var success = false;
                var index = self.Value.FindIndex(a => (a.key.Equals(key) as FBool).Value);
                if (index != -1)
                {
                    success = true;
                    return self.Value.ElementAt(index).val;
                }

                return new FNull();
            }
            public static FValue Remove(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                if (arguments[1] is not FValue key)
                    throw new Exception("Expected argument 1 to be a key");

                var success = false;
                var index = self.Value.FindIndex(a => (a.key.Equals(key) as FBool).Value);
                if (index != -1)
                {
                    success = true;
                    self.Value.RemoveAt(index);
                }

                return new FBool(success);
            }
            public static FValue RemoveAt(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                if (arguments[1] is not FInt val)
                    throw new Exception("Expected argument 1 to be a int");

                self.Value.RemoveAt(val.Value);

                return val;
            }
            public static FValue ContainsKey(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                if (arguments[1] is not FValue key)
                    throw new Exception("Expected argument 1 to be a key");

                var index = self.Value.FindIndex(a => (a.key.Equals(key) as FBool).Value);
                var success = (index != -1);

                return new FBool(success);
            }
            public static FValue ContainsValue(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                if (arguments[1] is not FValue value)
                    throw new Exception("Expected argument 1 to be a value");

                var index = self.Value.FindIndex(a => (a.val.Equals(value) as FBool).Value);
                var success = (index != -1);

                return new FBool(success);
            }
            public static FValue Count(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");


                return new FInt(self.Value.Count());
            }
            public static FValue First(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");


                return self.Value.First().val;
            }
            public static FValue Last(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");


                return self.Value.Last().val;
            }

            public static FValue ToString(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                string str = "";
                foreach (var (key,val) in self.Value)
                {
                    str += $"{{ {key.ToSpagString().Value},{val.ToSpagString().Value} }},\n";
                }

                return new FString(str);
            }

            public static FValue GetKeys(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                List<FValue> keys = self.Value.Select(x => x.key).ToList();

                return new FList(keys);
            }
            public static FValue GetValues(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                List<FValue> values = self.Value.Select(x => x.val).ToList();

                return new FList(values);
            }

            public static FValue Clear(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FDictionary self)
                    throw new Exception("Expected argument 0 to be a dictionairy");

                self.Value.Clear();

                return FBool.True;
            }
            public static FValue Create(Scope scope, List<FValue> arguments)
            {
                return new FDictionary();
            }
        }
    }
}
