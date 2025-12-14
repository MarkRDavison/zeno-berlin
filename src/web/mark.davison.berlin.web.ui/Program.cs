var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var bff = Environment.GetEnvironmentVariable("BFF_BASE_URI");

builder.Services
    .AddScoped(sp => new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    })
    .UseBerlinComponents()
    .UseBerlinFeatures()
    .UseBerlinServices();

await builder.Build().RunAsync();
