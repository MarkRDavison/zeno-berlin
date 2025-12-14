using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder
    .AddProject<mark_davison_berlin_api>("api")
    .WithEnvironment("IS_ASPIRE", "true");

var fakesite = builder
    .AddProject<mark_davison_berlin_api_fakesite>("fakesite")
    .WithEnvironment("IS_ASPIRE", "true")
    .WithReference(api);

var bff = builder
    .AddProject<mark_davison_berlin_bff>("bff")
    .WithEnvironment("IS_ASPIRE", "true")
    .WithReference(api)
    .WithEnvironment("BERLIN__API_ENDPOINT", api.GetEndpoint("https"));

var indexFile = Path.Combine("path_to_wasm_wwwroot", "index.html");
var content = File.ReadAllText(indexFile);
content = content.Replace("%BFF_BASE_URI%", bff.GetEndpoint("https"));
File.WriteAllText(indexFile, content);

var web = builder
    .AddProject<mark_davison_berlin_web_ui>("web")
    .WithEnvironment("IS_ASPIRE", "true")
    .WithReference(bff);

var curr = Directory.GetCurrentDirectory();

builder.Build().Run();
