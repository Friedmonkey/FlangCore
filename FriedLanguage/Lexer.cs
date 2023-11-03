using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage
{
    public class Lexer
    {
        public string Code { get; set; }
        public int Position { get; set; } = 0;
        public char Current => Peek(0);

        //look forward
        public char Peek(int off = 0)
        {
            if (Position + off >= Code.Length || Position + off < 0) return '\0';
            return Code[Position + off];
        }

        public Lexer(string code)
        {
            Code = code;
        }

        public List<SyntaxToken> Lex() 
        {
            List<SyntaxToken> tokens = new List<SyntaxToken>();
            //not null
            while (Current != '\0')
            {
                //generic token
                SyntaxToken insertToken = new SyntaxToken(SyntaxType.BadToken,Position,null,Current.ToString());
                switch (Current)
                {
                    case ';':
                        insertToken = (new(SyntaxType.Semicolon, Position, null, Current.ToString()));
                        break;
                    case '=':
                        if (Peek(1) == '=')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.EqualsEquals, Position, null, "=="));
                        }
                        else if (Peek(1) == '>')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.Arrow, Position, null, "=>"));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.Equals, Position, null, Current.ToString()));
                        }

                        break;
                    case '<':
                        if (Peek(1) == '=')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.LessThanEqu, Position, null, "<="));
                        }
                        else if (Peek(1) == '{')
                        {
                            string text = Current.ToString();
                            Position++;
                            text += Current;
                            Position++;
                            string code = Current.ToString();
                            text += Current;
                            while (!(Peek(2) == '>' && Peek(1) == '}'))
                            {
                                Position++;
                                code+= Current;
                                text+= Current;
                            }
                            Position++;
                            text += Current;
                            Position++;
                            text += Current;
                            insertToken = (new(SyntaxType.CsLit, Position, code, text));

                        }
                        else
                        {
                            insertToken = (new(SyntaxType.LessThan, Position, null, Current.ToString()));
                        }

                        break;
                    case '>':
                        if (Peek(1) == '=')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.GreaterThanEqu, Position, null, ">="));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.GreaterThan, Position, null, Current.ToString()));
                        }

                        break;
                    case '|':
                        if (Peek(1) == '|')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.OrOr, Position, null, "||"));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.BadToken, Position, null, Current.ToString()));
                        }

                        break;
                    case '&':
                        if (Peek(1) == '&')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.AndAnd, Position, null, "&&"));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.BadToken, Position, null, Current.ToString()));
                        }

                        break;
                    case '+':
                        if (Peek(1) == '=')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.PlusEqu, Position, null, "+="));
                        }
                        else if (Peek(1) == '+')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.PlusPlus, Position, null, "++"));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.Plus, Position, null, Current.ToString()));
                        }

                        break;
                    case '-':
                        if (Peek(1) == '=')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.MinusEqu, Position, null, "-="));
                        }
                        else if (Peek(1) == '-')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.MinusMinus, Position, null, "--"));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.Minus, Position, null, Current.ToString()));
                        }

                        break;
                    case '%':
                        if (Peek(1) == '=')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.ModEqu, Position, null, "%="));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.Mod, Position, null, Current.ToString()));
                        }

                        break;
                    case '*':
                        if (Peek(1) == '=')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.MulEqu, Position, null, "*="));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.Mul, Position, null, Current.ToString()));
                        }

                        break;
                    case '/':
                        if (Peek(1) == '/')
                        {
                            SkipComment();
                            continue;
                        }

                        if (Peek(1) == '=')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.DivEqu, Position, null, "/="));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.Div, Position, null, Current.ToString()));
                        }

                        break;
                    case '#':
                        insertToken = (new(SyntaxType.Idx, Position, null, Current.ToString()));
                        break;
                    case '.':
                        insertToken = (new(SyntaxType.Dot, Position, null, Current.ToString()));
                        break;
                    case ',':
                        insertToken = (new(SyntaxType.Comma, Position, null, Current.ToString()));
                        break;
                    case '(':
                        insertToken = (new(SyntaxType.LParen, Position, null, Current.ToString()));
                        break;
                    case ')':
                        insertToken = (new(SyntaxType.RParen, Position, null, Current.ToString()));
                        break;
                    case '[':
                        insertToken = (new(SyntaxType.LSqBracket, Position, null, Current.ToString()));
                        break;
                    case ']':
                        insertToken = (new(SyntaxType.RSqBracket, Position, null, Current.ToString()));
                        break;
                    case '{':
                        insertToken = (new(SyntaxType.LBraces, Position, null, Current.ToString()));
                        break;
                    case '}':
                        insertToken = (new(SyntaxType.RBraces, Position, null, Current.ToString()));
                        break;
                    case '!':
                        if (Peek(1) == '=')
                        {
                            Position++;
                            insertToken = (new(SyntaxType.BangEquals, Position, null, "!="));
                        }
                        else
                        {
                            insertToken = (new(SyntaxType.Bang, Position, null, Current.ToString()));
                        }

                        break;
                    //case '?':
                    //    if (Peek(1) == '?')
                    //    {
                    //        Position++;
                    //        insertToken = (new(SyntaxType.NullExpr, Position, null, "??"));
                    //    }
                    //    else
                    //    {
                    //        insertToken = (new(SyntaxType.Question, Position, null, Current.ToString()));
                    //    }

                    //    break;
                    case ':':
                        insertToken = (new(SyntaxType.Colon, Position, null, Current.ToString()));
                        break;
                }

                //fix "bad" tokens, they might be keywords or other things
                if (insertToken.Type == SyntaxType.BadToken)
                {
                    if (char.IsDigit(Current))
                    {
                        tokens.Add(ParseNumber());
                    }
                    else if (Current == '"')
                    {
                        SyntaxToken? token = ParseString();
                        if (token is null) 
                        {
                            
                        }
                        else
                            tokens.Add((SyntaxToken)token);
                    }
                    else if (char.IsLetter(Current) || Current == '@' || Current == '_')
                    {
                        tokens.Add(ParseIdentifierOrKeywordOrLogical());
                    }
                    else if (char.IsWhiteSpace(Current)) Position++;
                    else
                    {
                        throw new Exception("Bad token at pos " + insertToken.Position + " with text " + insertToken.Text);
                    }
                }
                else
                {
                    tokens.Add(insertToken);
                    Position++;
                }
            }

            TransformTokens(ref tokens);

            //End Of File, we're done
            tokens.Add(new SyntaxToken(SyntaxType.EOF, Position, null, "<EOF>"));
            return tokens;
        }
        private void SkipComment()
        {
            while (Current != '\0' && Current != '\n')
            {
                Position++;
            }
        }
        private void TransformTokens(ref List<SyntaxToken> tokens)
        {
            TransformToken(SyntaxType.EqualsEquals,"is", SyntaxType.Bang,"not", SyntaxType.BangEquals,"is not",ref tokens);
        }
        private bool TransformToken(SyntaxType tok1,string text1, SyntaxType tok2,string text2, SyntaxType replaceToken,string replaceText, ref List<SyntaxToken> tokens)
        {
            var found = false;
            for (int i = 0; i < tokens.Count(); i++)
            {
                var token = tokens[i];
                if (token.Type == tok2 && token.Text == text2 && found)
                {
                    tokens.Remove(tokens[i]);
                    tokens.Remove(tokens[i-1]);
                    var newToken = token;
                    newToken.Type = replaceToken;
                    newToken.Text = replaceText;
                    newToken.Value = replaceText;
                    tokens.Insert(i-1, newToken);
                    return true;
                }
                else if (token.Type == tok2 && token.Text == text2 && found)
                    found = false;


                if (token.Type == tok1 && token.Text == text1 && !found)
                {
                    found = true;
                }
                else if (token.Type == tok1 && token.Text == text1 && found)
                    found = false;
            }
            return false;
        }
        private SyntaxToken ParseIdentifierOrKeywordOrLogical()
        {
            string str = "";
            int startPos = Position;

            while (Current != '\0' && Current != ' ' && (char.IsLetterOrDigit(Current) || Current == '@' || Current == '_'))
            {
                str += Current;
                Position++;
            }

            var token = new SyntaxToken(SyntaxType.Identifier, startPos, str, str);
            SyntaxFacts.ClassifyIdentifierAsKeyword(ref token);
            SyntaxFacts.ClassifyIdentifierAsLogical(ref token);

            return token;
        }


        private SyntaxToken? ParseString()
        {
            string str = "";
            int startPos = Position;

            Position++;
            while (!(Current == '"' && Peek(-1) != '\\') && Current != '\0')
            {
                if (Current == '\\')
                {
                    Position++;

                    switch (Current)
                    {
                        case '"': str += "\""; break;
                        case 'n': str += "\n"; break;
                        case '\\': str += "\\"; break;
                        case '0': str += "\0"; break;
                        default: throw new Exception("Invalid escape sequence");
                    }

                    Position++;
                }
                else
                {
                    str += Current;
                    Position++;
                }
            }

            Position++;

            if (Current == 'i')
            {
                Position++;
                return new(SyntaxType.Identifier, startPos, str, str);
            }
            else if (Current == '$')
            {
                Position++;
                ParseVarString(new(SyntaxType.String, startPos, str, str));
                return null;
                //return new(SyntaxType.VarString, startPos, str, str);
            }
            else
            {
                return new(SyntaxType.String, startPos, str, str);
            }
        }

        private void ParseVarString(SyntaxToken token) 
        {

            string str = token.Text;


            List<string> arguments = new List<string>();
            string final = string.Empty;

            int length = 1; //the opening "

            string argument = string.Empty;
            bool Inside = false;
            int count = 0;
            for (int i = 0; i < str.Length; i++) 
            {
                char c = str[i];
                if (c is '"' or '\n' or '\\' or '\0')
                {
                    string real = string.Empty;
                    switch (c)
                    {
                        case '"':  real = "\""; break;
                        case '\n': real = "n";  break;
                        case '\\': real = "\\"; break;
                        case '\0': real = "0";  break;
                        //default: throw new Exception("Invalid escape sequence");
                    }
                    if (Inside)
                        argument += real; //dont escape it?
                    else
                        final += "\\"+real; //escape it
                    length++;
                    continue;
                }
                if (Inside)
                {
                    if (c == '}')
                    {
                        final += count;
                        count++;
                        final+= '}';
                        Inside = false;

                        arguments.Add(argument);
                        argument = string.Empty;
                    }
                    else
                    {
                        argument+= c;
                        continue;
                    }
                }
                else
                {
                    final += c;
                }

                if (c == '{')
                {
                    Inside = true;
                }

            }

            length += 2; //the closing " and the $ 

            string cs = string.Empty;
            cs += "FlangFormat";
            cs += "(";
                cs += '"';
                    cs += final;
                cs += '"';
            cs += ",";
                cs += "[";
                    cs += string.Join(",",arguments);
                cs += "]";
            cs += ")";

            Code = Code.Remove(token.Position,token.Text.Length+length); //left and right " plus the $ on the end
            Code = Code.Insert(token.Position, cs);
            Position = token.Position;


            //var tokens = new List<SyntaxToken>();
            //tokens.Add(new SyntaxToken(SyntaxType.Identifier, pos, "FlangFormat", "FlangFormat"));
            //tokens.Add(new SyntaxToken(SyntaxType.LParen, pos, null, "("));
            //tokens.Add(new SyntaxToken(SyntaxType.String, pos, final, final));
            //tokens.Add(new SyntaxToken(SyntaxType.Comma, pos, null, ","));
            //tokens.Add(new SyntaxToken(SyntaxType.LSqBracket, pos, null, "["));
            //foreach (string arg in arguments) 
            //{
            //    tokens.Add(new SyntaxToken(SyntaxType.Identifier, pos, arg, arg));
            //}
            //tokens.Add(new SyntaxToken(SyntaxType.RSqBracket, pos, null, "]"));
            //tokens.Add(new SyntaxToken(SyntaxType.RParen, pos, null, ")"));
            //
            //return tokens;
        }


        private SyntaxToken ParseNumber()
        {
            string numStr = "";
            bool isDecimal = false;
            int startPos = Position;

            while ((char.IsDigit(Current) || Current == '.' || Current == '_') && Current != '\0')
            {
                if (Current == '_') 
                {
                    Position++;
                    continue;
                }
                numStr += Current;

                if (Current == '.')
                {
                    isDecimal = true;
                }

                Position++;
            }

            char typeSpecifier = Char.ToLower(Current); // Capture the character after the number
            if (typeSpecifier is 'i' or 'l' or 'f' or 'd')
            {
                Position++; // Move to the next character after the specifier
                if (isDecimal)
                {
                    if (typeSpecifier is 'i' or 'l')
                    {
                        throw new Exception("Invalid number (tried to parse " + numStr + " as whole number)");
                    }
                    else if (typeSpecifier is 'f')
                    {
                        if (!float.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as float)");
                        return new(SyntaxType.Float, startPos, floatVal, numStr);
                    }
                    else if (typeSpecifier is 'd')
                    {
                        if (!double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double floatVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as double)");
                        return new(SyntaxType.Double, startPos, floatVal, numStr);
                    }
                    else
                    {
                        throw new Exception("Unknow type specifier on number");
                    }
                }
                else
                {
                    if (typeSpecifier is 'i')
                    {
                        if (!int.TryParse(numStr, out int intVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as int)");
                        return new(SyntaxType.Int, startPos, intVal, numStr);
                    }
                    else if (typeSpecifier is 'l')
                    {
                        if (!long.TryParse(numStr, out long longVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as long)");
                        return new(SyntaxType.Long, startPos, longVal, numStr);
                    }
                    else if (typeSpecifier is 'f')
                    {
                        if (!float.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as float)");
                        return new(SyntaxType.Float, startPos, floatVal, numStr);
                    }
                    else if (typeSpecifier is 'd')
                    {
                        if (!double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as double)");
                        return new(SyntaxType.Double, startPos, doubleVal, numStr);
                    }
                    else
                    {
                        throw new Exception("Unknow type specifier on number");
                    }
                }

            }

            if (isDecimal)
            {
                if (!float.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as float)");
                return new(SyntaxType.Float, startPos, floatVal, numStr);
            }
            else
            {
                if (!int.TryParse(numStr, out int intVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as int)");
                return new(SyntaxType.Int, startPos, intVal, numStr);
            }
        }
    }
}
