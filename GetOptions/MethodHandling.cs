using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPLib;

namespace GetOptions
{
    public class Get : IHTTPMethodHandler
    {
        public string MethodName
        {
            get
            {
                return "GET";
            }
        }

        // This function must generate three headers: a connection header,
        // a date header, and a server header.
        public HTTPResponse GenResponse(HTTPRequest hrRequest,
                                Dictionary<string, IHTTPMethodHandler> dictPlugins,
                                Dictionary<string, string> dictServerConfig) {
            List<string> lstHeaders = new List<string>();
            lstHeaders.Add("Connection: close");
            lstHeaders.Add("Date: " + DateTime.Now.ToString("r"));
            lstHeaders.Add("Server: " + dictServerConfig["ServerName"]);
            HTTPResponse Response = new HTTPResponse("", lstHeaders);
            return Response;
        }
    }

    public class Options : IHTTPMethodHandler
    {
        public string MethodName
        {
            get
            {
                return "OPTIONS";
            }
        }

        // This function must generate a connection header,
        // a date header, and a server header. Also, two other headers:
        // Content-Length, and Content-Type. (total of 5 headers)
        public HTTPResponse GenResponse(HTTPRequest hrRequest,
                                Dictionary<string, IHTTPMethodHandler> dictPlugins,
                                Dictionary<string, string> dictServerConfig)
        {
            // Status
            string strStatus = "HTTP/1.1 200 OK";
            // Headers
            List<string> lstHeaders = new List<string>();
            lstHeaders.Add("Connection: close");
            lstHeaders.Add("Date: " + DateTime.Now.ToString("r"));
            lstHeaders.Add("Server: " + dictServerConfig["ServerName"]);
            string strAllow = "Allow: ";

            // Do not know how to make it so there is no comma at the end
            foreach (string strKey in dictPlugins.Keys)
            {
                strAllow += strKey + ", ";
            }
            lstHeaders.Add(strAllow);
            HTTPResponse Response = new HTTPResponse(strStatus, lstHeaders);
            return Response;

        }
    }
}

