#nullable enable

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using ModelContextProtocol.Client;

namespace AgentFramework;

public static class KernelExtensions
{
    /// <summary>
    /// Adds tools from an MCP client to the Kernel as a plugin.        
    /// </summary>
    /// <param name="kernel"></param>
    /// <param name="mcpClient"></param>
    /// <param name="pluginName"></param>
    /// <returns></returns>
    public static async Task AddMcpToolsAsync(
        this IKernelBuilder builder,
        McpClient mcpClient,
        string pluginName)
    {

        // Discover tools from your MCP client
        var mcpTools = await mcpClient.ListToolsAsync();

        // Register them as a plugin in the Kernel
        builder.Plugins.AddFromFunctions(pluginName,
            mcpTools.Select(tool => tool.AsKernelFunction())
        );

    }

    /// <summary>
    /// Adds tools from an OpenAPI/Swagger endpoint to the Kernel as a plugin. Supports optional Bearer token authentication.
    /// </summary>
    /// <param name="kernel"></param>
    /// <param name="pluginName"></param>
    /// <param name="uri"></param>
    /// <param name="authToken"></param>
    /// <returns></returns>
    public static async Task AddRESTAPIAsync(
        this Kernel kernel,
        string pluginName,
        Uri uri,
        string? authToken = null
        )
    {

        // Configure the OpenAPI options with an Auth Callback
        var openApiOptions = new OpenApiFunctionExecutionParameters
        {
            // The AuthCallback is invoked just before the HTTP request is sent
            AuthCallback = (request, cancellationToken) =>
            {
                if (!string.IsNullOrEmpty(authToken))
                {
                    // Inject the Bearer token into the Authorization header
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                return Task.CompletedTask;
            },
            // Optional: If you need to ignore SSL errors (dev env only) or customize the client
            //HttpClient = new HttpClient() 
        };

        // Import the Plugin from the OpenAPI specification, passing in the execution parameters

        await kernel.ImportPluginFromOpenApiAsync(
            pluginName: pluginName,
            uri: uri,
            executionParameters: openApiOptions
        );

    }
}
