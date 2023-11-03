using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class BlockNode : SyntaxNode
    {
        private List<SyntaxNode> nodes;
        private readonly bool createNewScope;

        public BlockNode(SyntaxToken startTok, SyntaxToken endTok, List<SyntaxNode> nodes, bool createNewScope = true) : base(startTok.Position, endTok.Position)
        {
            this.Nodes = nodes;
            this.createNewScope = createNewScope;
        }

        public override NodeType Type => NodeType.Block;

        public List<SyntaxNode> Nodes { get => nodes; set => nodes = value; }

        public override FValue Evaluate(Scope scope)
        {
            var lastVal = FValue.Null;
            var blockScope = scope;

            if (createNewScope) blockScope = new Scope(scope, StartPosition);

            //evaluate labels first
            foreach (var node in Nodes)
            {
				if (node is not LabelNode)
					continue;
                node.Evaluate(blockScope);
			}

            for (int i = 0; i < Nodes.Count(); i++)
            {
                var node = Nodes[i];
                //skip labels we already evaluated
                if (node is LabelNode || node is null)
                    continue;
                var res = node.Evaluate(blockScope);

                if (!res.IsNull())
                {
                    lastVal = res;
                }

                if (scope.BreakAmount >= 1)
                { 
                    scope.SetState(ScopeState.ShouldBreak);
                    scope.SetBreakAmount(scope.BreakAmount);
                }
                else if (scope.State == ScopeState.ShouldBreak)
                    scope.SetState(ScopeState.None);

                if (scope.UnmatchAmount >= 1)
                {
                    scope.SetState(ScopeState.ShouldUnmatch);
                    scope.SetUnmatchAmount(scope.UnmatchAmount);
                }
                else if (scope.State == ScopeState.ShouldUnmatch)
                    scope.SetState(ScopeState.None);

                if (scope.State == ScopeState.ShouldBreak
                    || scope.State == ScopeState.ShouldContinue || scope.State == ScopeState.ShouldUnmatch) return lastVal;

                if (blockScope.State == ScopeState.ShouldJump)
                {
                    if (blockScope.CreatedPosition == blockScope.JumpToIfCreatedPosition)
                    {
                        i = blockScope.JumpTo;
                        scope.SetState(ScopeState.None);
                        continue;
                    }
                    else //finish this code block
                    {
                        return lastVal;
                    }
                }


                if (scope.State == ScopeState.ShouldReturn)
                {
                    //Debug.WriteLine("Returning from block node at range " + StartPosition + ".." + EndPosition + " with value " + scope.ReturnValue.ToString());
                    var v = scope.ReturnValue;
                    return v;
                }
            }
            //foreach (var node in Nodes)
            //{
            //}

            return lastVal;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var node in Nodes) yield return node;
        }

        public override string ToString()
        {
            return "BlockNode:";
        }
    }
}
