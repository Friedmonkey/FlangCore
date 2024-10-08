﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLang.NativeLibraries
{
    public partial class Lang : LanguageExtention
    {
        public override List<FlangClass> InjectClassus()
        {
            List<FlangClass> classes = new List<FlangClass>();



            var dateTimeClass = new FlangClass("DateTime",
            methods: new FlangMethod[]
            {
                            new FlangMethod("count",String.Count),
                            new FlangMethod("toint",String.ToInt,ClassOptions.Extend,"int default"),
                            new FlangMethod("split",String.Split,ClassOptions.Extend,"string split"),
                            new FlangMethod("join",String.Join,ClassOptions.Extend|ClassOptions.Static,"string char","list lst"),
                            new FlangMethod("format",String.Format,ClassOptions.Extend|ClassOptions.Static,"string formatable","list lst"),
                            new FlangMethod("replace",String.Replace,ClassOptions.Extend,"string replacing","string replacement")
            });

            var stringClass = new FlangClass("String", extends: true,
            methods: new FlangMethod[]
            {
                new FlangMethod("count",String.Count,ClassOptions.Extend),
                new FlangMethod("toint",String.ToInt,ClassOptions.Extend,"int default"),
                new FlangMethod("split",String.Split,ClassOptions.Extend,"string split"),
                new FlangMethod("join",String.Join,ClassOptions.Extend|ClassOptions.Static,"string char","list lst"),
                new FlangMethod("format",String.Format,ClassOptions.Extend|ClassOptions.Static,"string formatable","list lst"),
                new FlangMethod("replace",String.Replace,ClassOptions.Extend,"string replacing","string replacement"),
                new FlangMethod("contains",String.Contains,ClassOptions.Extend,"string contains"),
                new FlangMethod("toUpper",String.ToUpper,ClassOptions.Extend),
                new FlangMethod("toLower",String.ToLower,ClassOptions.Extend),
                new FlangMethod("trim",String.Trim,ClassOptions.Extend),
                new FlangMethod("startsWith",String.StartsWith,ClassOptions.Extend,"string with"),
                new FlangMethod("endsWith",String.EndsWith,ClassOptions.Extend,"string with"),
                new FlangMethod("substring",String.SubString,ClassOptions.Extend,"int start","int length"),
            });
            var intClass = new FlangClass("Int", extends: true,
            methods: new FlangMethod[]
            {
                new FlangMethod("tostring",Int.ToString,ClassOptions.Extend),
            });
            var listClass = new FlangClass("List", extends: true,
            methods: new FlangMethod[]
            {
                new FlangMethod("first",List.First,ClassOptions.Extend),
                new FlangMethod("last",List.Last,ClassOptions.Extend),
                new FlangMethod("add",List.Add,ClassOptions.Extend,"object item"),
                new FlangMethod("remove",List.Remove,ClassOptions.Extend,"object item"),
                new FlangMethod("removeAt",List.RemoveAt,ClassOptions.Extend,"int index"),
                new FlangMethod("contains",List.Contains,ClassOptions.Extend,"object item"),
                new FlangMethod("count",List.Count,ClassOptions.Extend),
                new FlangMethod("clear",List.Clear,ClassOptions.Extend),
                new FlangMethod("sort",List.Sort,ClassOptions.Extend),
                new FlangMethod("reverse",List.Reverse,ClassOptions.Extend),
                new FlangMethod("create",List.Create,ClassOptions.Static),
            });
            var dictClass = new FlangClass("Dict", extends: true,
            methods: new FlangMethod[]
            {
                new FlangMethod("first",Dict.First,ClassOptions.Extend),
                new FlangMethod("last",Dict.Last,ClassOptions.Extend),
                new FlangMethod("add",Dict.Add,ClassOptions.Extend,"object key","object value"),
                new FlangMethod("get",Dict.Get,ClassOptions.Extend,"object key"),
                new FlangMethod("remove",Dict.Remove,ClassOptions.Extend,"object key"),
                new FlangMethod("removeAt",Dict.RemoveAt,ClassOptions.Extend,"int index"),
                new FlangMethod("containsKey",Dict.ContainsKey,ClassOptions.Extend,"object key"),
                new FlangMethod("containsValue",Dict.ContainsValue,ClassOptions.Extend,"object value"),
                new FlangMethod("count",Dict.Count,ClassOptions.Extend),
                new FlangMethod("toString",Dict.ToString,ClassOptions.Extend),
                new FlangMethod("getKeys",Dict.GetKeys,ClassOptions.Extend),
                new FlangMethod("getValues",Dict.GetValues,ClassOptions.Extend),
                new FlangMethod("clear",Dict.Clear,ClassOptions.Extend),
                new FlangMethod("create",Dict.Create,ClassOptions.Static),
            });

            classes.Add(stringClass);
            classes.Add(intClass);
            classes.Add(listClass);
            classes.Add(dictClass);
            return classes;
        }
    }
}
