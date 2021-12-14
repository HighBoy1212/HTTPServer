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

        // This function must generate three headers: a connection header,
        // a date header, and a server header.
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

        // This function must generate a connection header,
        // a date header, and a server header. Also, two other headers:
        // Content-Length, and Content-Type. (total of 5 headers)
        public HTTPResponse GenResponse(HTTPRequest hrRequest,
                                Dictionary<string, IHTTPMethodHandler> dictPlugins,
                                Dictionary<string, string> dictServerConfig)
        {
            HTTPResponse GetReponse = new HTTPResponse();


        }
    }
}

