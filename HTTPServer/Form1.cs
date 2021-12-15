using System.Windows.Forms;
using System.Text;
using System.Net;
using System.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;
using System.Web;
using HTTPLib;
using System.IO;
using System.Reflection;

namespace HTTPServer
{
    public partial class Form1 : Form
    {
        // Dictionary for server configuration info
        Dictionary<string, string> dictServerConfig = new Dictionary<string, string>();
        // Dictionary for plugins
        Dictionary<string, IHTTPMethodHandler> dictPlugins = new Dictionary<string, IHTTPMethodHandler>();


        // Listening socket
        private Socket socListen = null;

        // Do we need a Network Stream?
        private NetworkStream nsStream = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartStop_Click_1(object sender, EventArgs e)
        {
            if (btnStartStop.Text == "Start")
            {
                btnStartStop.Text = "Stop";
                // Start a thread that listens for TCP connection requests
                Task.Run(new Action(vListen));
                rtbLog.AppendText(DateTime.Now.ToString("r") + ": Starting server... \r\n");
            }
            else
            {
                // Text must be "Stop", so stop listening
                btnStartStop.Text = "Start";

                socListen.Close();
                socListen.Dispose();
            }

        }

        // A function that listens for TCP connection requests
        private void vListen()
        {
            socListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaListen = IPAddress.Parse("127.0.0.1");
            int iPortNum = 8080;
            IPEndPoint ipeListen = new IPEndPoint(ipaListen, iPortNum);
            socListen.Bind(ipeListen);
            socListen.Listen(5);
            this.Invoke(new Action<string>(rtbLog.AppendText), DateTime.Now.ToString("r") + ": Server started \r\n");
            // Run a task that accepts connection requests and processes them
            Task.Run(new Action(vAcceptConnects));
        }

        // A function that accepts connection requests
        private void vAcceptConnects()
        {
            while (true)
            {
                try
                {
                    Socket socConnection = socListen.Accept();
                    // Start a task to exchange data (process requests)
                    Task.Run(() => vProcessRequests(socConnection));
                }
                catch
                {
                    return;
                }

            }
        }

        // A function that processes requests
        // CHECK THIS FUNCTION
        private void vProcessRequests(Socket socConnection)
        {
            // Use the method in HTTPLib to receive and parse the HTTP request
            // Receive method needs a stream as a parameter. 
            nsStream = new NetworkStream(socConnection);
            HTTPRequest hrRequest = HTTPRequest.Receive(nsStream);
            // Show request and IP Address in rich textbox
            string strIP = socConnection.RemoteEndPoint.ToString();
            this.Invoke(new Action<string>(rtbLog.AppendText), DateTime.Now.ToString("r") + ": " + strIP + "\r\n" + hrRequest.Method + " " + hrRequest.URI + " " + hrRequest.Version + "\r\n");
            // Need to see whether the method is GET, OPTIONS, or neither,
            // call the correct method handler, then send the response to the client
            if (dictPlugins.ContainsKey(hrRequest.Method.ToUpper()))
            {           
                HTTPResponse Response = dictPlugins[hrRequest.Method.ToUpper()].GenResponse(hrRequest, dictPlugins, dictServerConfig);
                Response.Send(nsStream);
            }
            else
            {
                string strResponse = "HTTP/1.1 405 Method Not Allowed \r\nConnection: close \r\nDate: " + DateTime.Now.ToString("r") + "\r\nServer: " + dictServerConfig["ServerName"] + "\r\nAllow:";
                foreach(string strKey in dictPlugins.Keys)
                {
                    strResponse += strKey + ", ";
                }
                byte[] byResponse = Encoding.ASCII.GetBytes(strResponse);
                nsStream.Write(byResponse, 0, byResponse.Length);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load all of the plugin DLL's that are listed in the file PluginList.txt
            using (StreamReader srPlugins = new StreamReader("Plugins\\PluginList.txt"))
            {
                // Read a line from the file
                string strOneLine = srPlugins.ReadLine();
                // The line read is a valid line from the file if it is not null
                while (strOneLine != null)
                {
                    // Construct the complete file name
                    string strFileName = "Plugins\\" + strOneLine;
                    Assembly asmMethod = Assembly.LoadFrom(strFileName);
                    // Loop through the types exported by the DLL, create an instance of each one
                    // and add the instance to the list of operations
                    foreach (Type tMethod in asmMethod.ExportedTypes)
                    {
                        // Create an instance of the type and add it to the list
                        try
                        {
                            IHTTPMethodHandler IMethod = (IHTTPMethodHandler)asmMethod.CreateInstance(tMethod.FullName);
                            dictPlugins.Add(IMethod.MethodName.ToUpper(), IMethod);
                        }
                        catch { }
                    }
                    // Read the next line from the file
                    strOneLine = srPlugins.ReadLine();
                }
            }
            // Value needs to be whatever we choose to name our server
            dictServerConfig.Add("ServerName", "Final Project");
            // Value needs to be the absolute path of the document root directory
            // (concatenate the path of the document root to the URI)
            dictServerConfig.Add("DocumentRoot", Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\DocumentRoot");
        }
    }
}
