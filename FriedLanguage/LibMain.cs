using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage
{
    public abstract class ExternLibMain
    {
        public abstract void Mount(Scope scope);
    }
    public abstract class NativeLibMain
    {
        public abstract void Mount(Scope scope);
    }
}
