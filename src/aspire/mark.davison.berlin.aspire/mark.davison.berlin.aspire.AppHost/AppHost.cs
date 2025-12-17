using mark.davison.berlin.aspire.ServiceDefaults;
using mark.davison.berlin.shared.constants;
using Projects;


var builder = DistributedApplication.CreateBuilder(args);



var api = builder
    .AddProject<mark_davison_berlin_api>(AspireConstants.Api)
    .WithNonProxiedHttpsEndpoint()
    .WithCommonHealthChecks()
    .WithExternalHttpEndpoints();

var jobs = builder
    .AddProject<mark_davison_berlin_api_jobs>(AspireConstants.Jobs)
    .WithNonProxiedHttpsEndpoint()
    .WithCommonHealthChecks()
    .WithExternalHttpEndpoints()
    .WithReference(api);

var orchestrator = builder
    .AddProject<mark_davison_berlin_api_orchestrator>(AspireConstants.Orchestrator)
    .WithNonProxiedHttpsEndpoint()
    .WithCommonHealthChecks()
    .WithExternalHttpEndpoints()
    .WithReference(api);

var fakesite = builder
    .AddProject<mark_davison_berlin_api_fakesite>(AspireConstants.Fakesite)
    .WithNonProxiedHttpsEndpoint()
    // TODO: .WithCommonHealthChecks()
    .WithExternalHttpEndpoints()
    .WithReference(api);

var bff = builder
    .AddProject<mark_davison_berlin_bff>(AspireConstants.Bff)
    .WithNonProxiedHttpsEndpoint()
    // TODO: .WithCommonHealthChecks()
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WithReference(jobs)
    .WithReference(orchestrator)
    .WithReference(fakesite);

var web = builder
    .AddProject<mark_davison_berlin_web_ui>(AspireConstants.Web)
    .WithNonProxiedHttpsEndpoint()
    .WithExternalHttpEndpoints()
    .WithReference(bff);

builder.Build().Run();
