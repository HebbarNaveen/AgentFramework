#nullable enable

using System.ComponentModel;
using System.Text.Json.Nodes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace CustomAgent;

public class StackHawk
{
    [KernelFunction("RunStackHawkScan")]
    [Description("Run a StackHawk scan using MCP tools for the provided URI. ")]
    private static async Task<string> RunStackHawkScan(Kernel kernel, [Description("The URI to scan")] string uri)
    {
        var openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            ResponseFormat = "json_object",
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,

        };
        var result = await kernel.InvokePromptAsync("Use the setup_stackhawk_for_project MCP tool to setup StackHawk for a new project with Application Name: ScanTestAgent. Return Application ID only in the response as json", new KernelArguments(openAIPromptExecutionSettings));

        Console.WriteLine($"Application setup result: {result}");
        int startIndex = result.ToString().IndexOf('{');
        var node = JsonNode.Parse(result.ToString().Substring(startIndex));
        if (node is null)
        {
            Console.WriteLine("Could not parse application ID from the MCP tool. Aborting StackHawk scan.");
            return "Could not parse application ID from the MCP tool. Aborting StackHawk scan.";
        }
        string? applicationId = node["applicationId"]?.ToString();
        if (string.IsNullOrEmpty(applicationId))
        {
            Console.WriteLine("Could not get application ID from the MCP tool. Aborting StackHawk scan.");
            return "Could not get application ID from the MCP tool. Aborting StackHawk scan.";
        }
        GenerateConfigurationFile(applicationId, uri);
        Console.WriteLine("Configuration file generated successfully.");
        Console.WriteLine("Starting StackHawk scan...Please wait...\nThis may take a few minutes depending on the size of the application and the scan settings.");
        result = await kernel.InvokePromptAsync("Run a StackHawk scan using the run_stackhawk_scan MCP tool and return the scan results as json.", new KernelArguments(openAIPromptExecutionSettings));
        return result.ToString();

    }

    private static void GenerateConfigurationFile(string? applicationId, string hostUri)
    {
        var config = File.ReadAllText("stackhawk_template.yml");
        config = config.Replace("00000000-0000-0000-0000-000000000000", applicationId);
        config = config.Replace("https://juice-shop.herokuapp.com", hostUri);
        File.WriteAllText("stackhawk.yml", config);
        Console.WriteLine($"Configuration file content: {config}");
    }


}
