namespace mark.davison.berlin.bff;

public sealed class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static bool IsAspire() =>
        Environment.GetEnvironmentVariable("ASPIRE_HOST") != null ||
        Environment.GetEnvironmentVariable("IS_ASPIRE") == "true";

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls(urls: Environment.GetEnvironmentVariable("BERLIN__URL") ?? "https://0.0.0.0:40000");
            })
            .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
            {
                configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                configurationBuilder.AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true);
                configurationBuilder.AddEnvironmentVariables();
            });
}