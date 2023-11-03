using FriedLanguage.BuiltinType;
using FriedLanguage.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class FunctionDefinitionNode : SyntaxNode
    {
        private SyntaxToken? nameToken;
        private List<SyntaxToken> args;
        private List<SyntaxToken> argTypes;
        private SyntaxToken returnType;
        private SyntaxNode block;
        private bool isOverride = false;

        public FunctionDefinitionNode(SyntaxToken? nameToken, List<SyntaxToken> args, List<SyntaxToken> argTypes,SyntaxToken returnType, SyntaxNode block, bool isOverride = false)
            : base(nameToken != null ? nameToken.Value.Position : args.GetStartingPosition(block.StartPosition), block.EndPosition) // either nametoken start, args start or finally block start
        {
            this.nameToken = nameToken;
            this.args = args;
            this.argTypes = argTypes;
            this.returnType = returnType;
            this.block = block;
            this.isOverride = isOverride;
        }

        public override NodeType Type => NodeType.FunctionDefinition;

        public override FValue Evaluate(Scope scope)
        {
            var f = new FFunction(scope, nameToken?.Text ?? "<anonymous>", args.Select((v) => v.Text).ToList(), argTypes.Select((v) => v.Text).ToList(),returnType.Text, block);
            if (nameToken != null)
            { 
                if (isOverride)
                    scope.SetAdmin(nameToken.Value.Text, f);
                else
                    scope.Set(nameToken.Value.Text, f);
            }
            return f;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (nameToken != null) yield return new TokenNode(nameToken.Value);
            foreach (var t in args) yield return new TokenNode(t);
            yield return block;
        }
    }
}
