using FriedLanguage.BuiltinType;
using FriedLanguage.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.CodeAnalysis.CSharp.Scripting;
//using Microsoft.CodeAnalysis.Scripting;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class CsLiteral : SyntaxNode
    {
        private SyntaxToken CsLit;
            
        public CsLiteral(SyntaxToken csLit)
            : base(csLit.Position, csLit.EndPosition) // either nametoken start, args start or finally block start
        {
            this.CsLit = csLit;
        }

        public override NodeType Type => NodeType.CsBlock;

        public override FValue Evaluate(Scope scope)
        {
            throw new Exception("CS Code generation is not supported on this platform.");
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return null;
        }
    }
}
