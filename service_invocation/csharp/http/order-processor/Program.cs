
internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.MapPost("/orders", (Order order) =>
        {
            Console.WriteLine("Order received : " + order);
            return order.ToString();
        });

        await app.RunAsync();
    }
}

public record Order(int orderId);
