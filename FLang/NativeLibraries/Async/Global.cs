using FriedLanguage.BuiltinType;
using FriedLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLang.NativeLibraries
{
    public partial class Async 
    {
        public static class Global
        {
            public static FValue Wait(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FInt msWait)
                    throw new Exception("Expected argument 0 to be a int");

                Task.Delay(msWait.Value).Wait();

                return FBool.True;
            }
        }
    }
}
