using FriedLanguage.BuiltinType;
using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage
{
    public class Interpreter
    {
        public Scope GlobalScope { get; private set; }

        public Interpreter()
        {
            GlobalScope = new(0);

            // Fixed default imports; should be kept as minimal as possible
            GlobalScope.Table["true"] = new FBool(true) { IsConstant = true };
            GlobalScope.Table["false"] = new FBool(false) { IsConstant = true };
            GlobalScope.Table["null"] = new FNull() { IsConstant = true };


            #region default class
            #region exception
            var ex = new FClass("exception", true, true);
            ex.InstanceBaseTable.Add(("$$ctor", new FNativeFunction(
                impl: (Scope scope, List<FValue> args) =>
                {
                    if (args[0] is not FClassInstance self) throw new Exception("unexpected error!");
                    if (args[1] is not FString message) throw new Exception("Expected argument 0 to be a string");

                    var val = self.GetValue("message",scope);
                    self.SetValue("message",message,scope);
                    return self;
                },
                expectedArgs: new() { "self", "message" }
            )));

            ex.InstanceBaseTable.Add(("message",new FString("empty exception")));
            GlobalScope.SetAdmin("exception", ex);
            #endregion
            GlobalScope.SetAdmin("Null", new FClass("Null", true, true));

            GlobalScope.SetAdmin("String", new FClass("String", true, true));
            GlobalScope.SetAdmin("Int", new FClass("Int", true, true));
            GlobalScope.SetAdmin("Bool", new FClass("Bool", true, true));
            GlobalScope.SetAdmin("Float", new FClass("Float", true, true));
            GlobalScope.SetAdmin("Double", new FClass("Double", true, true));
            GlobalScope.SetAdmin("Long", new FClass("Long", true, true));
            GlobalScope.SetAdmin("List", new FClass("List", true, true));
            GlobalScope.SetAdmin("Dict", new FClass("Dict", true, true));
            #endregion
        }

        public FValue Interpret(string text, ref InterpreterResult res)
        {
            Lexer lexer = new(text);
            res.LexedTokens = lexer.Lex();

            Parser p = new(res.LexedTokens, text);
            res.AST = p.Parse();

            res.LastValue = res.AST.Evaluate(GlobalScope);

            if (GlobalScope.State == ScopeState.ShouldReturn)
                return res.LastValue;
            else
                return new FNull();
        }

        public FValue Interpret(string text, ref TimingInterpreterResult res)
        {
            Stopwatch sw = new();

            sw.Start();
            Lexer lexer = new(text);
            res.Result.LexedTokens = lexer.Lex();
            res.LexTime = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            Parser p = new(res.Result.LexedTokens, text);
            res.Result.AST = p.Parse();
            res.ParseTime = sw.Elapsed.TotalMilliseconds;
            sw.Restart();

            res.Result.LastValue = res.Result.AST.Evaluate(GlobalScope);
            res.EvalTime = sw.Elapsed.TotalMilliseconds;
            sw.Stop();

            if (GlobalScope.State == ScopeState.ShouldReturn)
                return res.Result.LastValue;
            else
                return null;
        }
    }
}
