using System.Collections.Generic;

namespace HTTPLib {
    public interface IHTTPMethodHandler {
        // The name of the method handled. Must be all uppercase letters.
        string MethodName {
            get;
        }

        // Generate the response to an HTTP request using this method. Assumes
        // the method name in the request line matches the method name returned
        // by the above property.
        HTTPResponse GenResponse(HTTPRequest hrRequest,
                                 Dictionary<string, IHTTPMethodHandler> dictPlugins,
                                 Dictionary<string, string> dictServerConfig);
    }
}
