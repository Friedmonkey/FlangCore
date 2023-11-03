using FriedLanguage;
using FriedLanguage.BuiltinType;
using System.Collections.Generic;
using System.Linq;

namespace FriedLang
{
	public abstract class LanguageExtention
	{
		public List<FlangMethod> Methods = new();
		public List<FlangClass> Classus = new();

		public void Inject(Scope rootScope, bool extend)
		{
			Methods.Clear();
			Classus.Clear();

			Classus = InjectClassus();
			Methods = InjectMethods();

			Intercept();

			AddMethods(rootScope, Methods.ToArray());
            AddClassus(rootScope, Classus.ToArray());
		}

        public virtual List<FlangMethod> InjectMethods() { return new List<FlangMethod>(); }
		public virtual List<FlangClass> InjectClassus() { return new List<FlangClass>(); }

        public virtual void Intercept() { }

        #region manipulation
        public bool InterAddMethod(FlangMethod newMethod)
        {
            if (Methods.Contains(newMethod))
                return false;
            Methods.Add(newMethod);
            return true;
        }
        public bool InterRemoveMethod(string methodName)
        {
            var other = Methods.FirstOrDefault(m => m.Name == methodName);
            if (other != null)
            {
                Methods.Remove(other);
                return true;
            }
            return false;
        }
        public bool InterAddClass(FlangClass newClass)
        {
            if (Classus.Contains(newClass))
                return false;
            Classus.Add(newClass);
            return true;
        }
        public bool InterRemoveClass(string className)
        {
            var other = Classus.FirstOrDefault(m => m.Name == className);
            if (other != null)
            {
                Classus.Remove(other);
                return true;
            }
            return false;
        }
        public bool InterReplaceClass(string className, FlangClass newClass)
        {
            var other = Classus.FirstOrDefault(m => m.Name == className);
            if (other != null)
            {
                Classus.Remove(other);
                Classus.Add(newClass);
                return true;
            }
            return false;
        }
        public bool InterReplaceMethod(string className,FlangMethod newMethod) 
		{
            var other = Methods.FirstOrDefault(m => m.Name == className);
            if (other != null)
            {
                Methods.Remove(other);
                Methods.Add(newMethod);
                return true;
            }
            return false;
        }
        public bool InterReplaceMethod(string className, Implementation implementation)
        {
            var other = Methods.FirstOrDefault(m => m.Name == className);
            if (other != null)
            {
                Methods.Remove(other);

                var newer = new FlangMethod(other);
                newer.Implementation = implementation;
                Methods.Add(newer);
                return true;
            }
            return false;
        }
        public bool InterReplaceMethod(string name, Implementation implementation,params string[] expectedArgs)
        {
            var other = Methods.FirstOrDefault(m => m.Name == name);
            if (other != null)
            {
                Methods.Remove(other);

                var newer = new FlangMethod(other);
                newer.Implementation = implementation;
                newer.ExpectedArguments = expectedArgs.ToList();
                Methods.Add(newer);
                return true;
            }
            return false;
        }
        #endregion
        #region classus
        public void AddClassus(Scope scope, FlangClass[] clasus)
        {
			foreach (var clas in clasus)
			{
				AddClass(scope,clas);
			}
        }
        public void AddClass(Scope scope, FlangClass clas)
        {
            AddNewClass(scope, clas.Extends, clas.Name, clas.Methods.ToArray());
        }
        private void AddNewClass(Scope scope, bool extend, string name, params FlangMethod[] methods)
        {
            var @class = new FClass(name);

            addToClass(@class, methods);
            if (extend)
                scope.SetAdmin(name, @class);
            else
                scope.Set(name, @class);
        }
        private void addToClass(FClass @class, params FlangMethod[] methods)
        {
            foreach (var method in methods)
            {

                if (method.classStatic)
                {
                    if (method.classExtend)
                        @class.StaticTable.Add((method.Name, GenerateExtendMethod(method.Implementation, method.ExpectedArguments.ToArray())));
                    else
                        @class.StaticTable.Add((method.Name, GenerateMethod(method.Implementation, method.ExpectedArguments.ToArray())));
                }
                else
                { 
                    if (method.classExtend)
                        @class.InstanceBaseTable.Add((method.Name, GenerateExtendMethod(method.Implementation, method.ExpectedArguments.ToArray())));
                    else
                        @class.InstanceBaseTable.Add((method.Name, GenerateMethod(method.Implementation, method.ExpectedArguments.ToArray())));
                }
            }
        }
        #endregion
        #region methods
        public void AddMethods(Scope scope, params FlangMethod[] methods)
        {
			foreach (var method in methods)
			{
				AddMethod(scope, method);
			}
        }
        public void AddMethod(Scope scope, FlangMethod method)
        {
            AddNewMethod(scope, true, method.Name, method.Implementation,method.ExpectedArguments.ToArray());
        }
        private void AddNewMethod(Scope scope, bool extend, string name, Implementation implementation, params string[] expectedArguments)
        {
            if (extend)
                scope.SetAdmin(name, GenerateMethod(implementation, expectedArguments));
            else
                scope.Set(name, GenerateMethod(implementation, expectedArguments));
        }
		private FNativeFunction GenerateMethod(Implementation implementation, params string[] ExpectedArguments)
		{
			return new FNativeFunction(implementation.Invoke, ExpectedArguments.ToList());
		}
        private FNativeFunction GenerateExtendMethod(Implementation implementation, params string[] ExpectedArguments)
        {
            var expected = new List<string> { "this" };
            expected.AddRange(ExpectedArguments);
            return new FNativeFunction(implementation.Invoke, expected,true);
        }
        #endregion

    }
}
