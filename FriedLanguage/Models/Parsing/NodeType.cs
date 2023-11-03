using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models.Parsing
{
    public enum NodeType
    {
        Return,
        BinaryExpression,
        Token,
        BooleanLiteral,
        Block,
        CsBlock,
        Continue,
        Break,
        Unmatch,
        Label,
        Goto,
        Switch,
        InitVariable,
        AssignVariable,
        UnaryExpression,
        Dot,
        Call,
        IntLiteral,
        FloatLiteral,
        StringLiteral,
        Identifier,
        List,
        If,
        TryCatch,
        For,
        Foreach,
        Cast,
        While,
        FunctionDefinition,
        NativeImport,
        Instantiate,
        ClassDefinition,
        ClassFunctionDefinition,
        DotAssign,
        Export,
        Import,
        ClassPropertyDefinition,
        Repeat,
        Dict,
        DictGet
    }
}
