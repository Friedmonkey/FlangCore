using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage.Models
{
    public class Context
    {
        public FValue this[string a]
        {
            get
            {
                return scope.Get(a);
            }
            set
            {
                scope.SetAdmin(a, value);
            }
        }
        public FClassInstance self
        {
            get => (FClassInstance)this["self"];
            set => this["self"] = value;
        }
        public T getSelf<T>(string key) where T : FValue
        {
            return (self.GetValue(key) as T);
        }
        public void setSelf(string key,FValue newVal)
        {
            self.SetValue(key, newVal);
        }
        public string getSelfStr(string key)
        {
            return (self.GetValue(key) as FString).Value;
        }
        public void setSelfStr(string key, string newVal)
        {
            self.SetValue(key, new FString(newVal));
        }

        public T get<T>(string key) where T : FValue
        {
            return (scope.Get(key) as T);
        }
        public void set(string key, FValue newVal)
        {
            scope.SetAdmin(key, newVal);
        }
        public string getStr(string key)
        {
            return (scope.Get(key) as FString).Value;
        }
        public void setStr(string key, string newVal)
        {
            scope.SetAdmin(key, new FString(newVal));
        }
        public static Scope scope;
    }
}
