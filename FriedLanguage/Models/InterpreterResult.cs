using FriedLanguage.BuiltinType;
using FriedLanguage.Models.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models
{
    public struct InterpreterResult
    {
        public List<SyntaxToken> LexedTokens = null;
        public SyntaxNode AST = null;
        public FValue LastValue = null;

        public InterpreterResult() { }
    }

    public struct TimingInterpreterResult
    {
        public InterpreterResult Result = new();

        public double LexTime = 0, ParseTime = 0, EvalTime = 0;

        public TimingInterpreterResult() { }
    }
}
