using FriedLanguage.BuiltinType;
using FriedLanguage.Models.Parsing;
using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;
using static FriedLang.NativeLibraries.Lang;

namespace Friedlang.NativeLibraries.IO
{
    public class AnalizerBase<Type>
    {
        public List<Type> Analizable = new List<Type>();
        public int Position = 0;

        public Type End { get; set; }
        public Type Current => Peek(0);

        //look forward
        public Type Peek(int off = 0)
        {
            if (Position + off >= Analizable.Count || Position + off < 0) return End;
            return Analizable[Position + off];
        }
        public AnalizerBase(Type end)
        {
            this.Analizable = new List<Type>();
            this.End = end;
        }
        public AnalizerBase(List<Type> txt, Type end)
        {
            this.Analizable = txt;
            this.End = end;
        }
    }
    public class JsonParser : AnalizerBase<char>
    {
        public JsonParser() : base('\0'){ }
        public FValue Parse(string input) 
        {
            Analizable = input.ToList();
            Position = 0;
            var output = ParseExpression();
            if (output is FNull)
            {
                throw new Exception("Error parsing json");
            }
            return output;
        }
        private char MatchToken(char token)
        {
            if (Current == token)
            {
                Position++;
                return Current;
            }
            else
            {
                throw new Exception($"expected {token} got {Current} instead");
            }
        }
        private FValue ParseString()
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

            return new FString(str);
        }
        public FValue ParseExpression()
        {
            SkipWhiteSpaces();
            if (Current == '"')
            {
                return ParseString();
            }
            if (char.IsDigit(Current))
            {
                return ParseNumber();
            }
            if (Current is '[')
            { 
                return ParseListExpression();
            }
            if (Current is '{')
            {
                return ParseDictExpression();
            }
            return FNull.Null;
        }
        public void SkipWhiteSpaces() 
        {
            while (char.IsWhiteSpace(Current))
            { 
                Position++;
            }
        }
        public FValue ParseListExpression()
        {
            var lsq = MatchToken('[');
            char rsq;

            List<FValue> list = new();

            if (Current == ']')
            {
                rsq = MatchToken(']');
            }
            else
            {
                var expr = ParseExpression();
                list.Add(expr);

                while (Current == ',')
                {
                    Position++;
                    expr = ParseExpression();
                    list.Add(expr);
                }

                rsq = MatchToken(']');
            }

            return new FList(list);
        }
        public FValue ParseDictExpression()
        {
            var lsq = MatchToken('{');
            char rsq;

            List<(FValue tok, FValue expr)> dict = new();

            if (Current == '}')
            {
                rsq = MatchToken('}');
            }
            else
            {
                //var tok = default(SyntaxToken);

                //            if (Current.Type == SyntaxType.String || Current.Type == SyntaxType.Int)
                //            {
                //	Position++;
                //	tok = Peek(-1);
                //}else
                //	throw MakeException("Unexpected token " + Current.Type + "; expected string or int");



                var tok = ParseString();
                _ = MatchToken(':');
                var expr = ParseExpression();
                dict.Add((tok, expr));

                while (Current == ',')
                {
                    Position++;

                    //tok = MatchToken(SyntaxType.String);
                    tok = ParseExpression();
                    _ = MatchToken(':');
                    expr = ParseExpression();

                    dict.Add((tok, expr));
                }

                rsq = MatchToken('}');
            }

            return new FDynamic(dict);
        }

        private FValue ParseNumber()
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
                        return new FFloat(floatVal);
                    }
                    else if (typeSpecifier is 'd')
                    {
                        if (!double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double floatVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as double)");
                        return new FDouble(floatVal);
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
                        return new FInt(intVal);
                    }
                    else if (typeSpecifier is 'l')
                    {
                        if (!long.TryParse(numStr, out long longVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as long)");
                        return new FLong(longVal);
                    }
                    else if (typeSpecifier is 'f')
                    {
                        if (!float.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as float)");
                        return new FFloat(floatVal);

                    }
                    else if (typeSpecifier is 'd')
                    {
                        if (!double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleVal)) throw new Exception("Invalid number (tried to parse " + numStr + " as double)");
                        return new FDouble(doubleVal);

                    }
                    else
                    {
                        throw new Exception("Unknow type specifier on number");
                    }
                }

            }

            if (isDecimal)
            {
                if (!float.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatVal))
                {
                    if (double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleVal))
                    { 
                        return new FDouble(doubleVal);
                    }
                    throw new Exception("Invalid number (tried to parse " + numStr + " as float)");
                }
                return new FFloat(floatVal);
            }
            else
            {
                if (!int.TryParse(numStr, out int intVal))
                { 
                    if (long.TryParse(numStr, out long longVal)) return new FLong(longVal);
                    throw new Exception("Invalid number (tried to parse " + numStr + " as int)");
                }
                return new FInt(intVal);
            }
        }
    }
}
