using Microsoft.Net.Http.Headers;

namespace mark.davison.berlin.api.fakesite;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddScoped<IStoryGenerationService, StoryGenerationService>()
            .AddSingleton<IStoryGenerationStateService, StoryGenerationStateService>()
            .AddCors(options =>
                options.AddPolicy("AllowOrigin", _ => _
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .SetIsOriginAllowed(_ => true)// TODO: Config driven
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .Build()
                ));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors("AllowOrigin");

        app.UseHttpsRedirection();

        app.UseRouting();


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/works/{externalId}", async (HttpContext context,
                [FromRoute] int externalId,
                [FromServices] IStoryGenerationService storyGenerationService,
                CancellationToken cancellationToken) =>
            {
                var storyPage = await storyGenerationService.GenerateStoryPage(externalId, null, cancellationToken);

                return Results.Content(storyPage, new MediaTypeHeaderValue("text/html"));
            });

            endpoints.MapGet("/works/{externalId}/chapters/{chapterId}", async (
                HttpContext context,
                [FromRoute] int externalId,
                [FromRoute] int chapterId,
                [FromServices] IStoryGenerationService storyGenerationService,
                CancellationToken cancellationToken) =>
            {
                var storyPage = await storyGenerationService.GenerateStoryPage(externalId, chapterId, cancellationToken);

                return Results.Content(storyPage, new MediaTypeHeaderValue("text/html"));
            });

            endpoints.MapPost("/api/reset", async ([FromServices] IStoryGenerationStateService storyGenerationStateService) =>
            {
                await storyGenerationStateService.ResetAsync();
            });
        });
    }
}
