using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ClassFunctionDefinitionNode : SyntaxNode
    {
        private SyntaxToken name;
        private List<SyntaxToken> args;
        private List<SyntaxToken> argTypes;
        private SyntaxToken returnType;
        private SyntaxNode body;
        private bool isStatic;

        public ClassFunctionDefinitionNode(SyntaxToken name, List<SyntaxToken> args, List<SyntaxToken> argTypes,SyntaxToken returnType, SyntaxNode body, bool isStatic) : base(name.Position, body.EndPosition)
        {
            this.name = name;
            this.args = args;
            this.argTypes = argTypes;
            this.returnType = returnType;
            this.body = body;
            this.isStatic = isStatic;
        }

        public override NodeType Type => NodeType.ClassFunctionDefinition;

        public override FValue Evaluate(Scope scope)
        {
            var targetName = name.Text;

            if (targetName is "ctor" or "toString")
            {
                /*if(args.Where(v => v.Text == "self").Count() != 1) {
                    throw new Exception($"Special class method '{targetName}' must contain the argument 'self' exactly once.");
                }*/

                targetName = "$$" + targetName;
            }

            var f = new FFunction(scope, targetName, args.Select((v) => v.Text).ToList(), argTypes.Select((v) => v.Text).ToList(),returnType.Text, body);
            f.IsClassInstanceMethod = !isStatic;
            return f;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(name);
            foreach (var tok in args) yield return new TokenNode(tok);
            yield return body;
        }
    }
}
