using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ImportNode : SyntaxNode
    {
        private SyntaxToken path;
        public bool Extend { get; protected set; } = false;

        public ImportNode(SyntaxToken path,bool extend = false) : base(path.Position, path.EndPosition)
        {
            this.path = path;
            Extend = extend;
        }

        public override NodeType Type => NodeType.Import;

        public override FValue Evaluate(Scope scope)
        {
            if (!File.Exists(path.Text)) throw new Exception($"Failed to import '{path.Text}': File not found");
            var text = File.ReadAllText(path.Text);

            Interpreter ip = new();
            Scope rootScope = scope.GetRoot();

            // copy available namespaces provided by runtime
            foreach (var kvp in rootScope.Table)
            {
                if (kvp.Key.StartsWith("nlimporter$$"))
                {
                    ip.GlobalScope.Table[kvp.Key] = kvp.Value;
                }
            }

            InterpreterResult res = new();

            try
            {
                FValue returnValue = ip.Interpret(text, ref res);
                scope.SetAdmin("returnValue",returnValue);


                // copy export table

                foreach (var kvp in ip.GlobalScope.ExportTable)
                {
                    if (scope.Get(kvp.Key) != null)
                    {
                        bool exportExtends = kvp.Value.Value;
                        if (exportExtends || Extend)
                        { 
                            scope.SetAdmin(kvp.Key, kvp.Value.Key);
                            continue;
                        }
                        else
                            throw new Exception($"Failed to import '{path.Text}': Import conflict; file exports '{kvp.Key}' but that identifier is already present in the current scope.");
                    }

                    scope.Set(kvp.Key, kvp.Value.Key);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to import '{path.Text}': {ex.Message}");
            }

            return res.LastValue;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(path);
        }
    }
}
