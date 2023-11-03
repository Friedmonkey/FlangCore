using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLang.NativeLibraries
{
    public partial class Async : LanguageExtention
    {
        public override List<FlangMethod> InjectMethods()
        {
            return new List<FlangMethod> { new FlangMethod("wait",Global.Wait,ClassOptions.Static, "int time") };
        }
    }
}
