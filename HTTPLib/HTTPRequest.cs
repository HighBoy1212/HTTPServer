using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HTTPLib {
    public class HTTPRequest {
        // Fields.
        // Method, URI, HTTP version, list of headers.
        private string strMethod;
        private string strURI;
        private string strVersion;
        private Dictionary<string, string> dictHeaders;

        // Properties.
        public string Method {
            get {
                return strMethod;
            }
        }

        public string URI {
            get {
                return strURI;
            }
        }

        public string Version {
            get {
                return strVersion;
            }
        }

        public Dictionary<string, string> Headers {
            get {
                return dictHeaders;
            }
        }

        // Methods.
        // Constructor. Initialize the fields from the parameters passed in.
        public HTTPRequest(string strM, string strU, string strV, Dictionary<string, string> dictH) {
            // Convert method to uppercase and decode percent-encoded characters in the URI.
            this.strMethod = strM.ToUpper();
            this.strURI = System.Web.HttpUtility.UrlDecode(strU);
            this.strVersion = strV;
            this.dictHeaders = dictH;
        }

        public static HTTPRequest Receive(Stream strmInput) {
            // A string for the line we read and a list of strings for the headers.
            string strOneLine;
            Dictionary<string, string> dictHdrs = new Dictionary<string, string>();
            // Read the first line, which is the request line.
            strOneLine = ReadOneLine(strmInput);
            // Split the request line on spaces.
            string[] strPieces = strOneLine.Split(' ');
            // Read header lines until we read a blank line.
            while ((strOneLine = ReadOneLine(strmInput)) != "") {
                int iColon = strOneLine.IndexOf(':');
                string strName = strOneLine.Substring(0, iColon).ToLower();
                string strValue = strOneLine.Substring(iColon + 1).TrimStart();
                dictHdrs[strName] = strValue;
            }
            // Finished. Convert into an HTTPRequest object and return it.
            return new HTTPRequest(strPieces[0], strPieces[1], strPieces[2], dictHdrs);
        }

        // Enumeration for the state in the FSM for reading one line.
        private enum OneLineStates {
            Line,
            CR,
            Done
        }

        // Read one line from the input stream.
        private static string ReadOneLine(Stream strmInput) {
            // Initially we are reading the line, and we have an empty list of the bytes
            // read so far.
            OneLineStates olsCurrentState = OneLineStates.Line;
            List<byte> lstBytes = new List<byte>();
            // Read bytes as long as more bytes to read and not done yet.
            int iNextByte = 0;
            while (olsCurrentState != OneLineStates.Done &&
                (iNextByte = strmInput.ReadByte()) != -1) {
                // Look at our current state, and the byte read, to determine what to do.
                switch (olsCurrentState) {
                    case OneLineStates.Line:
                        // Check whether read in a CR (0x0D).
                        if (iNextByte == 0x0D) {
                            olsCurrentState = OneLineStates.CR;
                        } else {
                            // This byte is part of the current line.
                            lstBytes.Add((byte)iNextByte);
                        }
                        break;

                    case OneLineStates.CR:
                        // Check whether we read in a LF (0x0A).
                        if (iNextByte == 0x0A) {
                            // End of the line. Change to Done state.
                            olsCurrentState = OneLineStates.Done;
                        } else {
                            // The CR we read before and this byte are part of the line.
                            // Add both to the byte list and change back to Line state.
                            lstBytes.Add(0x0D);
                            lstBytes.Add((byte)iNextByte);
                            olsCurrentState = OneLineStates.Line;
                        }
                        break;

                    case OneLineStates.Done:
                        // We should never get here. If we do, throw an exception.
                        throw new Exception("Bug in 'ReadOneLine'");
                }
            }
            // Have finished reading one line. Convert to a string and return it.
            byte[] byLineBytes = lstBytes.ToArray();
            string strLine = Encoding.ASCII.GetString(byLineBytes);
            return strLine;
        }

        // Override the ToString method.
        public override string ToString() {
            // Build up the output string in pieces.
            string strOutput = "";
            // Show the method.
            strOutput += "Method: " + this.strMethod + "\r\n";
            // Show the URI.
            strOutput += "URI: " + this.strURI + "\r\n";
            // Show the version.
            strOutput += "Version: " + this.strVersion + "\r\n";
            // List the headers.
            strOutput += "Headers:\r\n";
            foreach (string strOneHdr in dictHeaders.Keys) {
                strOutput += "\t" + strOneHdr + ": " + dictHeaders[strOneHdr] + "\r\n";
            }
            return strOutput;
        }
    }
}
