using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public abstract class FBaseFunction : FValue
    {
        /// <summary>
        /// If this is true, the first argument should be the instance
        /// </summary>
        public bool IsClassInstanceMethod { get; set; }

        public List<string> ExpectedArgs { get; set; }
        //private List<string> expectedArgTypes { get; set; }
        public List<string> ExpectedArgTypes { get; set; }
        //{ 
        //    get => expectedArgTypes; 
        //    set => expectedArgTypes = value; 
        //}
        public string ReturnType { get; set; }
    }
}
