// 程序主入口，定义 Web API 服务
internal class Program
{
    // 主方法，异步启动 Web 应用
    private static async Task Main(string[] args)
    {
        // 创建 WebApplication 构建器
        var builder = WebApplication.CreateBuilder(args);

        // 构建 WebApplication 实例
        var app = builder.Build();

        // 如果处于开发环境，启用开发者异常页面
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // 映射 POST 路由 "/orders"，接收订单对象
        app.MapPost("/orders", (Order order) =>
        {
            Console.WriteLine("Order received : " + order);
            // 返回订单对象的字符串表示
            return order.ToString();
        });

        // 启动 Web 应用并监听请求
        await app.RunAsync();
    }
}

/// <summary>
/// 订单记录类型，包含订单编号。
/// </summary>
public record Order(int orderId);
