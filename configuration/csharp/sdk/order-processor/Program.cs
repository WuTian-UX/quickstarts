using Dapr.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // 配置存储名称常量
        const string DAPR_CONFIGURATION_STORE = "configstore";

        // 需要获取的配置项列表
        var CONFIGURATION_ITEMS = new List<string> { "orderId1", "orderId2" };

        // 订阅ID，初始为空
        string subscriptionId = string.Empty;

        // 创建 Dapr 客户端
        var client = new DaprClientBuilder().Build();

        // 从配置存储获取配置
        GetConfigurationResponse config = await client.GetConfiguration(DAPR_CONFIGURATION_STORE, CONFIGURATION_ITEMS);
        foreach (var item in config.Items)
        {
            // 序列化并输出每个配置项的值
            var cfg = System.Text.Json.JsonSerializer.Serialize(item.Value);
            Console.WriteLine("Configuration for " + item.Key + ": " + cfg);
        }

        // 300 秒后自动退出应用
        var shutdownTimer = new System.Timers.Timer();
        shutdownTimer.Interval = 300000;

        // 定时器到期时，取消订阅并退出应用
        shutdownTimer.Elapsed += (o, e) => Task.Run(async () => await unsubscribe(subscriptionId)).GetAwaiter().GetResult();
        shutdownTimer.Start();

        // 订阅配置变更
        SubscribeConfigurationResponse subscribe = await client.SubscribeConfiguration(DAPR_CONFIGURATION_STORE, CONFIGURATION_ITEMS);

        // 打印配置变更内容
        await foreach (var configItem in subscribe.Source)
        {
            // 首次订阅时只返回订阅ID，不包含配置项
            if (configItem.Keys.Count == 0)
            {
                Console.WriteLine("App subscribed to config changes with subscription id: " + subscribe.Id);
                subscriptionId = subscribe.Id;
                continue;
            }
            // 序列化并输出配置变更内容
            var cfg = System.Text.Json.JsonSerializer.Serialize(configItem);
            Console.WriteLine("Configuration update " + cfg);
        }

        // 取消订阅配置变更并退出应用的方法
        async Task unsubscribe(string subscriptionId)
        {
            try
            {
                await client.UnsubscribeConfiguration(DAPR_CONFIGURATION_STORE, subscriptionId);
                Console.WriteLine("App unsubscribed from config changes");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error unsubscribing from config updates: " + ex.Message);
            }
        }
    }
}