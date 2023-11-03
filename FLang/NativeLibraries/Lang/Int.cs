using FriedLanguage.BuiltinType;
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
        public static class Int
        {
            public static FValue ToString(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FInt self)
                    throw new Exception("Expected argument 0 to be a int");

                return new FString(self.Value.ToString());
            }
        }
    }
}
