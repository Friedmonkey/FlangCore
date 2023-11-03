using FriedLanguage.BuiltinType;
using FriedLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLang.NativeLibraries
{
    public partial class Popups
    {
        public static class Popup
        {
            public static FValue Show(Scope scope, List<FValue> arguments)
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
        }
    }
}
