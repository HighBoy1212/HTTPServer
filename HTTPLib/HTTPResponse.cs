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

        public HTTPResponse(string strStatus, List<string> Headers, Stream Body = null)
        {
            strStatusLine = strStatus;
            lstHeaders = Headers;
            sBody = Body;
        }
        public void Send(Stream strmOut)
        {
            // NEED TO: Convert the response into bytes (in blocks of 1024 bytes)
            // and write it to strmOut
            string strStatus = strStatusLine + "\r\n";
            byte[] byStatus = Encoding.ASCII.GetBytes(strStatus);
            string strHeaders = "";
            foreach (string strH in lstHeaders)
            {
                strHeaders += strH + "\r\n";
            }
            strHeaders += "\r\n";
            byte[] byHeaders = Encoding.ASCII.GetBytes(strHeaders);
            // Send status, then headers, then body
            strmOut.Write(byStatus, 0, byStatus.Length);
            strmOut.Write(byHeaders, 0, byHeaders.Length);
            if (sBody != null)
            {
                byte[] byBody = new byte[1024];
                int iBytesRead;
                while ((iBytesRead = sBody.Read(byBody, 0, 1024)) > 0)
                {
                    strmOut.Write(byBody, 0, iBytesRead);
                }
                sBody.Close();
            }
        }
    }
}
