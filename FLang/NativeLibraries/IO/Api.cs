
using FriedLanguage;
using FriedLanguage.BuiltinType;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FriedLang.NativeLibraries
{
    public partial class IO
    { 
        public static class Api
        {
            public static FValue Get(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString urlFvalue)
                    throw new Exception("Expected argument 0 to be a string");

                string url = urlFvalue.Value;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string strResult = response.Content.ReadAsStringAsync().Result;

                        return new FString(strResult);
                    }
                    else
                    {
                        return FNull.Null;
                    }
                }
            }
            public static FValue Post(Scope scope, List<FValue> arguments)
            {
                if (arguments[0] is not FString urlFvalue)
                    throw new Exception("Expected argument 0 to be a string");

                if (arguments[1] is not FString content)
                    throw new Exception("Expected argument 0 to be a string");


                string contentType = "application/json";

                if (arguments[2] is FString contentTypeFvalue)
                {
                    contentType = contentTypeFvalue.Value;
                }

                string url = urlFvalue.Value;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    // Prepare the request data
                    var requestContent = new StringContent(content.Value, System.Text.Encoding.UTF8, contentType);

                    HttpResponseMessage response = client.PostAsync(url, requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string strResult = response.Content.ReadAsStringAsync().Result;

                        return new FString(strResult);
                    }
                    else
                    {
                        return FNull.Null;
                    }
                }
            }
            
        }
    }
}
