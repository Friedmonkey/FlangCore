using FriedLanguage.BuiltinType;
using FriedLanguage.Models;
using FriedLanguage.Models.Parsing;
using FriedLanguage.Models.Parsing.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FriedLanguage
{
    public class Parser
    {
        //public List<(string name, int pos)> Labels = new();
        public List<SyntaxToken> Tokens { get; set; }
        public List<string> DisabledKeywords = new List<string>() { "unmatch" };
        //unmatch keyword is pretty broken, so its disabled
        public string Code { get; }

        public int Position = 0;

        public SyntaxToken Current => Peek(0);
        public static SyntaxToken EmptyToken => new SyntaxToken(SyntaxType.EOF, 0, null, null);

        public SyntaxToken Peek(int off = 0)
        {
            if (Position + off >= Tokens.Count || Position + off < 0) return new(SyntaxType.BadToken, 0, null, "");
            return Tokens[Position + off];
        }

        public SyntaxToken MatchToken(SyntaxType type) 
        {
            if (Current.Type == type)
            {
                Position++;
                return Peek(-1);
            }
            string error = "Unexpected token " + Current.Type + "; expected " + type;
            if (type == SyntaxType.Identifier)
            { 
                var currentCopy = new SyntaxToken(Current);
                currentCopy.Type = SyntaxType.Identifier;
                SyntaxFacts.ClassifyIdentifierAsKeyword(ref currentCopy);
                if (currentCopy.Type != SyntaxType.Identifier)
                {
                    error += " Keep in mind the identifier \""+Current.Text+"\" is a reserved keyword!";
                }
                else 
                {
                    SyntaxFacts.ClassifyIdentifierAsLogical(ref currentCopy);
                    if (currentCopy.Type != SyntaxType.Identifier)
                    { 
                        error += " Keep in mind the identifier \""+Current.Text+"\" is a reserved logical keyword!";
                        
                    }
                }
            }
            throw MakeException(error);
        }
        public bool MatchTokenOptionally(SyntaxType type,out SyntaxToken token)
        {
            if (Current.Type == type)
            {
                Position++;
                token = Peek(-1);
                return true; 
            }
            token = default;
            return false;
        }
        public SyntaxToken MatchTokenWithValue(SyntaxType type,object value)
        {
            if (Current.Type == type && Current.Value == value)
            {
                Position++;
                return Peek(-1);
            }
            throw MakeException("Unexpected token " + Current.Type + "; expected " + type + " with value " + value);
        }
        public SyntaxToken MatchKeyword(string value)
        {
            if (IsKeyword(value))
            {
                Position++;
                return Peek(-1);
            }

            throw MakeException("Unexpected token " + Current.Type + "; expected Keyword with value " + value);
        }

        public Parser(List<SyntaxToken> tokens, string code)
        {
            Tokens = tokens;
            Code = code;
        }

        public SyntaxNode Parse()
        {
            return ParseStatements();
        }


        public List<SyntaxNode> nodes { get; protected set; }
        public SyntaxNode ParseStatements() 
        {
            nodes = new List<SyntaxNode>();
            while (Current.Type != SyntaxType.EOF)
            {
                var pars = ParseStatement();
                if (pars != null)
                    nodes.Add(pars);
            }

            return new BlockNode(Tokens.FirstOrDefault(EmptyToken), Tokens.LastOrDefault(EmptyToken), nodes, false);
        }
        public List<SyntaxNode> GetBraces(out SyntaxToken start, out SyntaxToken end) 
        {
			start = MatchToken(SyntaxType.LBraces);
            end = default;
			List<SyntaxNode> nodes = new();

			while (Current.Type != SyntaxType.RBraces)
			{
				if (Current.Type == SyntaxType.EOF) throw MakeException("Unclosed block at " + Current.Position);

				nodes.Add(ParseStatement());
			}

			end = MatchToken(SyntaxType.RBraces);
            return nodes;
		}

        public SyntaxNode ParseScopedStatements()
        {
            //var startingBrace = MatchToken(SyntaxType.LBraces);
            //List<SyntaxNode> nodes = new();

            //while (Current.Type != SyntaxType.RBraces)
            //{
            //    if (Current.Type == SyntaxType.EOF) throw MakeException("Unclosed block at " + Current.Position);

            //    nodes.Add(ParseStatement());
            //}

            //var endingBrace = MatchToken(SyntaxType.RBraces);
            var nodes = GetBraces(out var start, out var end);

            return new BlockNode(start, end, nodes);
        }

        public void EnableKeyword(string keyword) 
        {
            if (keyword == "csharp")
            { 
                GlobalState.AllowCodeGeneration();
                return;
            }
            //if (keyword == "logic")
            //{
            //    GlobalState.EnableLogicalIdentMessages();
            //    return;
            //}
            var exists = (DisabledKeywords.Contains(keyword));
            if (exists)
                DisabledKeywords.Remove(keyword);
        }
        public void DisableKeyword(string keyword)
        {
            if (keyword == "csharp")
            {
                GlobalState.DisallowCodeGeneration();
                return;
            }
            //if (keyword == "logic")
            //{
            //    GlobalState.DisableLogicalIdentMessages();
            //    return;
            //}
            var exists = (DisabledKeywords.Contains(keyword));
            if (!exists)
                DisabledKeywords.Add(keyword);
        }
        public bool IsKeyword(string keyword)
        {
            var isKeyword = (Current.Type == SyntaxType.Keyword && Current.Text == keyword);
            var isDisabled = (DisabledKeywords.Contains(keyword));

            if (isKeyword && isDisabled)
            {
                throw new Exception($"Keyword \"{keyword}\" is disabled and cant be used.");
            }

            return isKeyword;
        }

#warning main function here
        public SyntaxNode ParseStatement()
        {
            if (IsKeyword("keyword"))
            {
                Position++;
                if (IsKeyword("disable"))
                {
                    Position++;
                    if (Current.Type == SyntaxType.Keyword)
                        DisableKeyword(Current.Text);
                    Position++;
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                }
                else if (IsKeyword("enable"))
                {
                    Position++;
                    if (Current.Type == SyntaxType.Keyword)
                        EnableKeyword(Current.Text);
                    Position++;
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                }
                return null;
            }
            else if (IsKeyword("return"))
            {
                var retTok = Current;

                if (Peek(1).Type == SyntaxType.Semicolon)
                {
                    Position += 2;
                    var ret = new ReturnNode(retTok);
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                    return ret;
                }
                else
                {
                    Position++;
                    var ret = new ReturnNode(retTok, ParseExpression());
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                    return ret;
                }
            }
            else if (IsKeyword("continue"))
            {
                var n = new ContinueNode(Current);

                Position++;
                MatchTokenOptionally(SyntaxType.Semicolon, out _);

                return n;
            }
            else if (IsKeyword("break"))
            {
                var ident = MatchKeyword("break");

                if (MatchTokenOptionally(SyntaxType.Semicolon, out _))
                {
                    return new BreakNode(ident);
                }
                else
                {
                    var numExpr = ParseExpression();
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                    return new BreakNode(ident,numExpr);
                }
            }
            else if (IsKeyword("unmatch"))
            {
                var ident = MatchKeyword("unmatch");

                if (MatchTokenOptionally(SyntaxType.Semicolon, out _))
                {
                    return new UnmatchNode(ident);
                }
                else
                {
                    var numExpr = ParseExpression();
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                    return new UnmatchNode(ident, numExpr);
                }
            }
            else if (IsKeyword("label"))
            {
                MatchKeyword("label");

                var ident = MatchToken(SyntaxType.Identifier);

                MatchToken(SyntaxType.Colon);

                return new LabelNode(ident, ident.Text, nodes.Count());
            }
            else if (IsKeyword("goto"))
            {
                MatchKeyword("goto");

                var ident = MatchToken(SyntaxType.Identifier);

                MatchToken(SyntaxType.Semicolon);

                return new GotoNode(ident, ident.Text);
            }
            else if (IsKeyword("import"))
            {
                Position++;

                bool isExtend = false;
                bool isNative = false;
                bool isMemory = false;

                SyntaxToken ident = default;
                SyntaxNode expr = default;

                if (IsKeyword("native"))
                {
                    isNative = true;
                    Position++;
                }
                else if (IsKeyword("memory"))
                {
                    isMemory = true;
                    Position++;
                }

                if (IsKeyword("extend"))
                {
                    isExtend = true;
                    Position++;
                }

                if (isMemory)
                {
                    expr = ParseExpression();
                }
                else if (isNative)
                {
                    ident = MatchToken(SyntaxType.Identifier);
                }
                else 
                {
                    ident = MatchToken(SyntaxType.String);
                }

                MatchTokenOptionally(SyntaxType.Semicolon, out _);

                if (isNative)
                {
                    return new NativeImportNode(ident, isExtend);
                }
                else if (isMemory)
                {
                    return new MemoryImportNode(Current, expr, isExtend);
                }
                else
                {
                    return new ImportNode(ident, isExtend);
                }
            }


            else if (IsKeyword("export"))
            {
                Position++;
                if (IsKeyword("extend"))
                {
                    Position++;
                    var ident = MatchToken(SyntaxType.Identifier);
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                    return new ExportNode(ident, true);
                }
                else
                {
                    var ident = MatchToken(SyntaxType.Identifier);
                    MatchTokenOptionally(SyntaxType.Semicolon, out _);
                    return new ExportNode(ident);
                }
            }
            else if (IsKeyword("class"))
            {
                var c = ParseClassDefinition();

                MatchTokenOptionally(SyntaxType.Semicolon, out _);
                return c;
            }
            else
            {
                var exprNode = ParseExpression();
                MatchTokenOptionally(SyntaxType.Semicolon, out _);

                return exprNode;
            }
        }
        private SyntaxNode ParseClassDefinition()
        {
            MatchKeyword("class");
            var strict = false;
            var extend = false;

            if (IsKeyword("strict"))
            {
                Position++;
                strict = true;
            }
            if (IsKeyword("extend"))
            {
                Position++;
                extend = true;
            }

            var className = MatchToken(SyntaxType.Identifier);

            MatchToken(SyntaxType.LBraces);
            var body = ParseClassBody(className);
            MatchToken(SyntaxType.RBraces);

            return new ClassDefinitionNode(className, body, strict,extend);
        }

        private List<SyntaxNode> ParseClassBody(SyntaxToken className)
        {
            List<SyntaxNode> nodes = new();
            //(IsKeyword("func")|| IsKeyword("prop")|| IsKeyword("op"))
            while
                (
                (
                    (
                        Current.Text == "int" ||
                        Current.Text == "string" ||
                        Current.Text == "bool" ||
                        Current.Text == "float" ||
                        Current.Text == "double" ||
                        Current.Text == "long" ||
                        Current.Text == "object" ||
                        Current.Text == "list" ||
                        Current.Text == "dictionary" ||
                        Current.Text == "void"

                    ) && Peek(1).Type == SyntaxType.Identifier
                ) || Current.Text == className.Text //constructor
                || IsKeyword("overload")
                || IsKeyword("static")
                || IsKeyword("const")
                )
            {
                bool hasHadAll = false;
                if (Current.Text == "static" || Current.Text == "const")
                {
                    Position++;
                }
            gohere:
                //varible
                if (((Peek(2).Type is SyntaxType.Semicolon or SyntaxType.Equals) && Current.Text != "void") || hasHadAll)
                {
                    bool fixedType = (Current.Text != "object");
                    bool isStatic = false;
                    bool isConst = false;
                    bool isNullable = false;
                    //must be a var
                    //"string name" will bypass tho because it doest have semi colon
                    //const STRING
                    if (Peek(-1).Text == "const")
                    {
                        isConst = true;
                        //static const STRING
                        if (Peek(-2).Text == "static")
                        {
                            isStatic = true;
                        }
                    }
                    //static STRING
                    else if (Peek(-1).Text == "static")
                    {
                        isStatic = true;
                    }
                    var varType = Current;
                    Position++;

                    if (Current.Type == SyntaxType.Question)
                    {
                        MatchToken(SyntaxType.Question);
                        isNullable = true;
                    }

                    var ident = MatchToken(SyntaxType.Identifier);

                    if (Current.Type == SyntaxType.Equals)
                    {
                        Position++;
                        var expr = ParseExpression();

                        nodes.Add(new ClassPropDefinitionNode(ident,varType, expr, isStatic, fixedType, isConst, isNullable));
                    }
                    else
                    {
                        nodes.Add(new ClassPropDefinitionNode(ident,varType, null, isStatic, fixedType, isConst, isNullable));
                    }


                }
                else if (IsKeyword("overload")) //overload
                {
                    bool isStatic = false, isConst = false;
                    if (Peek(-1).Text == "const")
                    {
                        isConst = true;
                        //static const STRING
                        if (Peek(-2).Text == "static")
                        {
                            isStatic = true;
                        }
                    }
                    //static STRING
                    else if (Peek(-1).Text == "static")
                    {
                        isStatic = true;
                    }
                    if (isConst)
                    {
                        throw new Exception("a overload cannot be marked with const");
                    }
                    if (!isStatic)
                    {
                        throw new Exception("a overload must be marked with static");
                    }

                    //MatchKeyword("overload");
                    Position++;

                    var opTok = Current;
                    Position++;


                    if (opTok.Type is not (SyntaxType.Plus or SyntaxType.Minus or SyntaxType.Mul or SyntaxType.Div or SyntaxType.EqualsEquals
                        or SyntaxType.LessThan or SyntaxType.LessThanEqu or SyntaxType.GreaterThan or SyntaxType.GreaterThanEqu))
                    {
                        throw MakeException("Can not find or override operator of token type " + opTok.Type + " at position " + opTok.Position);
                    }

                    //var args = ParseFunctionArgs();
                    //var body = ParseScopedStatements();

                    //nodes.Add(new ClassFunctionDefinitionNode(new(SyntaxType.Identifier, opTok.Position, "$$op" + opTok.Text, "$$op" + opTok.Text), args, body, false));

                    var name = Peek(-2);
                    name.Text = "$$op"+opTok.Text;
                    (var args, var types) = ParseFunctionArgs();
                    var body = ParseScopedStatements();
                    SyntaxToken returnType = new SyntaxToken();
                    returnType.Text = "object";
                    returnType.Type = SyntaxType.Identifier;


                    //var name = MatchToken(SyntaxType.Identifier);
                    //(var args, var types) = ParseFunctionArgs();
                    //var body = ParseScopedStatements();

                    nodes.Add(new ClassFunctionDefinitionNode(name, args, types, returnType, body, isStatic));
                }
                else if (Current.Text == className.Text) //constructor
                {
                    bool isStatic = false,isConst = false;
                    if (Peek(-1).Text == "const")
                    {
                        isConst = true;
                        //static const STRING
                        if (Peek(-2).Text == "static")
                        {
                            isStatic = true;
                        }
                    }
                    //static STRING
                    else if (Peek(-1).Text == "static")
                    {
                        isStatic = true;
                    }
                    if (isStatic || isConst)
                    {
                        throw new Exception("Constructor cannot be marked with static or const");
                    }
                    var name = MatchToken(SyntaxType.Identifier);
                    name.Text = "ctor";
                    (var args, var types) = ParseFunctionArgs();
                    var body = ParseScopedStatements();
                    SyntaxToken returnType = new SyntaxToken();
                    returnType.Text = "void";
                    returnType.Type = SyntaxType.Identifier;
                    nodes.Add(new ClassFunctionDefinitionNode(name, args,types,returnType, body, false)); //never static
                }
                else if (Peek(1).Type == SyntaxType.Identifier && Peek(2).Type == SyntaxType.LParen) //function
                {
                    bool isStatic = false, isConst = false;
                    if (Peek(-1).Text == "const")
                    {
                        isConst = true;
                        //static const STRING
                        if (Peek(-2).Text == "static")
                        {
                            isStatic = true;
                        }
                    }
                    //static STRING
                    else if (Peek(-1).Text == "static")
                    {
                        isStatic = true;
                    }
                    if (isConst)
                    {
                        throw new Exception("a Function cannot be marked with const");
                    }

                    var returnType = Current;
                    Position++;

                    var name = MatchToken(SyntaxType.Identifier);
                    (var args, var types) = ParseFunctionArgs();
                    var body = ParseScopedStatements();

                    nodes.Add(new ClassFunctionDefinitionNode(name, args, types, returnType, body, isStatic)); //never static
                }
                else
                {
                    hasHadAll = true;
                    goto gohere;
                }
     //           if (IsKeyword("func"))
     //           {
     //               Position++;
     //               var isStatic = false;

     //               if (IsKeyword("static"))
     //               {
     //                   Position++;
     //                   isStatic = true;
     //               }

					//var name = MatchToken(SyntaxType.Identifier);
     //               var args = ParseFunctionArgs();
     //               var body = ParseScopedStatements();

     //               nodes.Add(new ClassFunctionDefinitionNode(name, args, body, isStatic));
     //           }
     //           else if (IsKeyword("prop"))
     //           {
     //               Position++;
     //               var isStatic = false;

     //               if (IsKeyword("static"))
     //               {
     //                   Position++;
     //                   isStatic = true;
     //               }

     //               var name = MatchToken(SyntaxType.Identifier);
     //               MatchToken(SyntaxType.Equals);
     //               var expr = ParseExpression();

     //               nodes.Add(new ClassPropDefinitionNode(name, expr, isStatic));
     //           }
     //           else if (IsKeyword("op"))
     //           {
     //               Position++;

     //               var opTok = Current;
     //               Position++;

     //               if (opTok.Type is not (SyntaxType.Plus or SyntaxType.Minus or SyntaxType.Mul or SyntaxType.Div or SyntaxType.EqualsEquals
     //                   or SyntaxType.LessThan or SyntaxType.LessThanEqu or SyntaxType.GreaterThan or SyntaxType.GreaterThanEqu))
     //               {
     //                   throw MakeException("Can not find or override operator of token type " + opTok.Type + " at position " + opTok.Position);
     //               }

     //               var args = ParseFunctionArgs();
     //               var body = ParseScopedStatements();

     //               nodes.Add(new ClassFunctionDefinitionNode(new(SyntaxType.Identifier, opTok.Position, "$$op" + opTok.Text, "$$op" + opTok.Text), args, body, false));
     //           }

                while (Current.Type == SyntaxType.Semicolon)
                {
                    Position++;
                }
            }

            return nodes;
        }

        //     public SyntaxNode ParseExpression()
        //     {
        //         if (IsKeyword("var") ||
        //             IsKeyword("const"))
        //         {
        //             return ParseVariableDefinition();
        //         }


        //         if (Current.Type == SyntaxType.Identifier)
        //         {
        //    var ident = MatchToken(SyntaxType.Identifier);

        //	if (Current.Type == SyntaxType.Equals)
        //	{
        //		MatchToken(SyntaxType.Equals);
        //		var expr = ParseExpression();
        //		return new AssignVariableNode(ident, expr);
        //	}
        //             if (Current.Type == SyntaxType.LSqBracket)
        //             {
        //                 MatchToken(SyntaxType.LSqBracket);
        //                 var idxExpr = ParseExpression();
        //                 MatchToken(SyntaxType.RSqBracket);

        //                 if (Current.Type == SyntaxType.Equals)
        //                 {
        //			MatchToken(SyntaxType.Equals);
        //			var expr = ParseExpression();
        //			return new AssignVariableNode(ident, idxExpr, expr);

        //		}else
        //			return new IndexerNode(ident, idxExpr);

        //	}
        //             if (Current.Type is SyntaxType.Equals or SyntaxType.PlusEqu or SyntaxType.MinusEqu or SyntaxType.ModEqu or SyntaxType.DivEqu or SyntaxType.MulEqu)
        //             {

        //                 var assignTok = Current;
        //                 assignTok.Type = MapEqualsTokens(assignTok.Type);
        //                 Position++;

        //                 var expr = ParseExpression();
        //                 return new AssignVariableNode(ident, new BinaryExpressionNode(new IdentifierNode(ident), assignTok, expr));
        //             }
        //             if (Current.Type is SyntaxType.PlusPlus or SyntaxType.MinusMinus)
        //             {

        //                 var assignTok = Current;
        //                 assignTok.Type = MapDoubleTokens(assignTok.Type);
        //                 Position++;

        //                 return new AssignVariableNode(ident, new BinaryExpressionNode(new IdentifierNode(ident), assignTok, new IntLiteralNode(new SyntaxToken(SyntaxType.Int, assignTok.Position, 1, "1"))));
        //             }
        //}
        //         return BinaryOperation(() => ParseCompExpression(), new List<SyntaxType>() { SyntaxType.AndAnd, SyntaxType.OrOr });
        //     }
#warning parseExpresstion parse expr
        public SyntaxNode ParseExpression()
		{
            if (IsKeyword("const"))
            {
                Position++;
            }
			if (IsKeyword("var"))
			{
				return ParseVariableDefinition();
			}
            if (
                (
                    Current.Text == "int" ||
                    Current.Text == "string" ||
                    Current.Text == "bool" ||
                    Current.Text == "float" ||
                    Current.Text == "double" ||
                    Current.Text == "long" ||
                    Current.Text == "object" ||
                    Current.Text == "list" ||
                    Current.Text == "dictionary" ||
                    Current.Text == "void"
                ) && Peek(1).Type == SyntaxType.Identifier
                )
            {
                if (Peek(2).Type == SyntaxType.LParen)
                {
                    return ParseExplicitFunctionDefinition();
                }
                else
                { 
                    return ParseExplicitVariableDefinition();
                }
            }
            if (Current.Type == SyntaxType.Identifier && Peek(1).Type == SyntaxType.Equals)
			{
				var ident = MatchToken(SyntaxType.Identifier);
				MatchToken(SyntaxType.Equals);
				var expr = ParseExpression();
				return new AssignVariableNode(ident, expr);
			}
			if (Current.Type == SyntaxType.Identifier && Peek(1).Type == SyntaxType.LSqBracket)
			{
                int startpos = Position;
				var ident = MatchToken(SyntaxType.Identifier);

				MatchToken(SyntaxType.LSqBracket);
				var idxExpr = ParseExpression();
				MatchToken(SyntaxType.RSqBracket);

                if (Current.Type == SyntaxType.Equals)
                {
#warning Set Index here
                    MatchToken(SyntaxType.Equals);
                    var expr = ParseExpression();
                    return new AssignVariableNode(ident, idxExpr, expr,null);
                }
                Position = startpos;
			}

			if (Current.Type == SyntaxType.Identifier && (Peek(1).Type is SyntaxType.Equals or SyntaxType.PlusEqu or SyntaxType.MinusEqu or SyntaxType.ModEqu or SyntaxType.DivEqu or SyntaxType.MulEqu))
			{
				var ident = MatchToken(SyntaxType.Identifier);

				var assignTok = Current;
				assignTok.Type = MapEqualsTokens(assignTok.Type);
				Position++;

				var expr = ParseExpression();


                //if (Current.Type == SyntaxType.NullExpr)
                //{
                //    MatchToken(SyntaxType.NullExpr);
                //    var nullExpr = ParseExpression();
                //    return new AssignVariableNode(ident, new BinaryExpressionNode(new IdentifierNode(ident), assignTok, expr), null, nullExpr);
                //}

                return new AssignVariableNode(ident, new BinaryExpressionNode(new IdentifierNode(ident), assignTok, expr));
			}
			if (Current.Type == SyntaxType.Identifier && (Peek(1).Type is SyntaxType.PlusPlus or SyntaxType.MinusMinus))
			{
				var ident = MatchToken(SyntaxType.Identifier);

				var assignTok = Current;
				assignTok.Type = MapDoubleTokens(assignTok.Type);
				Position++;

				return new AssignVariableNode(ident, new BinaryExpressionNode(new IdentifierNode(ident), assignTok, new IntLiteralNode(new SyntaxToken(SyntaxType.Int, assignTok.Position, 1, "1"))));
			}
            return BinaryOperation(() => ParseCompExpression(), new List<SyntaxType>() { SyntaxType.AndAnd, SyntaxType.OrOr });
		}
		public SyntaxNode ParseVariableDefinition()
        {
            bool isConst = false;
            bool isNullable = false;

            if (Peek(-1).Type == SyntaxType.Keyword && Peek(-1).Text == "const")
            { 
                isConst = true;
            }


            //if (IsKeyword("const"))
            //{
            //    isConst = true;
            //    Position++;
            //}

            bool fixedType = true;
            MatchKeyword("var");


            if (Current.Type == SyntaxType.Question)
            {
                MatchToken(SyntaxType.Question);
                isNullable = true;
            }

            if (Current.Type == SyntaxType.Mod)
            {
                fixedType = false;
                Position++;
            }

            var ident = MatchToken(SyntaxType.Identifier);

            if (Current.Type == SyntaxType.Equals)
            {
                Position++;
                var expr = ParseExpression();

                return new InitVariableNode(ident, expr, fixedType, isConst,isNullable);
            }
            else
            {
                return new InitVariableNode(ident, fixedType, isConst,isNullable);
            }
        }
        public SyntaxNode ParseExplicitVariableDefinition()
        {
            bool isConst = false;
            bool isNullable = false;

            if (Peek(-1).Type == SyntaxType.Keyword && Peek(-1).Text == "const")
            {
                isConst = true;
            }

            bool fixedType = true;
            switch (Current.Text)
            {
                case "int":
                    Position++;
                    break;
                case "string":
                    Position++;
                    break;
                case "bool":
                    Position++;
                    break;
                case "float":
                    Position++;
                    break;
                case "double":
                    Position++;
                    break;
                case "long":
                    Position++;
                    break;
                case "list":
                    Position++;
                    break;
                case "dictionary":
                    Position++;
                    break;
                case "object":
                    fixedType = false;
                    Position++;
                    break;
                default:
                    throw new Exception(Current.Text+" is not a valid type for a varible");
            }
            var varType = Peek(-1);


            if (Current.Type == SyntaxType.Question)
            {
                MatchToken(SyntaxType.Question);
                isNullable = true;
            }

            //if (Current.Type == SyntaxType.Mod)
            //{
            //    fixedType = false;
            //    Position++;
            //}

            var ident = MatchToken(SyntaxType.Identifier);

            if (Current.Type == SyntaxType.Equals)
            {
                Position++;
                var expr = ParseExpression();

                return new InitExplicitVariableNode(ident, varType, expr, fixedType, isConst, isNullable);
            }
            else
            {
                return new InitExplicitVariableNode(ident, varType, fixedType, isConst, isNullable);
            }
        }
        public SyntaxNode ParseExplicitFunctionDefinition()
        {
            bool isStatic = false, isConst = false;
            if (Peek(-1).Text == "const")
            {
                isConst = true;
                //static const STRING
                if (Peek(-2).Text == "static")
                {
                    isStatic = true;
                }
            }
            //static STRING
            else if (Peek(-1).Text == "static")
            {
                isStatic = true;
            }
            if (isConst)
            {
                throw new Exception("a Function cannot be marked with const");
            }

            var returnType = Current;
            Position++;

            SyntaxToken? name = null; 

            if (Current.Type == SyntaxType.Identifier) //we dont need an identifer
                name = MatchToken(SyntaxType.Identifier);


            (var args,var types) = ParseFunctionArgs();

            SyntaxNode block;


            if (Current.Type == SyntaxType.Arrow)
            {
                var arrow = MatchToken(SyntaxType.Arrow);
                block = ParseScopedOrStatement();
                block = new ReturnNode(arrow, block);
            }
            else
            {
                block = ParseScopedStatements();
            }

            return new FunctionDefinitionNode(name, args, types, returnType, block, false); //never static



            //public SyntaxNode ParseFunctionExpression()
            //{
            //    MatchKeyword("func");

            //    bool @override = false;

            //    SyntaxToken? nameToken = null;

            //    if (IsKeyword("override"))
            //    {
            //        Position++;
            //        @override = true;
            //    }

            //    if (Current.Type == SyntaxType.Identifier)
            //        nameToken = MatchToken(SyntaxType.Identifier);

            //    (var args, var types) = ParseFunctionArgs();

            //    SyntaxNode block;

            //    if (Current.Type == SyntaxType.LBraces)
            //    {
            //        block = ParseScopedStatements();
            //    }
            //    else
            //    {
            //        var arrow = MatchToken(SyntaxType.Arrow);
            //        block = ParseScopedOrStatement();
            //        block = new ReturnNode(arrow, block);
            //    }

            //    return new FunctionDefinitionNode(nameToken, args, types, returnType, block,@override);
            //}
        }
        //parse comparision expression  
        public SyntaxNode ParseCompExpression()
        {
            if (Current.Type == SyntaxType.Bang)
            {
                Position++;
                return new UnaryExpressionNode(Peek(-1), ParseCompExpression());
            }
            else
            {
                return BinaryOperation(() => { return ParseArithmeticExpression(); },
                    new List<SyntaxType>() {
                        SyntaxType.EqualsEquals, SyntaxType.BangEquals, SyntaxType.LessThan, SyntaxType.LessThanEqu, SyntaxType.GreaterThan, SyntaxType.GreaterThanEqu
                    });
            }
        }

        public SyntaxNode ParseArithmeticExpression()
        {
            return BinaryOperation(() => { return ParseTermExpression(); }, new() { SyntaxType.Plus, SyntaxType.Minus });
        }

        public SyntaxNode ParseTermExpression()
        {
            return BinaryOperation(() => { return ParseFactorExpression(); }, new() { SyntaxType.Mul, SyntaxType.Div, SyntaxType.Mod, SyntaxType.Idx });
        }

        public SyntaxNode ParseFactorExpression()
        {
            if (Current.Type is SyntaxType.Plus or SyntaxType.Minus or SyntaxType.Bang)
            {
                var tok = Current;
                Position++;
                var factor = ParseFactorExpression();
                return new UnaryExpressionNode(tok, factor);
            }

            return ParsePowerExpression();
        }

        public SyntaxNode ParsePowerExpression()
        {
            return BinaryOperation(() => { return ParseDotExpression(); }, new() { SyntaxType.Pow }, () => { return ParseFactorExpression(); });
        }

        public SyntaxNode ParseDotExpression()
        {
            var callNode = ParseCallExpression();
            DotNode accessStack = new(callNode);

            if (Current.Type is SyntaxType.Dot)
            {
                while (Current.Type is SyntaxType.Dot)
                {
                    Position++;

                    if (Current.Type is SyntaxType.Identifier)
                    {
                        if (Peek(1).Type is SyntaxType.Equals)
                        {
                            var ident = MatchToken(SyntaxType.Identifier);
                            MatchToken(SyntaxType.Equals);
                            var expr = ParseExpression();

                            accessStack.NextNodes.Add(new AssignVariableNode(ident, expr));
                        }
                        else if (Peek(1).Type is SyntaxType.Equals or SyntaxType.PlusEqu or SyntaxType.MinusEqu or SyntaxType.ModEqu or SyntaxType.DivEqu or SyntaxType.MulEqu)
                        {
                            var ident = MatchToken(SyntaxType.Identifier);

                            var assignTok = Current;
                            assignTok.Type = MapEqualsTokens(assignTok.Type);
                            Position++;

                            var expr = ParseExpression();
                            // TODO: Check if we can do this in a better way
                            var binOpDot = accessStack.Clone();
                            binOpDot.NextNodes.Add(new IdentifierNode(ident));

                            accessStack.NextNodes.Add(new AssignVariableNode(ident, new BinaryExpressionNode(binOpDot, assignTok, expr)));
                        }
                        else if (Peek(1).Type is SyntaxType.PlusPlus or SyntaxType.MinusMinus)
                        {
                            var ident = MatchToken(SyntaxType.Identifier);

                            var assignTok = Current;
                            assignTok.Type = MapDoubleTokens(assignTok.Type);
                            Position++;

                            var binOpDot = accessStack.Clone();
                            binOpDot.NextNodes.Add(new IdentifierNode(ident));
                            accessStack.NextNodes.Add(new AssignVariableNode(ident, new BinaryExpressionNode(binOpDot, assignTok, new IntLiteralNode(new SyntaxToken(SyntaxType.Int, assignTok.Position, 1, "1")))));
                        }
                        else
                        {
                            var n = ParseCallExpression();
                            accessStack.NextNodes.Add(n);
                        }
                    }
                }
            }
            else return callNode;

            return accessStack;
        }

        public SyntaxNode ParseCallExpression()
        {
            var atomNode = ParseCastExpression();

            if (Current.Type is SyntaxType.LParen)
            {
                Position++;

                List<SyntaxNode> argumentNodes = new();

                if (Current.Type is SyntaxType.RParen)
                {
                    Position++;
                }
                else
                {
                    argumentNodes.Add(ParseExpression());

                    while (Current.Type is SyntaxType.Comma)
                    {
                        Position++;

                        argumentNodes.Add(ParseExpression());
                    }

                    MatchToken(SyntaxType.RParen);
                }

				if (Current.Type == SyntaxType.LSqBracket)
				{
					MatchToken(SyntaxType.LSqBracket);
					var idxExpr = ParseExpression();
					MatchToken(SyntaxType.RSqBracket);

				    return new CallNode(atomNode, argumentNodes,idxExpr);
				}

				return new CallNode(atomNode, argumentNodes);
            }

            return atomNode;
        }

        public SyntaxNode ParseCastExpression()
        {
            if (Current.Type is SyntaxType.LessThan)
            {
                MatchToken(SyntaxType.LessThan);
                var ident = MatchToken(SyntaxType.Identifier);

                MatchToken(SyntaxType.GreaterThan);

                var node = ParseCastExpression();
                return new CastNode(ident, node);
            }
            else
            {
                return ParseAtomExpression();
            }
        }

        public SyntaxNode ParseAtomExpression()
        {
            if (Current.Type is SyntaxType.Int)
            {
                var ident = MatchToken(SyntaxType.Int);
                return new IntLiteralNode(ident);
            }
            else if (Current.Type is SyntaxType.Long)
            {
                var ident = MatchToken(SyntaxType.Long);
                return new LongLiteralNode(ident);
            }
            else if (Current.Type is SyntaxType.Float)
            {
                var ident = MatchToken(SyntaxType.Float);
                return new FloatLiteralNode(ident);
            }
            else if (Current.Type is SyntaxType.Double)
            {
                var ident = MatchToken(SyntaxType.Double);
                return new DoubleLiteralNode(ident);
            }
            else if (Current.Type is SyntaxType.String)
            {
                var ident = MatchToken(SyntaxType.String);

                if (Current.Type == SyntaxType.LSqBracket)
                {
                    MatchToken(SyntaxType.LSqBracket);
                    var idxExpr = ParseExpression();
                    MatchToken(SyntaxType.RSqBracket);

                    return new StringLiteralNode(ident, idxExpr);
                }

                return new StringLiteralNode(ident);
            }
            else if
            (
                (
                    Current.Text == "int" ||
                    Current.Text == "string" ||
                    Current.Text == "bool" ||
                    Current.Text == "float" ||
                    Current.Text == "double" ||
                    Current.Text == "long" ||
                    Current.Text == "object" ||
                    Current.Text == "list" ||
                    Current.Text == "dictionary" ||
                    Current.Text == "void"
                )
            )
            {
                return ParseExplicitFunctionDefinition();
            }
            else if (Current.Type is SyntaxType.Identifier)
            {
                var ident = MatchToken(SyntaxType.Identifier);


                if (Current.Type == SyntaxType.LSqBracket)
                {
                    MatchToken(SyntaxType.LSqBracket);
                    var idxExpr = ParseExpression();
                    MatchToken(SyntaxType.RSqBracket);

                    return new IdentifierNode(ident, idxExpr);
                }

                return new IdentifierNode(ident);
            }
            else if (Current.Type is SyntaxType.LParen)
            {
                Position++;
                var expr = ParseExpression();

                MatchToken(SyntaxType.RParen);

                return expr;
            }
            else if (Current.Type is SyntaxType.LSqBracket)
            {
                return ParseListExpression();
            }
            else if (Current.Type is SyntaxType.LBraces)
            {
                return ParseDictExpression();
            }
            else if (IsKeyword("if"))
            {
                return ParseIfExpression();
            }
            else if (IsKeyword("switch"))
            {
                return ParseSwitchExpression();
            }
            else if (IsKeyword("try"))
            {
                return ParseTryCatchExpression();
            }
            else if (IsKeyword("for"))
            {
                return ParseForExpression();
            }
            else if (IsKeyword("foreach"))
            {
                return ParseForEachExpression();
            }
            else if (IsKeyword("repeat"))
            {
                return ParseRepeatExpression();
            }
            else if (IsKeyword("while"))
            {
                return ParseWhileExpression();
            }
            else if (IsKeyword("throw"))
            {
                return ParseThrowExpression();
            }
            else if (IsKeyword("new"))
            {
                return ParseInstantiateExpression();
            }
            else if (Current.Type == SyntaxType.CsLit)
            {
                Position++;
                return new CsLiteral(Peek(-1));
            }
            else
            {
                throw MakeException($"Unexpected token {Current.Type} at pos {Current.Position} with text {Current.Text} in atom expression!");
            }
        }

        public SyntaxNode ParseListExpression()
        {
            var lsq = MatchToken(SyntaxType.LSqBracket);
            SyntaxToken rsq;

            List<SyntaxNode> list = new();

            if (Current.Type == SyntaxType.RSqBracket)
            {
                rsq = MatchToken(SyntaxType.RSqBracket);
            }
            else
            {
                var expr = ParseExpression();
                list.Add(expr);

                while (Current.Type == SyntaxType.Comma)
                {
                    Position++;
                    expr = ParseExpression();
                    list.Add(expr);
                }

                rsq = MatchToken(SyntaxType.RSqBracket);
            }

            return new ListNode(list, lsq, rsq);
        }

        public SyntaxNode ParseDictExpression()
        {
            var lsq = MatchToken(SyntaxType.LBraces);
            SyntaxToken rsq;

            List<(SyntaxToken tok, SyntaxNode expr)> dict = new();

            if (Current.Type == SyntaxType.RBraces)
            {
                rsq = MatchToken(SyntaxType.RBraces);
            }
            else
            {
                var tok = default(SyntaxToken);

                if (Current.Type == SyntaxType.String || Current.Type == SyntaxType.Int)
                {
					Position++;
					tok = Peek(-1);
				}else
					throw MakeException("Unexpected token " + Current.Type + "; expected string or int");



				_ = MatchToken(SyntaxType.Colon);
                var expr = ParseExpression();
                dict.Add((tok, expr));

                while (Current.Type == SyntaxType.Comma)
                {
                    Position++;

                    tok = MatchToken(SyntaxType.String);
                    _ = MatchToken(SyntaxType.Colon);
                    expr = ParseExpression();

                    dict.Add((tok, expr));
                }

                rsq = MatchToken(SyntaxType.RBraces);
            }

            return new DictNode(dict, lsq, rsq);
        }

        public SyntaxNode ParseIfExpression()
        {
            var kw = MatchKeyword("if");

            IfNode node = new(kw);

            MatchTokenOptionally(SyntaxType.LParen,out _);
            var initialCond = ParseExpression();
            MatchTokenOptionally(SyntaxType.RParen,out _);
            var lastBlock = ParseScopedOrStatement();

            node.AddCase(initialCond, lastBlock);

            while (Current.Type == SyntaxType.Keyword && (string)Current.Value == "elseif")
            {
                Position++;

                MatchTokenOptionally(SyntaxType.LParen,out _);
                var cond = ParseExpression();
                MatchTokenOptionally(SyntaxType.RParen,out _);
                lastBlock = ParseScopedOrStatement();

                node.AddCase(cond, lastBlock);
            }

            if (IsKeyword("else"))
            {
                Position++;
                lastBlock = ParseScopedOrStatement();

                //always true
                node.AddCase(new IntLiteralNode(new SyntaxToken(SyntaxType.Int, 0, 1, "1")), lastBlock);
            }

            node.EndPosition = lastBlock.EndPosition;

            return node;
        }
		public SyntaxNode ParseSwitchExpression()
		{
			var kw = MatchKeyword("switch");

			SwitchNode @switch = new(kw);

			MatchToken(SyntaxType.LParen);
			var identifier = MatchToken(SyntaxType.Identifier);
			MatchToken(SyntaxType.RParen);


			MatchToken(SyntaxType.LBraces);
			List<SyntaxNode> nodes = new();

            int i = 0;
            int j = 0;
			while (Current.Type != SyntaxType.RBraces)
			{
                i++;
				if (Current.Type == SyntaxType.EOF) throw MakeException("Unclosed block at " + Current.Position);

				if (IsKeyword("case"))
                {
					var caseToken = MatchKeyword("case");

					if (IsKeyword("default"))
                    {
						//default case
						Position++;
						MatchToken(SyntaxType.Colon);
                        nodes.Add(new LabelNode(caseToken,"case_default",i));
					}
					else
					{
						var expr = ParseExpression();

						MatchToken(SyntaxType.Colon);
						nodes.Add(new CaseNode(caseToken, "case_"+j, i, expr));
                        j++;
					}
                    @switch.AddCase(nodes.Last());
				}
                else
				    nodes.Add(ParseStatement());
			}

			MatchToken(SyntaxType.RBraces);

            @switch.Code = nodes;
            @switch.Check = identifier;

            return @switch;
            /*
			MatchToken(SyntaxType.RBraces);









			var lastBlock = ParseScopedOrStatement();
			//node.AddCase(initialCond, lastBlock);

			while (Current.Type == SyntaxType.Keyword && (string)Current.Value == "elseif")
			{
				Position++;

				MatchToken(SyntaxType.LParen);
				var cond = ParseExpression();
                MatchToken(SyntaxType.RParen);


                lastBlock = ParseScopedOrStatement();

				node.AddCase(cond, lastBlock);
			}

			if (IsKeyword("else")
			{
				Position++;
				lastBlock = ParseScopedOrStatement();

				node.AddCase(new IntLiteralNode(new SyntaxToken(SyntaxType.Int, 0, 1, "1")), lastBlock);
			}

			node.EndPosition = lastBlock.EndPosition;

			return node;
            */
		}
		public SyntaxNode ParseTryCatchExpression()
        {
            var kw = MatchKeyword("try");

            TryCatchNode node = new(kw);

            var tryblock = ParseScopedOrStatement();

            node.SetTryBlock(tryblock);


            if (IsKeyword("catch"))
            {
                Position++;
                if (MatchTokenOptionally(SyntaxType.LParen, out _))
                {
                    var ident = MatchToken(SyntaxType.Identifier);
                    MatchToken(SyntaxType.RParen);
                    var catchblock = ParseScopedOrStatement();
                    node.SetCatchBlock(catchblock, true, ident);
                }
                else
                { 
                    var catchblock = ParseScopedOrStatement();
                    node.SetCatchBlock(catchblock,false,default);
                }
            }

            node.EndPosition = tryblock.EndPosition;

            return node;
        }

        public SyntaxNode ParseScopedOrStatement()
        {
            if (Current.Type == SyntaxType.LBraces)
                return ParseScopedStatements();
            else
            {
                var expr = ParseStatement();
                MatchTokenOptionally(SyntaxType.Semicolon, out _);
                return expr;
            }
        }

        public SyntaxNode ParseForExpression()
        {
            MatchKeyword("for");

            MatchToken(SyntaxType.LParen);
            var initialExpressionNode = ParseExpression();
            MatchTokenOptionally(SyntaxType.Semicolon, out _);
            var condNode = ParseExpression();
            MatchTokenOptionally(SyntaxType.Semicolon, out _);
            var stepNode = ParseExpression();
            MatchToken(SyntaxType.RParen);
            var block = ParseScopedOrStatement();

            return new ForNode(initialExpressionNode, condNode, stepNode, block);
        }

		public SyntaxNode ParseForEachExpression()
		{
			MatchKeyword("foreach");

			MatchToken(SyntaxType.LParen);
			MatchKeyword("var");
			var identInter = MatchToken(SyntaxType.Identifier);
			MatchKeyword("in");
			var expr = ParseExpression();
			MatchToken(SyntaxType.RParen);

			var block = ParseScopedOrStatement();

			return new ForeachNode(identInter, expr, block);
		}

		public SyntaxNode ParseRepeatExpression()
        {
            MatchKeyword("repeat");
            var keepScope = MatchTokenOptionally(SyntaxType.Bang, out _);

            MatchToken(SyntaxType.LParen);
            var timesExpr = ParseExpression();
            MatchKeyword("times");
            MatchToken(SyntaxType.RParen);

            var block = ParseScopedOrStatement();

            return new RepeatNode(timesExpr, block, keepScope);
        }

        public SyntaxNode ParseWhileExpression()
        {
            MatchKeyword("while");

            MatchToken(SyntaxType.LParen);
            var condNode = ParseExpression();
            MatchToken(SyntaxType.RParen);
            var block = ParseScopedOrStatement();

            return new WhileNode(condNode, block);
        }

        //public SyntaxNode ParseFunctionExpression()
        //{
        //    MatchKeyword("func");

        //    bool @override = false;

        //    SyntaxToken? nameToken = null;

        //    if (IsKeyword("override"))
        //    {
        //        Position++;
        //        @override = true;
        //    }

        //    if (Current.Type == SyntaxType.Identifier)
        //        nameToken = MatchToken(SyntaxType.Identifier);

        //    (var args, var types) = ParseFunctionArgs();

        //    SyntaxNode block;

        //    if (Current.Type == SyntaxType.LBraces)
        //    {
        //        block = ParseScopedStatements();
        //    }
        //    else
        //    {
        //        var arrow = MatchToken(SyntaxType.Arrow);
        //        block = ParseScopedOrStatement();
        //        block = new ReturnNode(arrow, block);
        //    }

        //    return new FunctionDefinitionNode(nameToken, args, types, returnType, block,@override);
        //}
        public SyntaxNode ParseThrowExpression()
        {
            MatchKeyword("throw");

            var expr = ParseExpression();

            return new ThrowNode(expr);
        }

        public SyntaxNode ParseInstantiateExpression()
        {
            Position++;
            var ident = MatchToken(SyntaxType.Identifier);

            List<SyntaxNode> argumentNodes = new();

            if (Current.Type is SyntaxType.LParen)
            {
                Position++;

                if (Current.Type is SyntaxType.RParen)
                {
                    Position++;
                }
                else
                {
                    argumentNodes.Add(ParseExpression());

                    while (Current.Type is SyntaxType.Comma)
                    {
                        Position++;

                        argumentNodes.Add(ParseExpression());
                    }

                    MatchToken(SyntaxType.RParen);
                }
            }

            return new InstantiateNode(ident, argumentNodes);
        }

        public (List<SyntaxToken>, List<SyntaxToken>) ParseFunctionArgs()
        {
            MatchToken(SyntaxType.LParen);

            List<SyntaxToken> args = new();
            List<SyntaxToken> types = new();


            if (Current.Type == SyntaxType.RParen)
            {
                MatchToken(SyntaxType.RParen);
            }
            else
            {
                var type = MatchToken(SyntaxType.Identifier);
                var ident = MatchToken(SyntaxType.Identifier);
                types.Add(type);
                args.Add(ident);

                while (Current.Type == SyntaxType.Comma)
                {
                    Position++;
                    type = MatchToken(SyntaxType.Identifier);
                    ident = MatchToken(SyntaxType.Identifier);
                    types.Add(type);
                    args.Add(ident);
                }

                MatchToken(SyntaxType.RParen);
            }

            return (args,types);
        }

        public SyntaxNode BinaryOperation(Func<SyntaxNode> leftParse, List<SyntaxType> allowedTypes, Func<SyntaxNode> rightParse = null)
        {
            var left = leftParse();
            SyntaxNode right;
            while (allowedTypes.Contains(Current.Type))
            {
                SyntaxToken operatorToken = Current;
                Position++;
                right = (rightParse ?? leftParse)();

                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }

        public ParsingException MakeException(string msg)
        {
            return new ParsingException(msg, this, Position);
        }
        public SyntaxType MapEqualsTokens(SyntaxType type) => type switch
        {
            SyntaxType.PlusEqu => SyntaxType.Plus,
            SyntaxType.MinusEqu => SyntaxType.Minus,
            SyntaxType.ModEqu => SyntaxType.Mod,
            SyntaxType.MulEqu => SyntaxType.Mul,
            SyntaxType.DivEqu => SyntaxType.Div,
            _ => throw new Exception(type + " is not a equals token.")
        };

        public SyntaxType MapDoubleTokens(SyntaxType type) => type switch
        {
            SyntaxType.PlusPlus => SyntaxType.Plus,
            SyntaxType.MinusMinus => SyntaxType.Minus,
            _ => throw new Exception(type + " is not a double token.")
        };
    }
}
