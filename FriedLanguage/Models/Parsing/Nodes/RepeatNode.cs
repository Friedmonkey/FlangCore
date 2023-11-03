using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class RepeatNode : SyntaxNode
    {
        private SyntaxNode timesExpr;
        private SyntaxNode block;
        private bool keepScope = false;

        public RepeatNode(SyntaxNode timesExpr, SyntaxNode block, bool keepScope = false) : base(timesExpr.StartPosition, block.EndPosition)
        {
            this.timesExpr = timesExpr;
            this.block = block;
            this.keepScope = keepScope;
        }

        public override NodeType Type => NodeType.Repeat;

        public override FValue Evaluate(Scope scope)
        {
            var timesRaw = timesExpr.Evaluate(scope);
            if (timesRaw is not FInt timesSInt) throw new Exception("Repeat x times expression must evaluate to FInt");
            var times = timesSInt.Value;

            if (keepScope)
            {
                if (block is not BlockNode blockNode) throw new Exception("Kept-scope repeat expressions must have a full body.");

                for (int i = 0; i < times; i++)
                {
                    foreach (var n in blockNode.Nodes)
                    {
                        n.Evaluate(scope);
                    }
                }
            }
            else
            {
                for (int i = 0; i < times; i++)
                {
                    block.Evaluate(scope);
                }
            }

            return FValue.Null;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return timesExpr;
            yield return block;
        }
    }
}
