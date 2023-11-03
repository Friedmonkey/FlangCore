using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FNativeLibraryImporter : FValue
    {
        public override FBuiltinType BuiltinName => FBuiltinType.NativeLibraryImporter;
        public Action<Scope,bool> Import { get; set; } = (Scope scope, bool extend) => { };

        public FNativeLibraryImporter(Action<Scope,bool> import)
        {
            Import = import;
        }

        public override bool IsTruthy()
        {
            return true;
        }
    }
}
