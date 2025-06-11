using Dapr.Client;
using System.Text.Json.Serialization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // 创建 Dapr 客户端，用于调用名为 "order-processor" 的服务
        var client = DaprClient.CreateInvokeHttpClient(appId: "order-processor");

        for (int i = 1; i <= 20; i++)
        {
            // 创建一个新的订单对象，订单号为 i
            var order = new Order(i);

            var cts = new CancellationTokenSource();
            // 支持 Ctrl+C 取消操作
            Console.CancelKeyPress += (sender, e) => cts.Cancel();

            // 调用远程服务，发送订单数据
            var response = await client.PostAsJsonAsync("/orders", order, cts.Token);

            Console.WriteLine("Order passed: " + order);

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}

/// <summary>
/// 订单类，包含订单编号。
/// 使用 record 类型实现不可变数据结构，并通过 JsonPropertyName 指定序列化字段名。
/// </summary>
public record Order([property: JsonPropertyName("orderId")] int OrderId);