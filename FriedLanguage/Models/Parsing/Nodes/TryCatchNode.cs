using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class TryCatchNode : SyntaxNode
    {
        public TryCatchNode(SyntaxToken startTok) : base(startTok.Position, startTok.Position) { } // We expect the parser to properly define the endpos

        //public List<SyntaxNode> Blocks { get; private set; } = new();
        public SyntaxNode TryBlock { get; private set; } = null;
        public SyntaxNode CatchBlock { get; private set; } = null;
        public KeyValuePair<SyntaxToken, FValue> CatchBlockException { get; private set; } = default;
        public bool hasCatchBlockException { get; private set; } = false;

        public override NodeType Type => NodeType.TryCatch;

        public override FValue Evaluate(Scope scope)
        {
            try
            {
                var scop = new Scope(scope, StartPosition);
                var output = TryBlock.Evaluate(scop);
                if (scope.State == ScopeState.ShouldUnmatch)
                    throw new Exception("unmatch was called");
                return output;
            }
            catch (Exception ex)
            {
                if (hasCatchBlockException)
                { 
                    if (CatchBlockException.Value is FException fex)
                        fex.Message = ex.Message;
                    return CatchBlock.Evaluate(new Scope(scope, StartPosition,CatchBlockException));
                }
                else
                    return CatchBlock.Evaluate(new Scope(scope, StartPosition));

            }
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return TryBlock;
            yield return CatchBlock;
        }
        internal void SetTryBlock(SyntaxNode block)
        {
            TryBlock = block;
        }
        internal void SetCatchBlock(SyntaxNode block,bool hasArg,SyntaxToken ident = default)
        {
            CatchBlock = block;
            hasCatchBlockException = hasArg;
            CatchBlockException = new(ident, new FException());
        }

        public override string ToString()
        {
            return "TryCatchNode:";
        }
    }
}
