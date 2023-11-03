using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models
{
#warning keywords are here
    public static class SyntaxFacts
    {
        public static void ClassifyIdentifierAsKeyword(ref SyntaxToken token)
        {
            if (token.Text.ToString() 
			#region existing
                                       is "import" or "native" or "export" 

                                       or "return" or "continue" or "break" 

                                       or "if" or "elseif" or "else" 

                                       or "for" or "while" 
                                       or "repeat" or "times" 

                                       //or "func" 
                                       //or "prop" 
                                       or "var" or "class" 
                                       //or "op" 

                                       or "static" or "const" 
                                       or "new" 
									   //"self" is a unofficial keyword
			#endregion

			#region  mine
									   //"this" its not an officiel keyword but its used for extending

									   or "memory"
									   or "csharp"

                                       //or "int"
                                       //or "float"
                                       //or "double"
                                       //or "long"
                                       //or "string"
                                       //or "bool"
                                       //or "object"
                                       //or "list"
                                       //or "dictionary"
                                       or "overload" 

                                       or "extend"
                                       
                                       or "try" or "catch" 
                                       or "throw"

                                       or "label" or "goto"
                                       
                                       or "switch" or "case"
                                       or "default"

									   or "strict" or "override"

                                       or "foreach" or "in"

                                       or "keyword" or "disable" or "enable"

                                       or "unmatch"

            #endregion
                                       )
			{
                token.Type = SyntaxType.Keyword;
            }
        }
        public static void ClassifyIdentifierAsLogical(ref SyntaxToken token)
        {
            var str = token.Text.ToString();
            switch (str)
            {
                case "becomes":
                    token.Type = SyntaxType.Equals;
                    Console.WriteLine("ClassifyIdentifierAsLogical matched becomes");
                    break;
                case "is":
                    token.Type = SyntaxType.EqualsEquals;
                    Console.WriteLine("ClassifyIdentifierAsLogical matched is");
                    break;
                case "or":
                    token.Type = SyntaxType.OrOr;
                    Console.WriteLine("ClassifyIdentifierAsLogical matched or");
                    break;
                case "and":
                    token.Type = SyntaxType.AndAnd;
                    Console.WriteLine("ClassifyIdentifierAsLogical matched and");
                    break;
                case "not":
                    token.Type = SyntaxType.Bang;
                    Console.WriteLine("ClassifyIdentifierAsLogical matched not");
                    break;
                case "then":
                    token.Type = SyntaxType.LBraces;
                    Console.WriteLine("ClassifyIdentifierAsLogical matched then");
                    break;
                case "stop":
                    token.Type = SyntaxType.RBraces;
                    Console.WriteLine("ClassifyIdentifierAsLogical matched stop");
                    break;
                default:
                    break;
            }
        }
    }
}
