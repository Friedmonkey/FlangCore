using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLang.NativeLibraries
{
    public partial class Popups : LanguageExtention
    {
        public override List<FlangClass> InjectClassus()
        {
            List<FlangClass> classes = new List<FlangClass>();

            //var popupClass = new FlangClass("Popup", extends: true,
            //methods: new FlangMethod[]
            //{
            //    new FlangMethod("count",String.Count,ClassOptions.Extend),
            //    new FlangMethod("toint",String.ToInt,ClassOptions.Extend,"default"),
            //    new FlangMethod("split",String.Split,ClassOptions.Extend,"split"),
            //    new FlangMethod("replace",String.Replace,ClassOptions.Extend,"replacing","replacement")
            //});

            //var popupTypeClass = new FlangClass("", extends: true,
            //methods: new FlangMethod[]
            //{
            //                new FlangMethod("count",String.Count,ClassOptions.Extend),
            //                new FlangMethod("toint",String.ToInt,ClassOptions.Extend,"default"),
            //                new FlangMethod("split",String.Split,ClassOptions.Extend,"split"),
            //                new FlangMethod("replace",String.Replace,ClassOptions.Extend,"replacing","replacement")
            //});

            //classes.Add(popupClass);
            return classes;
        }
    }
}
