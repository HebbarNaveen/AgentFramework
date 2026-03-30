#nullable enable

using Microsoft.SemanticKernel;

namespace AgentFramework;

public interface IAgentBuilder
{
    /// <summary>
    /// Adds a language model to the Agent. You can specify the model ID, endpoint, and API key for Azure OpenAI or OpenAI.
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="endpoint"></param>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    public IAgentBuilder WithModel(string modelId, string endpoint, string apiKey);
    /// <summary>
    /// Adds tools from an MCP client to the Kernel as a plugin. You can specify a local MCP client by providing the command to start it, or a remote MCP client by providing the base URL and API key.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="command"></param>
    /// <param name="arguments"></param>
    /// <param name="environmentVariables"></param>
    /// <returns></returns>
    public IAgentBuilder WithLocalMCPTools(string name, string command, List<string>? arguments = null, Dictionary<string, string?>? environmentVariables = null);
    /// <summary>
    /// Adds tools from a remote MCP client to the Kernel as a plugin. You need to provide the base URL and API key for the remote MCP client. The method will create an MCP client that connects to the remote endpoint and registers its tools in the Agent.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="baseUrl"></param>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    public IAgentBuilder WithRemoteMCPTools(string name, string baseUrl, string? apiKey = null);
    /// <summary>
    /// Adds tools from an OpenAPI/Swagger endpoint to the Kernel as a plugin. Supports optional Bearer token authentication.
    /// </summary>
    /// <param name="pluginName"></param>
    /// <param name="uri"></param>
    /// <param name="authToken"></param>
    /// <returns></returns>
    public Kernel WithRESTAPI(string pluginName, Uri uri, string? authToken = null);
    /// <summary>
    /// Adds a native code plugin to the Agent. You can specify the plugin class type and a name for the plugin. The plugin class should implement the necessary interfaces to be registered as a plugin in the Kernel. This method allows you to extend the Agent's capabilities with custom code that is not exposed through an API or MCP client.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public IAgentBuilder WithNativeCodePlugin<T>(string name) where T : class;
    /// <summary>
    /// Configures the Agent to use logging.
    /// </summary>
    /// <returns></returns>
    public IAgentBuilder WithLogging();
    /// <summary>
    /// Builds and returns the configured Kernel instance that represents the Agent. After calling this method, the Agent is ready to be used for executing tasks and interacting with tools. You can call this method after configuring the Agent with the desired models, tools, plugins, and logging options using the other methods in this interface.
    /// </summary>
    /// <returns></returns>
    public Kernel Build();

}
