using FriedLanguage.BuiltinType;
using FriedLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLang
{
    [Flags]
    public enum ClassOptions
    {
        None = 0,
        Static = 1,
        Extend = 2
    }

    public delegate FValue Implementation(Scope callingScope, List<FValue> arguments);
    public class FlangMethod
    {
        public FlangMethod() { }
        public FlangMethod(FlangMethod source) 
        {
            this.Name = source.Name;
            this.classStatic = source.classStatic;
            this.classExtend = source.classExtend;
            this.Implementation = source.Implementation;

            foreach (var arg in source.ExpectedArguments)
            {
                this.ExpectedArgumentTypes.Add(arg.Split(' ').First());
                this.ExpectedArguments.Add(arg.Split(' ').Last());
            }
        }
        public FlangMethod(string name, Implementation implementation, params string[] expectedArgs)
        {
            this.Name = name;
            this.Implementation = implementation;

            foreach (var arg in expectedArgs)
            {
                this.ExpectedArgumentTypes.Add(arg.Split(' ').First());
                this.ExpectedArguments.Add(arg.Split(' ').Last());
            }
        }
        public FlangMethod(string name, Implementation implementation, ClassOptions options, params string[] expectedArgs)
        {
            this.Name = name;
            this.Implementation = implementation;
            foreach (var arg in expectedArgs)
            {
                this.ExpectedArgumentTypes.Add(arg.Split(' ').First());
                this.ExpectedArguments.Add(arg.Split(' ').Last());
            }

            if ((options & ClassOptions.Static) != 0)
            {
                // Static option is set
                this.classStatic = true;
            }

            if ((options & ClassOptions.Extend) != 0)
            {
                // Extend option is set
                this.classExtend = true;
            }


        }
        public string Name { get; set; }
        public bool classStatic { get; set; }
        public bool classExtend { get; set; }
        public Implementation Implementation { get; set; }
        public List<string> ExpectedArguments { get; set; } = new List<string>();
        public List<string> ExpectedArgumentTypes { get; set; } = new List<string>();
    }
    public class FlangClass
    {

        public FlangClass() { }
        public FlangClass(FlangClass source) 
        {
            this.Name = source.Name;
            this.Extends = source.Extends;
            this.Methods = source.Methods;
            this.Constructor = source.Constructor;
        }
        public FlangClass(string name, params FlangMethod[] methods)
        {
            this.Name = name;
            this.Methods = methods.ToList();
        }
        public FlangClass(string name, FlangMethod constructor, params FlangMethod[] methods)
        {
            this.Name = name;
            this.Constructor = constructor;
            this.Methods = methods.ToList();
        }
        public FlangClass(string name, bool extends, params FlangMethod[] methods)
        {
            this.Name = name;
            this.Extends = extends;
            this.Methods = methods.ToList();
        }
        public FlangClass(string name, bool extends, FlangMethod constructor, params FlangMethod[] methods)
        {
            this.Name = name;
            this.Extends = extends;
            this.Constructor = constructor;
            this.Methods = methods.ToList();
        }



        public string Name { get; set; }
        public bool Extends { get; set; }
        public List<FlangMethod> Methods { get; set; } = new List<FlangMethod>();
        public FlangMethod Constructor { get; set; } = new FlangMethod() { Name = "$$ctor" };
    }
}
