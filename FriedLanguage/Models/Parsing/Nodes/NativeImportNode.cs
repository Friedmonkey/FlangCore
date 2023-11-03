using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class NativeImportNode : SyntaxNode
    {
        private SyntaxToken ident;
        public bool Extend { get; protected set; } = false;


        public NativeImportNode(SyntaxToken ident, bool extend = false) : base(ident.Position, ident.EndPosition)
        {
            this.ident = ident;
            this.Extend = extend;
        }

        public override NodeType Type => NodeType.NativeImport;

        public override FValue Evaluate(Scope scope)
        {
            if (ident.Text == "all")
            {
                var rootScope = scope.GetRoot();

                foreach (var kvp in rootScope.Table.ToList())
                {
                    if (kvp.Key.StartsWith("nlimporter$$"))
                    {
                        if (kvp.Value is not FNativeLibraryImporter importerFromAllLoop) throw new Exception("Found unexpexted type in root tables nlimporters!");
                        importerFromAllLoop.Import(scope,Extend);
                    }
                }

                return FValue.Null;
            }

            var val = scope.Get("nlimporter$$" + ident.Text);

            if (val == null || val is not FNativeLibraryImporter importer)
            {
                throw new Exception("Native library " + ident.Text + " not found!");
            }

            importer.Import(scope,Extend);
            return FValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(ident);
        }
    }
}
