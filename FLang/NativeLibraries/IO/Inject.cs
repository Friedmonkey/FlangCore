using System.Collections.Generic;

namespace FriedLang.NativeLibraries
{
    public partial class IO : LanguageExtention
    {
        public override List<FlangMethod> InjectMethods()
        {
            List<FlangMethod> methods = new()
            {
                new FlangMethod("print", Global.Print, "string message"),

                new FlangMethod("read", Global.Read)
            };

            return methods;
        }
        public override List<FlangClass> InjectClassus()
        {
            List<FlangClass> classes = new List<FlangClass>();

            var fileClass = new FlangClass("File", extends: false,
            methods: new FlangMethod[]
            {
                    new FlangMethod("write",File.Write,ClassOptions.Static,"string path","string content"),
                    new FlangMethod("read",File.Read,ClassOptions.Static,"string path"),
                    new FlangMethod("readAllLines",File.ReadAllLines,ClassOptions.Static,"string path"),
                    new FlangMethod("readLines",File.ReadAllLines,ClassOptions.Static,"string path"),
                    new FlangMethod("exists",File.Exists,ClassOptions.Static,"string path"),
            });
            var apiClass = new FlangClass("Api", extends: false,
            methods: new FlangMethod[]
            {
                new FlangMethod("get",Api.Get,ClassOptions.Extend, "string url"),
                new FlangMethod("post",Api.Post,ClassOptions.Static, "string url", "string content"),
                new FlangMethod("postWithType",Api.Post,ClassOptions.Static, "string url", "string content", "string contentType"),
            });

            classes.Add(fileClass);
            classes.Add(apiClass);
            return classes;
        }
    }
}
