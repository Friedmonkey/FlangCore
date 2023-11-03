using FriedLanguage.Models.Parsing;
using FriedLanguage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriedLanguage.BuiltinType;

namespace FriedLanguage.Extentions
{
    public static class ListExtensions
    {
        public static int GetEndingPosition(this IEnumerable<SyntaxNode> list, int fallback)
        {
            if (list.Count() == 0) return fallback;
            return list.Last().EndPosition;
        }

        public static int GetStartingPosition(this IEnumerable<SyntaxNode> list, int fallback)
        {
            if (list.Count() == 0) return fallback;
            return list.First().StartPosition;
        }

        // TODO: Maybe use an interface instead
        public static int GetEndingPosition(this IEnumerable<SyntaxToken> list, int fallback)
        {
            if (list.Count() == 0) return fallback;
            return list.Last().EndPosition;
        }

        public static int GetStartingPosition(this IEnumerable<SyntaxToken> list, int fallback)
        {
            if (list.Count() == 0) return fallback;
            return list.First().Position;
        }

        public static bool ExistsIn(this List<(string key, FValue val)> list, string Name)
        { 
            foreach (var (key, value) in list) 
            {
                if (key == Name)
                    return true;
            }
            return false;
        }
        public static bool UpdateIn(this List<(string key, FValue val)> list, string Name,FValue newVal)
        {
            foreach (var (key, value) in list)
            {
                if (key == Name)
                {
                    list.Remove((key,value));
                    list.Add((key,newVal));
                    return true;
                }
            }
            return false;
        }


    }
}
