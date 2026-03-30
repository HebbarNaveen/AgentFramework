// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

#nullable enable

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;
using CustomAgent.Models;
using AgentFramework;




namespace CustomAgent
{
    class CustomAgent
    {

        private static void Main()
        {
            try
            {
                // build configuration manager
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json", optional: false, reloadOnChange: true) // Read API  keys from a secure location in production code, such as Azure Key Vault or an environment variable. For this example, we are reading it from config.json for simplicity, but this is not recommended for production applications.
                    .AddEnvironmentVariables()
                    .Build();

                var config = new Configuration();
                ConfigurationBinder.Bind(configuration, config);

                AgentBuilder builder = new AgentBuilder();
                builder.WithModel(config.model.id, config.model.endpoint, config.model.api_key)
                       .WithLogging();
                builder.WithNativeCodePlugin<SendCloudMessagePlugin>("Send_Device_To_Cloud_Message");
                builder.WithNativeCodePlugin<StackHawk>("RunStackHawkScan");

                if (config?.mcp_servers != null)
                {
                    foreach (var localServer in config.mcp_servers.local_servers)
                    {
                        if (localServer != null)
                        {
                            builder.WithLocalMCPTools(localServer.name, localServer.command, localServer.args, localServer.environment_variables);
                        }
                    }
                    foreach (var remoteServer in config.mcp_servers.remote_servers)
                    {
                        if (remoteServer != null)
                        {
                            builder.WithRemoteMCPTools(remoteServer.name, remoteServer.url, remoteServer.api_key);
                        }
                    }
                }
                var kernel = builder.Build();

                if (config?.rest_apis != null)
                {
                    foreach (var rest in config.rest_apis)
                    {
                        if (rest != null)
                        {
                            builder.WithRESTAPI(rest.plugin_name, new Uri(rest.uri));
                        }
                    }
                }

                LaunchChat(kernel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during initialization: {ex.Message}");
            }
        }

        /// <summary>
        /// Launches a simple console-based chat interface with the custom agent. The conversation history is maintained and sent with each user input to provide context to the model. The chat continues until the user types 'exit'. At the end of the conversation, the entire chat history is saved to a text file named "chat_history.txt".
        /// </summary>
        /// <param name="kernel"></param>
        private static void LaunchChat(Kernel kernel)
        {
            // Retrieve the chat completion service
            var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
            var openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new FunctionChoiceBehaviorOptions
                {
                    RetainArgumentTypes = true
                })
            };

            Console.WriteLine("Hello! I am your custom agent. Ask me anything or type 'exit' to quit.");

            // Create a history store the conversation
            var history = new ChatHistory();

            // Initiate a back-and-forth chat
            string? userInput;
            do
            {
                Console.WriteLine("\n--------------------------------------------------\n");
                // Collect user input
                Console.Write("User > ");
                userInput = Console.ReadLine();

                // Add user input
                if (string.IsNullOrEmpty(userInput))
                {
                    continue;
                }
                history.AddUserMessage(userInput);

                // Get the response from the AI
                var result = chatCompletionService.GetChatMessageContentAsync(
                    history,
                    executionSettings: openAIPromptExecutionSettings,
                    kernel: kernel).Result;

                // Print the results
                Console.WriteLine("\n--------------------------------------------------\n");
                Console.WriteLine("Assistant > " + result);
                Thread.Sleep(TimeSpan.FromSeconds(3)); // Simulate some delay for print to complete before the next message is printed

                // Add the message from the agent to the chat history
                history.AddMessage(result.Role, result.Content ?? string.Empty);

            } while (!string.IsNullOrEmpty(userInput) && userInput.ToLower() != "exit");

            File.WriteAllLines("chat_history.txt", history.Select(m => $"{m.Role}: {m.Content}"));
        }

    }
}
