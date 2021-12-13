using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPLib;

namespace GetOptions
{
    public class Options : IHTTPMethodHandler
    {
        public string MethodName
        {
            get
            {
                return "OPTIONS";
            }
        }
        public HTTPResponse GenResponse(HTTPRequest hrRequest,
                                Dictionary<string, IHTTPMethodHandler> dictPlugins,
                                Dictionary<string, string> dictServerConfig) {
            
        }
    }

    public class Get : IHTTPMethodHandler
    {
        public string MethodName
        {
            get
            {
                return "GET";
            }
        }
        public HTTPResponse GenResponse(HTTPRequest hrRequest,
                                Dictionary<string, IHTTPMethodHandler> dictPlugins,
                                Dictionary<string, string> dictServerConfig)
        {
            HTTPResponse GetReponse = new HTTPResponse();

        }
    }
}

