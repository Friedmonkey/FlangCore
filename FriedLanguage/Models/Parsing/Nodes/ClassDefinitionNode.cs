using FriedLanguage.BuiltinType;
using FriedLanguage.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FriedLanguage.Models.Parsing.Nodes
{
    internal class ClassDefinitionNode : SyntaxNode
    {
        private SyntaxToken className;
        private IEnumerable<SyntaxNode> body;
        private readonly bool strict;
        private readonly bool extend;

        public ClassDefinitionNode(SyntaxToken className, IEnumerable<SyntaxNode> body, bool strict, bool extend) : base(className.Position, body.GetEndingPosition(className.EndPosition))
        {
            this.className = className;
            this.body = body;
            this.strict = strict;
            this.extend = extend;
        }

        public override NodeType Type => NodeType.ClassDefinition;

        public override FValue Evaluate(Scope scope)
        {

            if (this.extend)
            {
                var clas = scope.Get(className.Text);
                if (clas is FClass fclass)
                {
                    foreach (var bodyNode in body)
                    {
                        if (bodyNode is ClassFunctionDefinitionNode cfdn)
                        {
                            var funcRaw = cfdn.Evaluate(scope);
                            if (funcRaw is not FFunction func) throw new Exception("Expected ClassFunctionDefinitionNode to return FFunction");

                            if (func.IsClassInstanceMethod)
                            {
                                //if (func.ExpectedArgs.IndexOf("self") == -1) 
                                //    func.ExpectedArgs.Insert(0, "self");

                                if (func.ExpectedArgs.IndexOf("self") == -1)
                                { 
                                    func.ExpectedArgs.Insert(0, "self");
                                    func.ExpectedArgTypes.Insert(0, "classinstance"); //idk?
                                }
                                if (func.ExpectedArgs.IndexOf("this") == -1)
                                {
                                    func.ExpectedArgs.Insert(0, "this");
                                    func.ExpectedArgTypes.Insert(0, "classinstance");
                                }
                                fclass.InstanceBaseTable.Add((func.FunctionName, func));
                            }
                            else
                            {
                                fclass.StaticTable.Add((func.FunctionName, func));
                            }
                        }
                        else if (bodyNode is ClassPropDefinitionNode cpdn)
                        {
                            //var val = cpdn.Expression.Evaluate(scope);
                            FValue val = FValue.Null;
                            if (cpdn.Expression is null)
                            {
                                switch (cpdn.VarType.Text)
                                {
                                    case "string":
                                        val = new FString("");
                                        break;
                                    case "int":
                                        val = new FInt(0);
                                        break;
                                    case "float":
                                        val = new FFloat(0);
                                        break;
                                    case "double":
                                        val = new FDouble(0);
                                        break;
                                    case "long":
                                        val = new FLong(0);
                                        break;
                                    case "bool":
                                        val = new FBool(false);
                                        break;
                                    case "list":
                                        val = new FList();
                                        break;
                                    case "dictionary":
                                        val = new FDictionary();
                                        break;
                                }
                            }
                            else
                                val = cpdn.Expression.Evaluate(scope);

                            val.IsConstant = cpdn.IsConstant;
                            val.TypeIsFixed = cpdn.FixedType;
                            val.IsNullable = cpdn.IsNullable;


                            if (!cpdn.IsStatic)
                            {
                                fclass.InstanceBaseTable.Add((cpdn.Name.Text, val));
                            }
                            else
                            {
                                fclass.StaticTable.Add((cpdn.Name.Text, val));
                            }
                        }
                        else
                        {
                            throw new Exception("Unexpected node in extended class");
                        }
                    }
                }
                else 
                {
                    throw new Exception("Trying to extend class that doest exist");
                }
                scope.SetAdmin(className.Text, fclass);
                return fclass;
            }
            var @class = new FClass();
            @class.Name = className.Text;
            @class.Strict = strict;

            foreach (var bodyNode in body)
            {
                if (bodyNode is ClassFunctionDefinitionNode cfdn)
                {
                    var funcRaw = cfdn.Evaluate(scope);
                    if (funcRaw is not FFunction func) throw new Exception("Expected ClassFunctionDefinitionNode to return SFunction");

                    if (func.IsClassInstanceMethod)
                    {
                        if (func.ExpectedArgs.IndexOf("self") == -1)
                        { 
                            func.ExpectedArgs.Insert(0, "self");
                            func.ExpectedArgTypes.Insert(0,"classinstance");
                        }

                        @class.InstanceBaseTable.Add((func.FunctionName, func));
                    }
                    else
                    {
                        @class.StaticTable.Add((func.FunctionName, func));
                    }
                }
                else if (bodyNode is ClassPropDefinitionNode cpdn)
                {
                    FValue val = FValue.Null;
                    if (cpdn.Expression is null)
                    {
                        switch (cpdn.VarType.Text)
                        {
                            case "string":
                                val = new FString("");
                                break;
                            case "int":
                                val = new FInt(0);
                                break;
                            case "float":
                                val = new FFloat(0);
                                break;
                            case "double":
                                val = new FDouble(0);
                                break;
                            case "long":
                                val = new FLong(0);
                                break;
                            case "bool":
                                val = new FBool(false);
                                break;
                            case "list":
                                val = new FList();
                                break;
                            case "dictionary":
                                val = new FDictionary();
                                break;
                        }
                    }
                    else
                        val = cpdn.Expression.Evaluate(scope);

                    ////cpdn.copyMeta(ref val);
                    //val.IsConstant = false;

                    val.IsConstant = cpdn.IsConstant;
                    val.TypeIsFixed = cpdn.FixedType;
                    val.IsNullable = cpdn.IsNullable;


                    if (!cpdn.IsStatic)
                    {
                        @class.InstanceBaseTable.Add((cpdn.Name.Text, val));
                    }
                    else
                    {
                        @class.StaticTable.Add((cpdn.Name.Text, val));
                    }
                }
                else
                {
                    throw new Exception("Unexpected node in class definition");
                }
            }

            scope.Set(className.Text, @class);
            return @class;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return new TokenNode(className);
            foreach (var n in body) yield return n;
        }
    }
}
