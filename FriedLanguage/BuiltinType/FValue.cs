using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public abstract class FValue
    {
        public static FValue Null => new FNull();


        public abstract FBuiltinType BuiltinName { get; }


        #region Metadata
        public bool TypeIsFixed { get; set; } = true;
        public bool IsConstant { get; set; } = false;
        public bool IsNullable { get; set; } = false;
        #endregion

        public virtual FValue Add(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("Add");
        }

        public virtual FValue Sub(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("Sub");
        }

        public virtual FValue Mul(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("Mul");
        }

        public virtual FValue Div(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("Div");
        }

        public virtual FValue Mod(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("Mod");
        }

        public virtual FValue Idx(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("Idx");
        }

        public virtual FValue SetIndex(FValue index, FValue newValue)
        {
            throw NotSupportedOn("SetIndex");
        }
        public virtual FValue GetIndex(FValue index)
        {
            throw NotSupportedOn("GetIndex");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual FValue Dot(FValue other,SyntaxToken Token = default, Scope scope = default)
        {
            throw NotSupportedOn("Dot");
        }

        public virtual FValue DotAssignment(FValue key, FValue value, Scope scope = default)
        {
            throw NotSupportedOn("DotAssignment");
        }

        // TODO: Maybe force equals to be implemented?
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual FValue Equals(FValue other,  SyntaxToken calledToken = default, Scope scope = null)
        {
            throw NotSupportedOn("Equals");
        }

        public virtual FValue LessThan(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("LessThan");
        }

        public virtual FValue LessThanEqu(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("LessThanEqu");
        }

        public virtual FValue GreaterThan(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("GreaterThan");
        }

        public virtual FValue GreaterThanEqu(FValue other, Scope scope = null)
        {
            throw NotSupportedOn("GreaterThanEqu");
        }

        public virtual FValue CastToBuiltin(FBuiltinType other)
        {
            throw NotSupportedOn("CastToBuiltin");
        }

        public virtual FValue Call(Scope scope, List<FValue> args, SyntaxToken callerToken = default)
        {
            throw NotExistOn(callerToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool IsNull()
        {
            return false;
        }

        public virtual FValue Not(Scope scope = null)
        {
            throw NotSupportedOn("Not");
        }

        public virtual FValue ArithNot(Scope scope = null)
        {
            throw NotSupportedOn("ArithNot");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool IsTruthy();

        public override string ToString()
        {
            return BuiltinName.ToString();
        }

        public virtual FString ToSpagString()
        {
            return new FString("<unknown of type " + BuiltinName.ToString() + ">");
        }

        protected NotImplementedException NotSupportedBetween(FValue other, string type)
        {
            return new NotImplementedException(type + " not supported between " + BuiltinName.ToString() + " and " + other.BuiltinName.ToString());
        }

        protected NotImplementedException NotSupportedOn(string type)
        {
            return new NotImplementedException(type + " is not supported on " + BuiltinName.ToString());
        }

		protected NotImplementedException NotExistOn(SyntaxToken token)
		{
			return new NotImplementedException(token.Text + " Does not exist (it hasn't been imported or defined YET)");
		}

		protected ArgumentException CastInvalid(string type)
        {
            return new ArgumentException(BuiltinName.ToString() + " can not be cast to " + type);
        }

        internal void CopyMeta(ref FValue other)
        {
            other.TypeIsFixed = TypeIsFixed;
            other.IsConstant = IsConstant;
            other.IsNullable = IsNullable;
        }

        public string SpagToCsString()
        {
            return ToSpagString().Value;
        }

    }
}
