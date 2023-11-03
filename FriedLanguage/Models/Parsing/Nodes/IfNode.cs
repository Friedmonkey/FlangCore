using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class IfNode : SyntaxNode
    {
        public IfNode(SyntaxToken startTok) : base(startTok.Position, startTok.Position) { } // We expect the parser to properly define the endpos

        public List<(SyntaxNode cond, SyntaxNode block)> Conditions { get; private set; } = new();

        public override NodeType Type => NodeType.If;

        public override FValue Evaluate(Scope scope)
        {
            int i = 0;
            foreach ((SyntaxNode cond, SyntaxNode block) in Conditions)
            {
                i++;
                var condRes = cond.Evaluate(scope);

                if (condRes.IsTruthy())
                {
                    var newScope = new Scope(scope, StartPosition);
                    var eval = block.Evaluate(newScope);
                    if (newScope.State == ScopeState.ShouldUnmatch)
                    {
                        if (i != Conditions.Count()) //decrese it unless its the last one because then there is nothing to unmatch
                            newScope.SetUnmatchAmount(newScope.UnmatchAmount - 1);
                    }
                    else
                        return eval;
                }
            }

            return FValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var (cond, block) in Conditions)
            {
                yield return cond;
                yield return block;
            }
        }

        internal void AddCase(SyntaxNode cond, SyntaxNode block)
        {
            Conditions.Add((cond, block));
        }

        public override string ToString()
        {
            return "IfNode:";
        }
    }
}
