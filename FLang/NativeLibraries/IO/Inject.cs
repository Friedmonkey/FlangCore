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
                    new FlangMethod("append",File.Append,ClassOptions.Static,"string path", "string text"),
                    new FlangMethod("exists",File.Exists,ClassOptions.Static,"string path"),
            });
            var directoryClass = new FlangClass("Dir", extends: false,
            methods: new FlangMethod[]
            {
                    new FlangMethod("create",Dir.Create,ClassOptions.Static,"string path"),
                    new FlangMethod("delete",Dir.Delete,ClassOptions.Static,"string path"),
                    new FlangMethod("deleteForce",Dir.Delete,ClassOptions.Static,"string path", "bool force"),
                    new FlangMethod("getDirectories",Dir.GetDirectories,ClassOptions.Static,"string path"),
                    new FlangMethod("getDirs",Dir.GetDirectories,ClassOptions.Static,"string path"),
                    new FlangMethod("getFiles",Dir.GetFiles,ClassOptions.Static,"string path"),
                    new FlangMethod("getCurrent",Dir.GetCurrent,ClassOptions.Static),
                    new FlangMethod("exists",Dir.Exists,ClassOptions.Static,"string path"),
            });
            var pathClass = new FlangClass("Path", extends: false,
            methods: new FlangMethod[]
            {
                    new FlangMethod("combine",Path.Combine,ClassOptions.Static,"string path", "string path"),
                    new FlangMethod("combineList",Path.Combine,ClassOptions.Static,"list paths"),
                    new FlangMethod("getFileName",Path.GetFileName,ClassOptions.Static,"string path"),
                    new FlangMethod("getDirectoryName",Path.GetDirectoryName,ClassOptions.Static,"string path"),
                    new FlangMethod("getDirName",Path.GetDirectoryName,ClassOptions.Static,"string path"),
                    new FlangMethod("exists",Path.Exists,ClassOptions.Static,"string path"),
            });
            var JSONClass = new FlangClass("JSON", extends: false,
            methods: new FlangMethod[]
            {
                    new FlangMethod("serialize",JSON.Serialize,ClassOptions.Static,"object obj"),
                    new FlangMethod("deserialize",JSON.Deserialize,ClassOptions.Static,"string json"),
                    new FlangMethod("deserializeClass",JSON.DeserializeClass,ClassOptions.Static,"string json", "class clas"),
            });
            var apiClass = new FlangClass("Api", extends: false,
            methods: new FlangMethod[]
            {
                new FlangMethod("get",Api.Get,ClassOptions.Extend, "string url"),
                new FlangMethod("post",Api.Post,ClassOptions.Static, "string url", "string content"),
                new FlangMethod("postWithType",Api.Post,ClassOptions.Static, "string url", "string content", "string contentType"),
            });

            classes.Add(fileClass);
            classes.Add(directoryClass);
            classes.Add(pathClass);
            classes.Add(JSONClass);
            classes.Add(apiClass);
            return classes;
        }
    }
}
