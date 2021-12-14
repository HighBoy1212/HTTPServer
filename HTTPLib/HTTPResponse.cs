using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HTTPLib {
    public class HTTPResponse {
        // Fields
        private string strStatusLine;
        private List<string> lstHeaders;
        // Check this, but if a 
        private Stream sBody = Stream.Null;

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
            // NEED TO: Convert the response into bytes (in blocks of 1024 bytes)
            // and write it to strmOut

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
