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
        public static class String
        {
            public static FValue Count(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                return new FInt(self.Value.Length);

            }
            public static FValue ToInt(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FInt defaul)
                    throw new Exception("Expected argument 1 to be a int");

                if (Int32.TryParse(self.Value, out int output))
                    return new FInt(output);
                else
                    return defaul;

            }
            public static FValue Split(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FString splitter)
                    throw new Exception("Expected argument 1 to be a string");

                char Char = splitter.Value.First();
                var splitted = self.Value.Split(Char);

                return new FList(splitted);
            }
            public static FValue Join(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FClass self)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FString character)
                    throw new Exception("Expected argument 1 to be a string");

                if (arguments[2] is not FList lst)
                    throw new Exception("Expected argument 2 to be a list");

                string output = string.Join(character.Value, FLang.FromFriedList(lst.Value));

                return new FString(output);
            }
            public static FValue Format(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FClass self)
                    throw new Exception("Expected argument 0 to be a fClass");

                arguments.RemoveAt(0);
                arguments.Insert(0,new FInt(1));

                return FormatBase(scope,arguments.Skip(1).ToList());
            }
            public static FValue FormatBase(Scope scope, List<FValue> arguments)
            {
                int skipp = 0;

                if (arguments[0] is FInt fint)
                { 
                    skipp = fint.Value;
                    arguments.RemoveAt(0);
                }

                if (arguments[0] is not FString str)
                    throw new Exception($"Expected argument {0+skipp} to be a string");

                if (arguments[1] is not FList lst)
                    throw new Exception($"Expected argument {1+skipp} to be a list");

                List<object> formattable = FLang.FromFriedList(lst.Value);
                string output = string.Format(str.Value, formattable.ToArray());

                return new FString(output);
            }
            public static FValue Replace(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FString replacing)
                    throw new Exception("Expected argument 1 to be a string");

                if (arguments[2] is not FString replacement)
                    throw new Exception("Expected argument 2 to be a string");


                return new FString(self.Value.Replace(replacing.Value, replacement.Value));
            }
            public static FValue Contains(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FString contains)
                    throw new Exception("Expected argument 1 to be a string");

                return new FBool(self.Value.Contains(contains.Value));
            }

            public static FValue StartsWith(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FString with)
                    throw new Exception("Expected argument 1 to be a string");

                return new FBool(self.Value.StartsWith(with.Value));
            }

            public static FValue EndsWith(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FString with)
                    throw new Exception("Expected argument 1 to be a string");

                return new FBool(self.Value.EndsWith(with.Value));
            }

            public static FValue SubString(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FInt start)
                    throw new Exception("Expected argument 1 to be a int");

                if (arguments[1] is not FInt length)
                    throw new Exception("Expected argument 1 to be a int");

                int len = length.Value;

                if (len == -1)
                    len = self.Value.Length;

                return new FString(self.Value.Substring(start.Value,len));
            }

            public static FValue ToUpper(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                return new FString(self.Value.ToUpper());
            }
            public static FValue ToLower(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                return new FString(self.Value.ToUpper());
            }

            public static FValue Trim(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString self)
                    throw new Exception("Expected argument 0 to be a string");

                return new FString(self.Value.Trim());
            }
        }
    }
}
