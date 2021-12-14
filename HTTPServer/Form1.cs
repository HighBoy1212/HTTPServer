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
            // Load all method plugins from the Plugins dictionary
            Dictionary<string, IHTTPMethodHandler> dictPlugins = new Dictionary<string, IHTTPMethodHandler>();

            // Value needs to be whatever we choose to name our server
            dictServerConfig.Add("ServerName", "");
            // Value needs to be the absolute path of the document root directory
            // (concatenate the path of the document root to  the URI)
            dictServerConfig.Add("DocumentRoot", "");
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text == "Start")
            {
                btnStartStop.Text = "Stop";
                // Start a thread that listens for TCP connection requests
                Task.Run(new Action(vListen));
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
            // These are parameters HTTPResponse needs
            string strStatus = "";
            List<string> lstHeaders = new List<string>();

            // Use the method in HTTPLib to receive and parse the HTTP request
            // Receive method needs a stream as a parameter. 
            nsStream = new NetworkStream(socConnection);
            HTTPRequest hrRequest = HTTPRequest.Receive(nsStream);

            bool bRunning = true;
            // Need to see whether the method is GET, OPTIONS, or neither,
            // call the correct method handler, then send the response to the client
            while (bRunning)
            {
                // Method, URI, Version, Headers (dict)
                (string strMethod, string strURI, string strVersion, Dictionary<string, string> dictHdrs) = hrRequest;
                    switch (strMethod.ToUpper())
                    {
                        case "GET":
                            // Need to call the "GET" version of GenResponse
                            GenResponse(hrRequest, dictPlugins, dictServerConfig);

                            nsStream.Close();
                            bRunning = false;
                            break;
                        case "OPTIONS":
                            // Need to call the "OPTIONS' version of GenResponse
                            GenResponse(hrRequest, dictPlugins, dictServerConfig);

                            nsStream.Close();
                            bRunning = false;
                            break;
                        default:
                            // Method is not GET or OPTIONS, send a response saying
                            // "HTTP/1.1 405 Method Not Allowed"
                            strStatus = "HTTP/1.1 405 Method Not Allowed";

                            nsStream.Close();
                            bRunning = false;
                            break;
                    }
                }
            }
        }
    }
}
