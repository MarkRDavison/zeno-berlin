namespace mark.davison.berlin.shared.commands.Igntion;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinCommands(this IServiceCollection services)
    {
        services.AddTransient<ICommandProcessor<AddStoryCommandRequest, AddStoryCommandResponse>, AddStoryCommandProcessor>();
        services.AddTransient<ICommandValidator<AddStoryCommandRequest, AddStoryCommandResponse>, AddStoryCommandValidator>();
        return services;
    }
}
