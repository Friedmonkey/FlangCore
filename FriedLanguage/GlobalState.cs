using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage
{
    public static class GlobalState
    {
        private static bool isInConstructor = false;
        private static bool csCodeGeneration = true;
        public static void EnterConstructor()
        { 
            isInConstructor = true;
        }
        public static void ExitConstructor()
        {
            isInConstructor = false;
        }
        public static bool IsInConstructor()
        {
            return isInConstructor;
        }

        public static void AllowCodeGeneration()
        {
            csCodeGeneration = true;
        }
        public static void DisallowCodeGeneration()
        {
            csCodeGeneration = false;
        }
        public static bool IsCodeGenerationAllowed()
        {
            return csCodeGeneration;
        }
    }
}
