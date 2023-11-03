using FriedLanguage.BuiltinType;
using FriedLanguage.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class MemoryImportNode : SyntaxNode
    {
        private SyntaxToken ident;
        private SyntaxNode expr;
        public bool Extend { get; protected set; } = false;

        public MemoryImportNode(SyntaxToken ident,SyntaxNode expr,bool extend = false) : base(ident.Position, expr.EndPosition)
        {
            this.Ident = ident;
            this.Expr = expr;
            this.Extend = extend;
        }

        public override NodeType Type => NodeType.Import;

        public SyntaxToken Ident { get => ident; set => ident = value; }
        public SyntaxNode Expr { get => expr; set => expr = value; }

        public override FValue Evaluate(Scope scope)
        {
            string text;
            var val = Expr.Evaluate(scope);
            if (val is FString fstring)
            {
                text = fstring.Value;
            }
            else
            {
                throw new Exception($"Expected string at memory import on position {ident.Position} got {val.BuiltinName}");
            }


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
                scope.SetAdmin("returnValue", returnValue);


                // copy export table

                foreach (var kvp in ip.GlobalScope.ExportTable)
                {
                    if (scope.Get(kvp.Key) != null) 
                    {
                        bool exportExtends = kvp.Value.Value;
                        if (exportExtends || Extend)
                        {
                            var existing = scope.Get(kvp.Key);
                            var newOne = kvp.Value.Key;
                            if (existing is FClass existingClass && newOne is FClass newClass)
                            {
                                var merge = new FClass(existingClass,newClass, true);
                                scope.SetAdmin(kvp.Key, merge);
                            }
                            continue;
                        }
                        else
                            throw new Exception($"Failed to import '{Ident.Text}': Import conflict; file exports '{kvp.Key}' but that identifier is already present in the current scope.");
                    }

                    scope.Set(kvp.Key, kvp.Value.Key);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to import '{Ident.Text}': {ex.Message}");
            }

            return res.LastValue;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(Ident);
        }
    }
}
