#nullable enable

using ModelContextProtocol.Client;



namespace AgentFramework;

public class MCPClient
{

    /// <summary>
    /// Creates a new MCP client that connects to a local MCP server using the specified command, arguments, and environment variables. The method starts the local MCP server as a subprocess and establishes communication with it using standard input/output streams. This allows the Agent to interact with the tools exposed by the local MCP client as plugins in the Kernel.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="command"></param>
    /// <param name="arguments"></param>
    /// <param name="environmentVariables"></param>
    /// <returns></returns>
    public static async Task<McpClient> CreateLocalMCPClient(string name, string command, List<string>? arguments = null, IDictionary<string, string?>? environmentVariables = null)
    {

        var client = await McpClient.CreateAsync(
        new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = name,
            Command = command,
            Arguments = arguments ?? new List<string>(),
            EnvironmentVariables = environmentVariables

        }));
        return client;

    }
    /// <summary>
    /// Creates a new MCP client that connects to a remote MCP server using the specified base URL and API key. The method establishes communication with the remote MCP server over HTTP and allows the Agent to interact with the tools exposed by the remote MCP client as plugins in the Kernel. The API key is included in the Authorization header of the HTTP requests to authenticate with the remote MCP server.
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    public static async Task<McpClient> CreateRemoteMCPClient(string baseUrl, string? apiKey = null)
    {
        var client = await McpClient.CreateAsync(
        new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(baseUrl),
            Name = "AuthenticatedRemoteMCP",
            AdditionalHeaders = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + apiKey }
            }
        }));
        return client;

    }
}
