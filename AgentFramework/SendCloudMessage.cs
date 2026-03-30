#nullable enable

using Microsoft.Azure.Devices.Client;
using System.Text;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace CustomAgent
{
public class SendCloudMessagePlugin
{
        private static DeviceClient? s_deviceClient;

        // The device connection string to authenticate the device with your IoT hub. Read this from a secure location in production code, such as Azure Key Vault or an environment variable. For this example, we are hardcoding it for simplicity, but this is not recommended for production applications.
        private const string s_connectionString = "HostName=henkel-dev-iothub.azure-devices.net;DeviceId=testdevice1;SharedAccessKey=VRJzQ08ZNL6q4K/vtTTUFpVOVuVw/x+/WH3SwnD3NpI=";
   
    public SendCloudMessagePlugin()
    {
        // Connect to the IoT hub using the MQTT protocol
        s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, TransportType.Mqtt);
    }
    
    // Async method to send simulated telemetry
     [KernelFunction("Send_Device_To_Cloud_Message")]
     [Description("Sends a device-to-cloud message")]   
    private static async void SendDeviceToCloudMessagesAsync()
        {
            
            string messageString = File.ReadAllText(@".\message.json");
           
            //while (true)
            {

                var message = new Message(Encoding.ASCII.GetBytes(messageString))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8"
                };

                // Send the tlemetry message
                if (s_deviceClient is null)
                {
                    return;
                }
                await s_deviceClient.SendEventAsync(message).ConfigureAwait(false);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000).ConfigureAwait(false);
            }
        }
}
}
