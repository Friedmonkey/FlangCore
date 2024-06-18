using FriedLanguage;
using FriedLanguage.BuiltinType;
using FriedLanguage.Models.Parsing;
using FriedLanguage.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static FriedLang.NativeLibraries.IO;

namespace FriedLang.NativeLibraries
{
    public partial class IO
    { 
        public static class JSON
        {
            public static FValue Serialize(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FValue obj)
                    throw new Exception("Expected argument 0 to be a value");

                var objec = FLang.FromFriedVar(obj);

                var json = JsonConvert.SerializeObject(objec);

                return new FString(json);
            }
            public static FValue Deserialize(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString json)
                    throw new Exception("Expected argument 0 to be a string");

                //var objec = JsonConvert.DeserializeObject<List<(object,object)>>(json.Value);

                FLang Flang = new FLang();
                GlobalState.DisableLogicalIdentMessages();

                Flang.ImportNative<Lang>("lang");

                string code = $$"""
                keyword disable while;
                keyword disable true;
                keyword disable if;
                keyword disable for;
                keyword disable lable;
                keyword disable goto;
                keyword disable keyword
                return {{json.Value}};
""";

                var obj = Flang.RunCode(code, false);

                if (obj == null)
                {
                    if (Flang.LastError.EndsWith("is disabled and cant be used."))
                    {
                        return new FException("json parsing failed, please input actual json");    
                    }
                    return new FException(Flang.LastError);    
                }

                return (FValue)obj;
            }
            //public static SyntaxNode DeserializeToDynamic(string input)
            //{
            //    var lsq = MatchToken(SyntaxType.LBraces);
            //    SyntaxToken rsq;

            //    List<(SyntaxNode tok, SyntaxNode expr)> dict = new();

            //    if (Current.Type == SyntaxType.RBraces)
            //    {
            //        rsq = MatchToken(SyntaxType.RBraces);
            //    }
            //    else
            //    {
            //        //var tok = default(SyntaxToken);

            //        //            if (Current.Type == SyntaxType.String || Current.Type == SyntaxType.Int)
            //        //            {
            //        //	Position++;
            //        //	tok = Peek(-1);
            //        //}else
            //        //	throw MakeException("Unexpected token " + Current.Type + "; expected string or int");



            //        var tok = ParseExpression();
            //        _ = MatchToken(SyntaxType.Colon);
            //        var expr = ParseExpression();
            //        dict.Add((tok, expr));

            //        while (Current.Type == SyntaxType.Comma)
            //        {
            //            Position++;

            //            //tok = MatchToken(SyntaxType.String);
            //            tok = ParseExpression();
            //            _ = MatchToken(SyntaxType.Colon);
            //            expr = ParseExpression();

            //            dict.Add((tok, expr));
            //        }

            //        rsq = MatchToken(SyntaxType.RBraces);
            //    }

            //    return new DynamicNode(dict, lsq, rsq);
            //}
        }
    }
}
