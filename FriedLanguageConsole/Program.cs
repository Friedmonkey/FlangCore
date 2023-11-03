using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlTypes;
using System.IO;
using System.Threading.Tasks;
using FriedLang;
using FriedLanguage;
using FriedLanguage.BuiltinType;
using FriedLang.NativeLibraries;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace FriedLanguageConsole
{
	public class webIO : IO
	{
		public override void Intercept()
		{
			InterReplaceMethod("print",PrintBlue);
			InterRemoveClass("File");
		}
		public static FValue PrintBlue(Scope scope, List<FValue> arguments)
		{
            Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(arguments.First().SpagToCsString());
            Console.ResetColor();
			return arguments.First();
		}
	}
    //			//class strict printer
    //{
    //	prop text = "a";
    //	func ctor(txt)
    //	{
    //		self.text = txt;
    //	}
    //	func write()
    //	{
    //		print(self.text);
    //	}
    //}

    //var p = new printer("Hiiiii :3");
    //p.write();
    //p.write();
    //p.write();
    //p.text = "Something else :p"
    ////p.lollers = "oop xdxdxd" //can add vars that dont exist
    //p.write();
    //p.write();
    ////p.text = p.lollers;
    //p.write();
    //p.write();

    //class extend String
    //{
    //	prop static empty = "";
    //}
    //import native io;
    //import native lang;

    //class person
    //{
    //    string name = "";
    //    person(nam)
    //    {
    //        var name = "lol"; 
    //        self.name = nam;
    //    }
    //    void hello()
    //    {
    //        print("Hello my name is "+self.name);
    //    }
    //}

    //var p = new person("jhonn");
    //p.hello();
    //return p;

    //import native io;

    //        print("what is your first name?");
    //string fname = read();
    //print("what is your last name?");
    //string lname = read();
    //print("Hello {fname+\" \\n \"+lname}"$);

    ///switch case
    ///goto statement
    //dll import/interopt?
    ///add class
    ///add var (_GET["url"])
    ///add double
    ///add bool
    ///add operators as keyword = is || or && and
    //add tenery opearot ? :
    //add nullcoleancane opator ??
    ///add unmatch keyword to exit if and go to else or exit case and go to default
    //add declass keyword?
    internal class Program
    {
        static void Main(string[] args)
        {
            FLang fLang = new FLang();
			fLang.ImportNative<IO>("io");
			fLang.ImportNative<Lang>("lang");

			fLang.AddVariable("pi",Math.PI);

            //MessageBox.Show("hi","popup",MessageBoxButtons.YesNoCancel,MessageBoxIcon.None);
            string code = """
            
            code += "void print(string message)";
            code += "{                         ";
            code += "    import native io;     ";
            code += "    print(message);       ";
            code += "}                         ";
            code += "import native lang;       ";
            code += "keyword disable csharp;   ";
            code += "keyword disable import;   ";
            code += "keyword disable keyword;  ";

            import native io;
            var a = read();
            print("Hello world!");
            return a;
""";


            if (args.Length > 0)
                code = File.ReadAllText(args[0]);


            var output = fLang.RunCode(code);
			if (output != null) 
			{
				if (output is IEnumerable<object> objs)
					Console.WriteLine("output was:" + string.Join(", ", objs));
				else
					Console.WriteLine("output was:" + output);
			}
            Console.ReadLine(); //so console doest exit
        }
    }
}
//import native io;
//import native lang;

//class strict person
//            {
//                string name; //works
//string name //doest work
//                string name = "aaa"; //works

//void cctor

//                func ctor(nam, ag)
//                {
//    self.name = nam;
//    self.age = ag;
//}
//func murder()
//{
//    self.alive = false;
//}
//            }

//            var p = new person("Jhonny", 56);
//p.murder();
//print(p.name + " Died at the age of " + p.age);
//p.name = "Dead Jhonny";
//return p;

////bool test = false;
////test = true;
////return test
