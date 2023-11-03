using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing
{
    public abstract class SyntaxNode
    {
        public virtual int StartPosition { get; internal set; }
        public virtual int EndPosition { get; internal set; }

        public abstract NodeType Type { get; }

        public abstract FValue Evaluate(Scope scope);
        public abstract IEnumerable<SyntaxNode> GetChildren();

        public SyntaxNode(int startPos, int endPos)
        {
            StartPosition = startPos;
            EndPosition = endPos;
        }
    }
}
