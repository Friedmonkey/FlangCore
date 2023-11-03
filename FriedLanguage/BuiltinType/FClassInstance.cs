using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FClassInstance : FValue
    {
        public FClass Class { get; set; }
        public List<(string key, FValue val)> InstanceTable { get; set; } = new();
        public bool initialized = false;

        /// <summary>
        /// These are native properties intended to allow native functions/classes to store/read information faster than using InstanceTable.
        /// However, these can not be directly accessed through the Dot operator.
        /// </summary>
        public Dictionary<object, object> NativeProperties { get; set; } = new();

        public override FBuiltinType BuiltinName => FBuiltinType.ClassInstance;

        public FClassInstance() { }
        public FClassInstance(FClass @class)
        {
            Class = @class;
            InstanceTable = Class.InstanceBaseTable.ToList();
        }

        public void CallConstructor(Scope scope, List<FValue> args)
        {
            var ctor = Dot(new FString("$$ctor"));

            if (ctor.IsNull()) throw new Exception("Class " + Class.Name + " does not have a constructor and can therefore not be instantiated.");

            var newArgs = new List<FValue> { this };
            newArgs.AddRange(args);

            GlobalState.EnterConstructor();
            ctor.Call(scope, args);
            GlobalState.ExitConstructor();
            //scope.ConstructorScope = false;
            this.initialized = true;

            if (scope.State != ScopeState.None)
            {
                scope.SetState(ScopeState.None);
                scope.SetReturnValue(FValue.Null);
            }
        }

        public override FString ToSpagString()
        {
            var toStringVal = GetValue("$$toString");

            if (toStringVal is not FBaseFunction toStringFunc)
            {
                return new FString("<instance of class " + Class.Name + ">");
            }
            else
            {
                // TODO: Find a solution to pass the scope; maybe keep a "DefiningScope" on each value?
                // For now, just use an empty scope
                //  SUBTODO: If this is done, dont forget to reset Scope.State!
                var ret = toStringFunc.Call(new Scope(0), new() { this });

                if (ret is not FString str) throw new Exception("A classes toString function must return a string!");
                return str;
            }
        }

        public override string ToString()
        {
            return $"<FClassInstance ClassName={Class.Name}>";
        }

        public override FValue Dot(FValue other,SyntaxToken Token = default, Scope scope = default)
        {
            if (other is not FString key) throw NotSupportedBetween(other, "Dot");

            var val = GetValue(key.Value,scope);
            if (Class.Strict && val.IsNull()) throw new Exception($"Property {other.ToSpagString().Value} not on position {Token.Position} found!");

            return val;
        }


        public override FValue DotAssignment(FValue key, FValue other, Scope scope = null)
        {
            if (key is not FString keyVal) throw NotSupportedBetween(key, "DotAssignment");

            // todo: get rid of the code duplication
            foreach (var kvp in Class.StaticTable)
            {
                if (kvp.key == keyVal.Value)
                {
                    if (kvp.val.IsConstant) throw new Exception($"Tried to overwrite constant value {kvp.key}!");

                    InstanceTable.Remove(kvp);
                    InstanceTable.Add((keyVal.Value, other));
                    return other;
                }
            }

#warning might be a breaking change cus i added the if initalized first
#warning commented this out idk why it wss here, prob for good reason
            //if (initialized)
            //{
             foreach (var kvp in InstanceTable)
             {
                 if (kvp.key == keyVal.Value)
                 {
                     if (kvp.val.IsConstant && !GlobalState.IsInConstructor()) throw new Exception($"Tried to overwrite constant value {kvp.key}!");


                     kvp.val.CopyMeta(ref other);
                     InstanceTable.Remove(kvp);
                     InstanceTable.Add((keyVal.Value, other));
                     return other;
                 }
             }
            //}

            bool success = SetValue(keyVal.Value,other,scope);

            if (Class.Strict && !success) throw new Exception($"Property {key.ToSpagString().Value} not found!");
            InstanceTable.Add((keyVal.Value, other));
            return other;
        }

        public FValue GetValue(string key, Scope scope = null)
        {
            if (scope != null && !initialized)
            {
                var clas = scope.Get(Class.Name);
                if (clas is FClass fclass)
                {
                    Class = fclass;
                    InstanceTable = fclass.InstanceBaseTable.ToList();
                }
                initialized = true;
            }
            foreach (var kvp in Class.StaticTable)
            {
                if (kvp.key == key) return kvp.val;
            }

            foreach (var kvp in InstanceTable)
            {
                if (kvp.key == key) return kvp.val;
            }

            return FValue.Null;
        }
        public bool SetValue(string key,FValue value, Scope scope = null)
        {
            if (scope != null && !initialized)
            {
                var clas = scope.Get(Class.Name);
                if (clas is FClass fclass)
                {
                    Class = fclass;
                    InstanceTable = fclass.InstanceBaseTable.ToList();
                }
                initialized = true;
            }
            foreach (var kvp in Class.StaticTable)
            {
                if (kvp.key == key)
                {
                    int index = Class.StaticTable.IndexOf(kvp);
                    Class.StaticTable[index] = new(kvp.key,value);
                    return true;
                }
            }

            foreach (var kvp in InstanceTable)
            {
                if (kvp.key == key)
                {
                    int index = InstanceTable.IndexOf(kvp);
                    InstanceTable[index] = new(kvp.key, value);
                    return true;
                }
            }

            return false;
        }

        public override bool IsTruthy()
        {
            return true;
        }

        public override FValue Add(FValue other, Scope scope = null)
        {
            var overload = GetValue("$$op+",scope);
            if (overload == null) base.Add(other);

            SyntaxToken cToken = new SyntaxToken(SyntaxType.Keyword, "", "Overload Add");


            var ret = overload.Call(scope, new List<FValue>() { this, other },cToken); // TODO: Use proper scope; dont forget to reset state then
            return ret;
        }

        public override FValue Sub(FValue other, Scope scope = null)
        {
            var overload = GetValue("$$op-",scope);
            if (overload == null) base.Sub(other);

            SyntaxToken cToken = new SyntaxToken(SyntaxType.Keyword, "", "Overload Subtract");


            var ret = overload.Call(scope, new List<FValue>() { this, other },cToken); // TODO: Use proper scope; dont forget to reset state then
            return ret;
        }

        public override FValue Mul(FValue other, Scope scope = null)
        {
            var overload = GetValue("$$op*",scope);
            if (overload == null) base.Mul(other);

            SyntaxToken cToken = new SyntaxToken(SyntaxType.Keyword, "", "Overload Multiply");


            var ret = overload.Call(scope, new List<FValue>() { this, other },cToken); // TODO: Use proper scope; dont forget to reset state then
            return ret;
        }

        public override FValue Div(FValue other, Scope scope = null)
        {
            var overload = GetValue("$$op/",scope);
            if (overload == null) base.Div(other);

            SyntaxToken cToken = new SyntaxToken(SyntaxType.Keyword,"","Overload Divide");

            var ret = overload.Call(scope, new List<FValue>() { this, other },cToken); // TODO: Use proper scope; dont forget to reset state then
            return ret;
        }
    }
}
