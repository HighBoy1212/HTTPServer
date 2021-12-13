using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPTest {
    class Program {
        static void Main(string[] args) {
            // Set up the components of an HTTP request message.
            string hmMethod = "GET";
            string strURI = "/Examples/Test/ABC.html";
            string strVersion = "HTTP/1.1";
            List<string> lstrHeaders = new List<string>();
            lstrHeaders.Add("Host: BroncoL.hastings.edu");
            lstrHeaders.Add("Accept-Language: en-us");
            lstrHeaders.Add("Connection: close");

            // Convert to a message, with proper line endings.
            string strRequestLine = hmMethod + " " + strURI + " " + strVersion;
            string strMessage = strRequestLine + "\r\n" + String.Join("\r\n", lstrHeaders) + "\r\n\r\n";

            // Convert to an array of bytes.
            byte[] byMessage = Encoding.ASCII.GetBytes(strMessage);

            // Create a memory stream and write the message to it.
            System.IO.MemoryStream msStream = new System.IO.MemoryStream();
            msStream.Write(byMessage, 0, byMessage.Length);

            // Move back to the beginning of the stream, so we can read its contents.
            msStream.Seek(0, System.IO.SeekOrigin.Begin);

            // Create an HTTPRequest object.
            HTTPLib.HTTPRequest hrRequest = HTTPLib.HTTPRequest.Receive(msStream);
            // Display it.
            Console.WriteLine(hrRequest.ToString());
            // Wait for input from the user, so the window stays open.
            Console.ReadLine();
        }
    }
}
