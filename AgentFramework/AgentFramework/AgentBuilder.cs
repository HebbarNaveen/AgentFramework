#nullable enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace AgentFramework;

public class AgentBuilder : IAgentBuilder
{
    private Kernel kernel { get; set; }
    private IKernelBuilder builder { get; set; }

    /// <summary>
    /// Initializes a new instance of the AgentBuilder class. This constructor sets up the Kernel Builder and prepares it for configuration. You can use the WithModel, WithLocalMCPTools, WithRemoteMCPTools, WithRESTAPI, and WithNativeCodePlugin methods to add capabilities to the Agent before calling Build to create the Kernel instance.
    /// </summary>
    public AgentBuilder()
    {

        // 1. Initialize the Kernel Builder
        builder = Kernel.CreateBuilder();
        // 2. Configure the Kernel Builder with default settings (optional)
        kernel = builder.Build();
    }

    /// <summary>
    /// Adds a language model to the Agent. You can specify the model ID, endpoint, and API key for Azure OpenAI or OpenAI.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="endpoint"></param>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    public IAgentBuilder WithModel(string modelId, string endpoint, string apiKey)
    {
        /*  //  Add OpenAI Chat Completion service
             // Ensure you have the 'Microsoft.SemanticKernel.Connectors.OpenAI' NuGet package installed.
             builder.AddOpenAIChatCompletion(
                 modelId: "gpt-5.1",                // The OpenAI model you want to use
                 apiKey: "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"     // Your API key
             ); */

        // For Azure OpenAI
        builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
        Console.WriteLine($"Model '{modelId}' added is added to the Agent.");
        return this;
    }

    /// <summary>
    /// Adds tools from an MCP client to the Kernel as a plugin. You can specify a local MCP client by providing the command to start it, or a remote MCP client by providing the base URL and API key.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="command"></param>
    /// <param name="arguments"></param>
    /// <param name="environmentVariables"></param>
    /// <returns></returns>
    public IAgentBuilder WithLocalMCPTools(string name, string command, List<string>? arguments = null, Dictionary<string, string?>? environmentVariables = null)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(command) || arguments == null)
        {
            Console.WriteLine("MCP client configuration is incomplete, skipping registration.");
            return this;
        }
        var mcpClient = MCPClient.CreateLocalMCPClient(name, command, arguments, environmentVariables ?? new Dictionary<string, string?>()).Result;
        builder.AddMcpToolsAsync(mcpClient, name).Wait();
        Console.WriteLine($"Local '{name}' MCP tools are added to the Agent.");
        return this;
    }

    /// <summary>
    /// Adds tools from a remote MCP client to the Kernel as a plugin. You need to provide the base URL and API key for the remote MCP client. The method will create an MCP client that connects to the remote endpoint and registers its tools in the Agent.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="baseUrl"></param>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    public IAgentBuilder WithRemoteMCPTools(string name, string baseUrl, string? apiKey = null)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(baseUrl))
        {
            Console.WriteLine("Remote MCP client configuration is incomplete, skipping registration.");
            return this;
        }
        var mcpClient = MCPClient.CreateRemoteMCPClient(baseUrl, apiKey).Result;
        builder.AddMcpToolsAsync(mcpClient, name).Wait();
        Console.WriteLine($"Remote '{name}' MCP tools are added to the Agent.");
        return this;
    }

    /// <summary>
    /// Adds tools from an OpenAPI/Swagger endpoint to the Kernel as a plugin. Supports optional Bearer token authentication.
    /// </summary>
    /// <param name="pluginName"></param>
    /// <param name="uri"></param>
    /// <param name="authToken"></param>
    /// <returns></returns>
    public Kernel WithRESTAPI(string pluginName, Uri uri, string? authToken = null)
    {
        if (string.IsNullOrEmpty(pluginName) || string.IsNullOrEmpty(uri.ToString()))
        {
            Console.WriteLine("REST API configuration is incomplete, skipping registration.");
            return kernel;
        }
        kernel.AddRESTAPIAsync(pluginName, uri, authToken).Wait();
        Console.WriteLine($"REST API '{pluginName}' is added to the Agent.");
        return kernel;
    }

    /// <summary>
    /// Adds a native code plugin to the Agent. The plugin is implemented as a C# class that contains methods decorated with [KernelFunction] attribute. The methods will be registered as tools in the Agent and can be invoked by the language model. You need to specify the type of the plugin class and a name for the plugin.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public IAgentBuilder WithNativeCodePlugin<T>(string name) where T : class
    {
        builder.Plugins.AddFromType<T>(name);
        Console.WriteLine($"Native code plugin '{name}' is added to the Agent.");
        return this;
    }

    /// <summary>
    /// Builds the Agent by creating a Kernel instance with the configured services and plugins. This method should be called after all the desired configurations are done. It returns a Kernel object that represents the Agent and can be used to run prompts and invoke tools.
    /// </summary>
    /// <returns></returns>
    public Kernel Build()
    {
        kernel = builder.Build();
        return kernel;
    }

    /// <summary>
    /// Adds logging capabilities to the Agent. This method configures the logging services in the Kernel to use console logging with a minimum log level of Error. You can customize the logging configuration by modifying the AddLogging method. Logging can help you monitor the Agent's behavior and troubleshoot issues.
    /// </summary>
    /// <returns></returns>
    public IAgentBuilder WithLogging()
    {
        builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Error));
        Console.WriteLine("Logging is added to the Agent.");
        return this;
    }
}
