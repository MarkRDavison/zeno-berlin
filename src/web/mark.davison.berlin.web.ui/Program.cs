var bffRoot = "https://localhost:40000";
var authConfig = new AuthenticationConfig();
authConfig.SetBffBase(bffRoot);

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    })
    .UseBerlinWeb(authConfig);

await builder.Build().RunAsync();