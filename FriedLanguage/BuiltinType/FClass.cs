using FriedLanguage.BuiltinType;
using FriedLanguage.Extentions;
using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.BuiltinType
{
    public class FClass : FValue
    {
        public List<(string key, FValue val)> StaticTable { get; set; } = new();
        public List<(string key, FValue val)> InstanceBaseTable { get; set; } = new();

        public FClass(FClass clas1, FClass clas2, bool overwrite1From2)
        {
            if (clas1.Name != clas2.Name)
                return;
            var clas = new FClass(clas1.Name);

            foreach (var n in clas1.StaticTable)
            {
                clas.StaticTable.Add((n.key, n.val));
            }
            foreach (var n in clas1.InstanceBaseTable)
            {
                clas.InstanceBaseTable.Add((n.key, n.val));
            }

            foreach (var n in clas2.StaticTable)
            {
                bool exists = clas.StaticTable.Exists(item => item.key == n.key);
                if (exists && overwrite1From2)
                {
                    var index = clas.StaticTable.FindIndex(item => item.key == n.key);
                    clas.StaticTable[index] = (n.key, n.val);
                }
                else if (!exists)
                {
                    clas.StaticTable.Add((n.key, n.val));
                }
            }

            foreach (var n in clas2.InstanceBaseTable)
            {
                bool exists = clas.InstanceBaseTable.Exists(item => item.key == n.key);
                if (exists && overwrite1From2)
                {
                    var index = clas.InstanceBaseTable.FindIndex(item => item.key == n.key);
                    clas.InstanceBaseTable[index] = (n.key, n.val);
                }
                else if (!exists)
                {
                    clas.InstanceBaseTable.Add((n.key, n.val));
                }
            }

            this.Name = clas.Name;
            this.InstanceBaseTable = clas.InstanceBaseTable;
            this.StaticTable = clas.StaticTable;
        }

        public string Name { get; set; } = "";
        public bool Strict { get; set; } = false;
        public override FBuiltinType BuiltinName => FBuiltinType.Class;

        public FClass()
        {
        }

        public FClass(string name, bool isStrict = false,bool isConst = false)
        {
            Name = name;
            this.Strict = isStrict;
            base.IsConstant = isConst;
        }

        public override FString ToSpagString()
        {
            return new FString("<class " + Name + ">");
        }

        public override string ToString()
        {
            return $"<FClass Name={Name}>";
        }

        public override FValue Dot(FValue other,SyntaxToken Token = default, Scope scope = default)
        {
            if (other is not FString key) throw NotSupportedBetween(other, "Dot");

            foreach (var kvp in StaticTable)
            {
                if (kvp.key == key.Value) return kvp.val;
            }


            throw new Exception($"Identifier \"{Token.Text}\" on position {Token.Position} Does not exist!");
            //return FValue.Null;
            // Fried Change fried friedmonkey
        }

        public override bool IsTruthy()
        {
            return StaticTable != null;
        }
    }
}
