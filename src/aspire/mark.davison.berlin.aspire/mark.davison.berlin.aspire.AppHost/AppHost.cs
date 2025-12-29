var builder = DistributedApplication.CreateBuilder(args);

/*
var redis = builder
    .AddRedis(AspireConstants.Redis);

var db = builder
    .AddPostgres("postgresql")
    .AddDatabase("berlin", "berlin");
*/

var api = builder
    .AddProject<mark_davison_berlin_api>(AspireConstants.Api)
    .WithNonProxiedHttpsEndpoint()
    .WithCommonHealthChecks()
    .WithExternalHttpEndpoints();
//.WithEnvironment("BERLIN__REDIS__HOST", redis.GetEndpoint(AspireConstants.Redis).Host);

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
    .WithReference(api)
    .WithEnvironment("BERLIN__JOB_CHECK_RATE", "*/1 * * * *")
    ;

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
