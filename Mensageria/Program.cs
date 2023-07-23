using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.ReasonCodes;
using HiveMQtt.MQTT5.Types;
using System.Numerics;
using System.Text.Json;


var options = new HiveMQClientOptions
{
    Host = "f66a23c8691340f3913ec9cf0504c6c4.s2.eu.hivemq.cloud",
    Port = 8883,
    UseTLS = true,
    UserName = "teste",
    Password = "Teste123",
};

var client = new HiveMQClient(options);


Console.WriteLine($"Connecting to {options.Host} on port {options.Port} ...");

// Connect
HiveMQtt.Client.Results.ConnectResult connectResult;
try
{
    connectResult = await client.ConnectAsync().ConfigureAwait(false);
    if (connectResult.ReasonCode == ConnAckReasonCode.Success)
    {
        Console.WriteLine($"Connect successful: {connectResult}");
    }
    else
    {
        // FIXME: Add ToString
        Console.WriteLine($"Connect failed: {connectResult}");
        Environment.Exit(-1);
    }
}
catch (System.Net.Sockets.SocketException e)
{
    Console.WriteLine($"Error connecting to the MQTT Broker with the following socket error: {e.Message}");
    Environment.Exit(-1);
}
catch (Exception e)
{
    Console.WriteLine($"Error connecting to the MQTT Broker with the following message: {e.Message}");
    Environment.Exit(-1);
}

client.OnMessageReceived += (sender, args) =>
{
    string received_message = args.PublishMessage.PayloadAsString;
    Console.WriteLine(received_message);

};

// Subscribe
await client.SubscribeAsync("test").ConfigureAwait(false);



//initialise telemetry values
int imei1 = 456456786;
int imei2 = 789789798;
var rand = new Random();


Console.WriteLine("Publishing message...");

while (true)
{
    int currentTemperature = (int)(imei1 + rand.NextInt64());
    int currentHumidity = (int)(imei2 + rand.NextInt64());
    var msg = JsonSerializer.Serialize(
        new
        {
            serial = "FAKE5555SERIAL",
            imei_1 = currentTemperature,
            imei_2 = currentHumidity,
        });
    //Publish MQTT messages
    var result = await client.PublishAsync("test", msg, QualityOfService.AtLeastOnceDelivery).ConfigureAwait(false);

}