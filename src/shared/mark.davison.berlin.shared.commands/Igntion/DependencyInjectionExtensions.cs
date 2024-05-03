namespace mark.davison.berlin.shared.commands.Igntion;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseBerlinCommands(this IServiceCollection services)
    {
        services.AddTransient<ICommandProcessor<EditStoryCommandRequest, EditStoryCommandResponse>, EditStoryCommandProcessor>();
        services.AddTransient<ICommandProcessor<DeleteStoryCommandRequest, DeleteStoryCommandResponse>, DeleteStoryCommandProcessor>();
        services.AddTransient<ICommandProcessor<AddStoryCommandRequest, AddStoryCommandResponse>, AddStoryCommandProcessor>();
        services.AddTransient<ICommandValidator<AddStoryCommandRequest, AddStoryCommandResponse>, AddStoryCommandValidator>();
        services.AddTransient<ICommandProcessor<ExportCommandRequest, ExportCommandResponse>, ExportCommandProcessor>();
        services.AddTransient<ICommandProcessor<ImportCommandRequest, ImportCommandResponse>, ImportCommandProcessor>();
        services.AddTransient<ICommandValidator<ImportCommandRequest, ImportCommandResponse>, ImportCommandValidator>();
        services.AddTransient<ICommandProcessor<EditFandomCommandRequest, EditFandomCommandResponse>, EditFandomCommandProcessor>();
        services.AddTransient<ICommandProcessor<AddFandomCommandRequest, AddFandomCommandResponse>, AddFandomCommandProcessor>();
        services.AddTransient<ICommandValidator<AddFandomCommandRequest, AddFandomCommandResponse>, AddFandomCommandValidator>();
        services.AddTransient<ICommandValidator<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse>, AddStoryUpdateCommandValidator>();
        services.AddTransient<ICommandProcessor<AddStoryUpdateCommandRequest, AddStoryUpdateCommandResponse>, AddStoryUpdateCommandProcessor>();

        return services;
    }
}
