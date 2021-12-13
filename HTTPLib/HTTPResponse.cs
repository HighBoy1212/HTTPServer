using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HTTPLib {
    public class HTTPResponse {
        // Fields
        private string strStatusLine;
        private List<string> lstHeaders;
        private Stream sBody;

        public string Status
        {
            get
            {
                return strStatusLine;
            }
        }
        public List<string> Headers
        {
            get
            {
                return lstHeaders;
            }
        }
        public Stream Body
        {
            get
            {
                return sBody;
            }
        }

        public HTTPResponse(string strStatus, List<string> Headers, Stream Body)
        {
            strStatusLine = strStatus;
            lstHeaders = Headers;
            sBody = Body;
        }
        public void Send(Stream strmOut)
        {
            string strHeaders = "";
            foreach(string strH in lstHeaders)
            {
                if(strH == "Connection")
                {
                    strHeaders += strH + ": close \r\n";
                }
                else if (strH == "Date")
                {
                    strHeaders += strH + ": " + DateTime.Now.ToString("r");
                }
                else if (strH == "Server")
                {
                    strHeaders += strH + ": " + "some name";
                }
                else if (strH == "Allow")
                {

                }
            }

        }
    }
}
