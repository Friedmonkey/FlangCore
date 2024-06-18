using FriedLang.NativeLibraries;
using FriedLanguage;
using FriedLanguage.BuiltinType;
using FriedLanguage.Extentions;
using FriedLanguage.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static FriedLang.NativeLibraries.Lang;

namespace FriedLang
{
    public class FLang
	{
		Interpreter _interpreter;
        public string LastError = "A 'return','break' or 'unmatch' made the flang runtime end too soon!";
		public FLang()
		{
			_interpreter = new Interpreter();
			_interpreter.GlobalScope.SetAdmin("imports",new FClass("imports",true,true));
            this.AddMethod(new FlangMethod("FlangFormat", Lang.String.FormatBase, "string formatable", "list lst"));
        }
        public void AddNatives()
		{
			ImportNative<IO>("io");
			ImportNative<Lang>("lang");
			ImportNative<Async>("async");
		}
		#region varibles
		public bool AddVariable(string Name, object Value) 
		{
            if (IsDictionary(Value.GetType()))
            {
                throw new Exception("You cant add dictionaries this way, use AddDictionary<Key,Value>(dictionary) instead!");
            }
            var FriedVar = ToFriedVar(Value);

			if (FriedVar == null)
				return false;
			
			_interpreter.GlobalScope.Set(Name,FriedVar);

			return true;
        }
        public bool AddDictionary<Key,Value>(string Name, Dictionary<Key,Value> Dict)
        {
            Dictionary<object, object> Dictionary = new Dictionary<object, object>();

            foreach (var kvp in Dict)
            {
                Dictionary.Add(kvp.Key, kvp.Value);
            }

            if (!IsDictionary(Dictionary.GetType()))
            {
                throw new Exception("This isnt a dictionary!!!");
            }

			var Keys = ToFriedList(Dictionary.Keys);
			var Values = ToFriedList(Dictionary.Values);

            if (Keys == null || Values == null)
                return false;

			FDictionary dict = new(Keys,Values);

            _interpreter.GlobalScope.Set(Name, dict);

            return true;
        }
        public static (List<FValue>,List<FValue>) ToFriedDictionary(IEnumerable<(object,object)> list)
        {

            List<FValue> Keys = new List<FValue>();
            List<FValue> Values = new List<FValue>();

            //foreach (var kvp in list)
            //{
            //    Dictionary.Add(kvp.Key, kvp.Value);
            //}

            //if (!IsDictionary(Dictionary.GetType()))
            //{
            //    throw new Exception("This isnt a dictionary!!!");
            //}

            //var Keys = ToFriedList(Dictionary.Keys);
            //var Values = ToFriedList(Dictionary.Values);

            //if (Keys == null || Values == null)
            //    return false;

            //FDictionary dict = new(Keys, Values);

            //_interpreter.GlobalScope.Set(Name, dict);

            return (Keys, Values);
        }
        public static List<FValue> ToFriedList(IEnumerable<object> List) 
		{
			var output = new List<FValue>();
            foreach (object item in List)
            {
                var vr = ToFriedVar(item);
                if (vr != null)
                {
                    output.Add(vr);
                }
            }
			return output;
        }
        public static FValue ToFriedVar(object Value)
		{
            if (Value is null)
                return new FNull();
            if (Value is string value1)
				return new FString(value1);
			if (Value is int value2)
				return new FInt(value2);
			if (Value is bool value3)
				return new FBool(value3);
            if (Value is float value4)
                return new FFloat(value4);
            if (Value is double value5)
                return new FDouble(value5);
            if (Value is long value6)
                return new FLong(value6);


            if (Value is IEnumerable<(object, object)> value8)
            {
                (var keys, var values) = ToFriedDictionary(value8);
                return new FDictionary(keys, values);
            }

			if (Value is IEnumerable<object> value7) 
				return new FList(ToFriedList(value7).ToArray());


            return null;
		}
        public static List<(object, object)> ListFromFriedDictionary(object dict)
        {
            if (dict is not FDictionary fdict)
                return null;

            var Dictionary = ListFromFriedDictionary(fdict.Value);

            return Dictionary;
        }
        public static Dictionary<object, object> FromFriedDictionary(object dict)
        {
            if (dict is not FDictionary fdict)
                return null;


            var Dictionary = FromFriedDictionary(fdict.Value);

            return Dictionary;
        }
        public static List<(object,object)> ListFromFriedDictionary(List<(FValue, FValue)> Dict)
        {
            List<(object, object)> Dictionary = new List<(object, object)>();

            foreach (var (key, value) in Dict)
            {
                Dictionary.Add((FromFriedVar(key), FromFriedVar(value)));
            }
            return Dictionary;
        }
        public static Dictionary<object,object> FromFriedDictionary(List<(FValue,FValue)> Dict)
        {
            Dictionary<object, object> Dictionary = new Dictionary<object, object>();

            foreach (var (key,value) in Dict)
            {
                Dictionary.Add(FromFriedVar(key), FromFriedVar(value));
            }
            return Dictionary;
        }
        public static List<T> FromFriedList<T>(List<FValue> List)
        {
            var output = new List<T>();
            foreach (FValue item in List)
            {
                var vr = FromFriedVar(item);
                if (vr != null && vr is T)
                {
                    output.Add((T)vr);
                }
            }
            return output;
        }
        public static List<object> FromFriedList(List<FValue> List)
        {
            var output = new List<object>();
            foreach (FValue item in List)
            {
                var vr = FromFriedVar(item);
                if (vr != null)
                {
                    output.Add(vr);
                }
            }
            return output;
        }
        public static object FromFriedVar(FValue Value)
        {
            if (Value is FNull)
                return new FNull();
            if (Value is FString value1)
                return value1.Value;
			if (Value is FInt value2)
                return value2.Value;
            if (Value is FBool value3)
                return value3.Value;
            if (Value is FFloat value4)
                return value4.Value;
            if (Value is FDouble value5)
                return value5.Value;
            if (Value is FLong value6)
                return value6.Value;

            if (Value is FList value7)
                return FromFriedList(value7.Value);

            if (Value is FDictionary value8)
                return FromFriedDictionary(value8.Value);

            if (Value is FDynamic value9)
                return FromFriedDictionary(value9.Value);

            return null;
        }
        private static bool IsDictionary(Type type)
        {
			Type[] dictionaryInterfaces =
			{
			  typeof(IDictionary<,>),
			  typeof(System.Collections.IDictionary),
			  typeof(IReadOnlyDictionary<,>),
			};
            return dictionaryInterfaces
             .Any(dictInterface =>
                 dictInterface == type || // 1
                 (type.IsGenericType && dictInterface == type.GetGenericTypeDefinition()) || // 2
                 type.GetInterfaces().Any(typeInterface => // 3
                                          typeInterface == dictInterface ||
                                          (typeInterface.IsGenericType && dictInterface == typeInterface.GetGenericTypeDefinition())));
        }
        #endregion
        #region classus
        public void AddClass(FlangClass clas)
        {
			AddNewClass(_interpreter.GlobalScope,clas.Extends,clas.Name,clas.Methods.ToArray());
        }
        private void AddNewClass(Scope scope, bool extend, string name, params FlangMethod[] methods)
        {
            var @class = new FClass(name);

			addToClass(@class,methods);
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
					@class.StaticTable.Add((method.Name, GenerateMethod(method.Implementation,method.ExpectedArguments.ToArray())));
            }
        }
        #endregion
        #region methods
        public void AddMethod(FlangMethod method)
		{
			AddNewMethod(_interpreter.GlobalScope,true,method.Name,method.Implementation,method.ExpectedArguments.ToArray());
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
		#endregion

		public void ImportMemory(string name, string Code)
		{
			var imports = _interpreter.GlobalScope.Get("imports");
			if (imports is FClass fclass)
			{
				var code = new FString(Code);
				code.IsConstant = true;
				code.TypeIsFixed = true;

				if (!fclass.StaticTable.ExistsIn(name))
					fclass.StaticTable.Add((name, code));
			}
		}
		public void ImportNative<LangExtension>(string Name) where LangExtension : LanguageExtention, new()
		{
			var methodInfo = typeof(LangExtension).GetMethod("Inject", new[] { typeof(Scope), typeof(bool) });
			if (methodInfo != null && methodInfo.ReturnType == typeof(void))
			{
				// Create an instance of the class
				var classInstance = new LangExtension();

				// Create a delegate from the method
				var injectAction = (Action<Scope,bool>)Delegate.CreateDelegate(typeof(Action<Scope,bool>), classInstance, methodInfo);

				_interpreter.GlobalScope.Set("nlimporter$$" + Name, new FNativeLibraryImporter(injectAction));
			}
			else
			{
				throw new NotImplementedException("The method \"Inject\" is not implemented correctly.");
			}
		}

		public object RunCode(string code,bool parseOutput = true)
		{
			TimingInterpreterResult result = new TimingInterpreterResult();
			try
			{
                var friedVar = _interpreter.Interpret(code, ref result);
                if (friedVar == null)
                    return null;
                else
                {
                    if (parseOutput)
                        return FromFriedVar(friedVar);
                    else
                        return friedVar;
                }
			}
			catch (Exception ex)
			{
				//Console.WriteLine("Error: " + ex.Message);
                LastError = ex.Message;
				return null;
			}
		}
	}
}
