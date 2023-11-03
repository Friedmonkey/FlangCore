using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing
{
    public class ParsingException : Exception
    {
        public override string Message => GetMessage();

        private string parseMessage;
        private Parser context;
        private int tokenIdx;

        private SyntaxToken CausingToken => context.Tokens[tokenIdx];
        private string SurroundingText => ""; // TODO

        public ParsingException(string parseMessage, Parser context, int tokenIdx = -1)
        {
            this.parseMessage = parseMessage;
            this.context = context;
            this.tokenIdx = tokenIdx == -1 ? context.Position : tokenIdx;
        }

        const string NORMAL = "\u001b[38;5;15m";
        const string ERROR = "\u001b[38;5;160m";

        // TODO: Clean up this shit
        public string GetMessage()
        {
            return $@"{parseMessage}
Position: {CausingToken.Position}";
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
