using FriedLanguage.BuiltinType;
using FriedLanguage.Models;
using FriedLanguage.Models.Parsing.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage
{
    public enum ScopeState
    {
        None,
        ShouldBreak,
        ShouldUnmatch,
        ShouldContinue,
        ShouldReturn,
        ShouldJump
    }
    public class Scope
    {
        public Dictionary<string, FValue> Table { get; set; } = new();
        public Dictionary<string, KeyValuePair<FValue,bool>> ExportTable { get; set; } = new();
        public Scope ParentScope { get; set; }

        //public List<(string name, int pos)> Labels = new();

        public string nowVar = "tnow";
		public int JumpTo = -1;
		public int JumpToIfCreatedPosition = -1;
		public int BreakAmount = -1;
		public int UnmatchAmount = -1;

        public ScopeState State { get; private set; } = ScopeState.None;
        public FValue ReturnValue { get; set; } = FValue.Null;

        public int CreatedPosition { get; private set; }

        public Scope(int startPos)
        {
            CreatedPosition = startPos;
        }

        public Scope(Scope parentScope, int startPos)
        {
            ParentScope = parentScope;
            CreatedPosition = startPos;
        }
        public Scope(Scope parentScope, int startPos, params KeyValuePair<SyntaxToken,FValue>[] varibles)
        {
            ParentScope = parentScope;
            CreatedPosition = startPos;

            foreach (var (token,value) in varibles)
            {
                if (!Table.ContainsKey(token.Text))
                    this.Table.Add(token.Text,value);
            }
        }

        public Scope Clone()
        {
            var newScope = new Scope(CreatedPosition);
            newScope.ParentScope = ParentScope;
            newScope.State = State;
            newScope.ReturnValue = ReturnValue;

            foreach (var (key, value) in Table)
            {
                newScope.Table[key] = value;
            }

            foreach (var (key, value) in ExportTable)
            {
                newScope.ExportTable[key] = value;
            }
            return newScope;
        }

        public FValue Get(string key)
        {
            if (key == nowVar)
                return new FLong(DateTime.Now.Ticks);
            if (Table.TryGetValue(key, out FValue val)) return val;

            if (ParentScope == null) return null;
            return ParentScope.Get(key);
        }
		public void Delete(string key)
		{
			if (!Table.ContainsKey(key)) throw new Exception(
				"INTERNAL: Scope.Delete can not be used to if it doest exist values");
			Table.Remove(key);
		}
		public void Set(string key, FValue value)
        {
            if (Table.ContainsKey(key)) throw new Exception(
                "INTERNAL: Scope.Set can not be used to overwrite values");
            Table[key] = value;
        }
		public void SetAdmin(string key, FValue value)
		{
			Table[key] = value;
		}

		public Exception? Update(string key, FValue value)
        {
            if (Table.TryGetValue(key, out var origVal))
            {
                if (origVal.TypeIsFixed &&
                    origVal.BuiltinName != value.BuiltinName)
                {
                    if (!(value.BuiltinName == FBuiltinType.Null && origVal.IsNullable))
                    {
                        string GotVar = value.BuiltinName.ToString().ToLower();
                        if (GotVar != origVal.BuiltinName.ToString().ToLower())
                        {
                            try
                            {
                                value = value.CastToBuiltin(origVal.BuiltinName);
                            }
                            catch
                            {
                                //throw new InvalidOperationException("Tried to initiliaze a " + origVal.BuiltinName.ToString().ToLower() + " variable with a " + GotVar + " value; this is not permitted. Use var% or object instead.");
                                return new InvalidOperationException("A variables type may not change after initilization (Tried to assign " + value.BuiltinName.ToString().ToLower() + " to " + origVal.BuiltinName.ToString().ToLower() + ")");
                            }
                        }
                    }
                }


                if (origVal.IsConstant) throw new InvalidOperationException("Tried to assign to constant variable.");
                //copy meta data from org to value
                origVal.CopyMeta(ref value);

                //set the name to the new FValue
                Table[key] = value;
                return null;
            }

            if (ParentScope == null) throw new Exception("Could not update field " + key + ": Not found");
            return ParentScope.Update(key, value);
        }
        public Exception? UpdateIndex(string key, FValue index, FValue value)
        {
            if (Table.TryGetValue(key, out var origVal))
            {
                if (origVal.IsConstant) throw new InvalidOperationException("Tried to assign to constant variable.");


                origVal.SetIndex(index,value);


                Table[key] = origVal;
                return null;
            }

            if (ParentScope == null) throw new Exception("Could not update field " + key + ": Not found");
            return ParentScope.UpdateIndex(key,index, value);
        }

        public bool Update(string key, FValue value, out Exception ex)
        {
            var updateEx = Update(key, value);

            if (updateEx == null)
            {
                ex = new Exception();
                return true;
            }
            else
            {
                ex = updateEx;
                return false;
            }
        }
        public bool UpdateIndex(string key, FValue index, FValue value, out Exception ex)
        {
            var updateEx = UpdateIndex(key, index, value);

            if (updateEx == null)
            {
                ex = new Exception();
                return true;
            }
            else
            {
                ex = updateEx;
                return false;
            }
        }

        public Scope GetRoot()
        {
            if (ParentScope == null) return this;
            return ParentScope.GetRoot();
        }

        public void SetState(ScopeState state)
        {
            State = state;
            ParentScope?.SetState(state);
        }
        public void SetBreakAmount(int amount)
        {
            if (amount == -1)
                return;
            BreakAmount = amount;
            ParentScope?.SetBreakAmount(amount);
            //ParentScope?.SetBreakAmount(amount-1);
        }
        public void SetUnmatchAmount(int amount)
        {
            if (amount == -1)
                return;
            UnmatchAmount = amount;
            ParentScope?.SetUnmatchAmount(amount);
            //ParentScope?.SetUnmatchAmount(amount - 1);
        }
        public void SetJumpPos(int pos)
		{
			JumpTo = pos;
			ParentScope?.SetJumpPos(pos);
		}
		public void SetJumpPosCreatedPos(int pos)
		{
			JumpToIfCreatedPosition = pos;
			ParentScope?.SetJumpPosCreatedPos(pos);
		}

		public void SetReturnValue(FValue val)
        {
            ReturnValue = val;
            ParentScope?.SetReturnValue(val);
        }

        /*public string GetScopeStrace() {
            string o = "";
            var current = this;

            while(true) {
                current += ""
            }
        }*/
    }
}
