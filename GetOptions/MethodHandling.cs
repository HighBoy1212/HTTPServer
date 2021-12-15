using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPLib;
using System.IO;

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
            // List for headers
            List<string> lstHeaders = new List<string>();
            lstHeaders.Add("Connection: close");
            lstHeaders.Add("Date: " + DateTime.Now.ToString("r"));
            lstHeaders.Add("Server: " + dictServerConfig["ServerName"]);

            // Get the absolute path
            string strAbsolutePath = dictServerConfig["DocumentRoot"] + hrRequest.URI;
            // Get the Content-length (length of body in bytes)
            try {
                FileStream fsBody = File.Open(strAbsolutePath, FileMode.Open, FileAccess.Read);
                int iLength = (int)fsBody.Length;
                lstHeaders.Add("Content-Length: " + iLength.ToString());               
                // Dictionary for MIME Types
                Dictionary<string, string> dictMimes = new Dictionary<string, string>();
                dictMimes.Add(".htm", "text/html");
                dictMimes.Add(".html", "text/html");
                dictMimes.Add(".css", "text/css");
                dictMimes.Add(".js", "text/javascript");
                dictMimes.Add(".gif", "image/gif");
                dictMimes.Add(".jpg", "image/jpeg");
                dictMimes.Add(".jpeg", "image/jpeg");
                dictMimes.Add(".png", "image/png");
                // Get file extension and use dictionary to add Content-Type header
                string strExtension = Path.GetExtension(strAbsolutePath);
                if (dictMimes.ContainsKey(strExtension))
                {
                    lstHeaders.Add("Content-Type: " + dictMimes[strExtension]);
                }
                else
                {
                    lstHeaders.Add("Content-Type: text/plain");
                }
                HTTPResponse Response = new HTTPResponse("HTTP/1.1 200 OK", lstHeaders, fsBody);
                return Response;
            }
            catch { 
                return new HTTPResponse("HTTP/1.1 404 Not Found", lstHeaders); 
            }         
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

